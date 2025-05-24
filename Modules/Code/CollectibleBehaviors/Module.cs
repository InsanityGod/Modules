using InsanityLib.Util;
using Modules.Code.BlockEntityBehaviors;
using Modules.Code.Interfaces.Modules;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;

namespace Modules.Code.CollectibleBehaviors
{
    public class Module : CollectibleBehavior
    {
        public const string moduleBehaviorKey = "module";
        public const string modulePropertiesKey = "moduleProperties";
        
        public Module(CollectibleObject collObj) : base(collObj) { }

        public override WorldInteraction[] GetHeldInteractionHelp(ItemSlot inSlot, ref EnumHandling handling) => new WorldInteraction[]
        {
            new()
            {
                MouseButton = EnumMouseButton.Right,
                ActionLangCode = "modules:apply",
                //TODO GetMatchingStacks
            }
        };

        public override void OnHeldInteractStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handHandling, ref EnumHandling handling)
        {
            if(blockSel != null)
            {
                var blockEntity = blockSel.FindBlockEntity(byEntity.World);

                if(IsApplicable(byEntity.Api, blockEntity, slot.Itemstack))
                {
                    if(byEntity.Api.Side == EnumAppSide.Server)
                    {
                        var item = slot.Itemstack.Clone();
                        item.StackSize = 1;
                        item.Collectible.GetBehavior<Module>().Randomize(byEntity.Api, item);

                        var properties = slot.Itemstack.Attributes[modulePropertiesKey]?.ToJsonToken();
                        var module = blockEntity.TryAddPermanentbehavior(
                            slot.Itemstack.Attributes.GetString(moduleBehaviorKey),
                            properties != null ? new JsonObject(properties) : new JsonObject(new JObject())
                        );

                        if(module != null)
                        {
                            slot.TakeOut(1);
                            slot.MarkDirty();
                            
                            if(module is BlockEntityModuleBase moduleBase) moduleBase.Item = item;
                        }
                    }

                    handHandling = EnumHandHandling.PreventDefault;
                    handling = EnumHandling.Handled;
                    return;
                }
            }

            base.OnHeldInteractStart(slot, byEntity, blockSel, entitySel, firstEvent, ref handHandling, ref handling);
        }

        public static Type GetModuleClass(ICoreAPI api, ItemStack itemStack)
        {
            var behaviorName = itemStack.Attributes.GetString(moduleBehaviorKey);
            if (string.IsNullOrEmpty(behaviorName)) return null; //No module

            var moduleClass = api.ClassRegistry.GetBlockEntityBehaviorClass(behaviorName);

            if(moduleClass == null) api.GetService<ILogger>().Warning("[Modules] Encountered invalid itemstack '{0}': Unknown module '{1}'", itemStack, behaviorName);
            else if(!typeof(IBlockEntityBehaviorModule).IsAssignableFrom(moduleClass))
            {
                api.GetService<ILogger>().Error("[Modules] Encountered invalid module '{0}': module is not and instance of {1}", itemStack, typeof(IBlockEntityBehaviorModule));
                return null; //Don't return anything if it's not a module class
            }

            return moduleClass;
        }

        public bool IsApplicable(ICoreAPI api, BlockEntity blockEntity, ItemStack itemStack)
        {
            var clientApi = api as ICoreClientAPI;
            
            var moduleClass = GetModuleClass(api, itemStack);
            if(moduleClass == null)
            {
                clientApi?.TriggerIngameError(this, Constants.InvalidModule, Lang.Get(Constants.InvalidModule));
                return false;
            }

            if(blockEntity == null)
            {
                clientApi?.TriggerIngameError(this, Constants.InvalidTarget, Lang.Get(Constants.InvalidTarget));
                return false;
            }

            if(blockEntity.Behaviors.Exists(module => module.GetType() == moduleClass)) //TODO some way to allow certain modules to be registered twice
            {
                clientApi?.TriggerIngameError(this, Constants.AlreadyApplied, Lang.Get(Constants.AlreadyApplied));
                return false;
            }

            var testMethod = moduleClass.GetMethod(nameof(IBlockEntityBehaviorModule.IsApplicableTo), BindingFlags.Static | BindingFlags.Public);
            if(!(bool)testMethod.Invoke(null, new object[] { blockEntity }))
            {
                clientApi?.TriggerIngameError(this, Constants.InvalidTarget, Lang.Get(Constants.InvalidTarget));
                return false;
            }

            return true;
        }

        public void Randomize(ICoreAPI api, ItemStack itemStack)
        {
            itemStack.Attributes ??= new TreeAttribute();
            if(itemStack.Attributes.HasAttribute(modulePropertiesKey)) return; //Already randomized
            
            var moduleClass = GetModuleClass(api, itemStack);
            if(moduleClass == null) return; //Unknown/invalid module

            var moduleProperties = itemStack.Attributes[modulePropertiesKey] = new TreeAttribute(); //Ensure the tree is present

            moduleClass.GetMethod(nameof(IBlockEntityBehaviorModule.RandomizeAttributes), BindingFlags.Static | BindingFlags.Public)
                .Invoke(null, new object[] { moduleProperties });
        }

        public override void GetHeldItemName(StringBuilder sb, ItemStack itemStack)
        {
            var moduleName = new AssetLocation(itemStack.Attributes?.GetString("module") ?? "invalid");
            sb.Append($" ({Lang.Get($"{itemStack.Collectible.Code.Domain}:module-{moduleName.Path}")})");
        }

        //TODO maybe find a a way to Randomize when you collect one
        //TODO maybe make it required to analyze the module first?

        public override void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo) => AppendModuleEffects(inSlot.Itemstack, dsc);

        public static bool AppendModuleEffects(ItemStack itemStack, StringBuilder dsc, bool SeperationLine = true)
        {
            var props = itemStack.Attributes?.GetTreeAttribute("moduleProperties");
            if(props == null) return false;

            var moduleName = new AssetLocation(itemStack.Attributes.GetString("module") ?? "invalid");
            
            var effectBuilder = new StringBuilder();
            var langKeyBase = $"{itemStack.Collectible.Code.Domain}:module-{moduleName.Path}-effect-";
            foreach((var key, var prop) in props)
            {
                var langKey = langKeyBase + key.ToLower();
                var languageStr = Lang.GetUnformatted(langKey);
                if(languageStr == langKey) continue;

                effectBuilder.AppendLine(string.Format(languageStr, prop.GetValue()));
            }

            if(effectBuilder.Length > 0)
            {
                if(SeperationLine) dsc.AppendLine();
                dsc.Append(effectBuilder);
                return true;
            }

            return false;
        }
    }
}

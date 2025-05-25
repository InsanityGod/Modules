using InsanityLib.Util;
using Modules.Code.BlockEntityBehaviors;
using Modules.Code.Interfaces.Modules;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;

namespace Modules.Code.CollectibleBehaviors
{
    public class Module : CollectibleBehavior
    {
        public const string moduleBehaviorKey = "module";
        public const string modulePropertiesKey = "moduleProperties";

        public Module(CollectibleObject collObj) : base(collObj)
        {
        }

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
            if (blockSel == null) return;

            var blockEntity = blockSel.FindBlockEntity(byEntity.World);

            if (byEntity.Api is not ICoreServerAPI serverApi)
            {
                if (!IsApplicable(byEntity.Api, blockEntity, slot.Itemstack)) return;
            }
            else if (TryApply(serverApi, slot.Itemstack, blockEntity))
            {
                slot.TakeOut(1);
                slot.MarkDirty();
            }

            handHandling = EnumHandHandling.PreventDefault;
            handling = EnumHandling.Handled;
        }

        public static bool TryApply(ICoreServerAPI serverApi, ItemStack itemStack, BlockEntity blockEntity)
        {
            if(!IsApplicable(serverApi, blockEntity, itemStack)) return false;

            var newItemStack = itemStack.Clone();
            newItemStack.StackSize = 1;
            newItemStack.Collectible.GetBehavior<Module>().Randomize(serverApi, newItemStack);

            var properties = newItemStack.Attributes[modulePropertiesKey]?.ToJsonToken();
            var module = blockEntity.TryAddPermanentbehavior(
                newItemStack.Attributes.GetString(moduleBehaviorKey),
                properties != null ? new JsonObject(properties) : new JsonObject(new JObject())
            );
            if (module is BlockEntityModuleBase moduleBase) moduleBase.Item = newItemStack;

            return module != null;
        }

        public static Type GetModuleClass(ICoreAPI api, ItemStack itemStack)
        {
            var behaviorName = itemStack.Attributes.GetString(moduleBehaviorKey);
            if (string.IsNullOrEmpty(behaviorName)) return null; //No module

            var moduleClass = api.ClassRegistry.GetBlockEntityBehaviorClass(behaviorName);

            if (moduleClass == null) api.GetService<ILogger>().Warning("[Modules] Encountered invalid itemstack '{0}': Unknown module '{1}'", itemStack, behaviorName);
            else if (!typeof(IBlockEntityBehaviorModule).IsAssignableFrom(moduleClass))
            {
                api.GetService<ILogger>().Error("[Modules] Encountered invalid module '{0}': module is not and instance of {1}", itemStack, typeof(IBlockEntityBehaviorModule));
                return null; //Don't return anything if it's not a module class
            }

            return moduleClass;
        }

        public static bool IsApplicable(ICoreAPI api, BlockEntity blockEntity, ItemStack itemStack)
        {
            var clientApi = api as ICoreClientAPI;

            var moduleClass = GetModuleClass(api, itemStack);
            if (moduleClass == null)
            {
                clientApi?.TriggerIngameError(typeof(Module), Constants.InvalidModule, Lang.Get(Constants.InvalidModule));
                return false;
            }

            if (blockEntity == null)
            {
                clientApi?.TriggerIngameError(typeof(Module), Constants.InvalidTarget, Lang.Get(Constants.InvalidTarget));
                return false;
            }

            if (blockEntity.Behaviors.Exists(module => module.GetType() == moduleClass)) //TODO some way to allow certain modules to be registered twice
            {
                clientApi?.TriggerIngameError(typeof(Module), Constants.AlreadyApplied, Lang.Get(Constants.AlreadyApplied));
                return false;
            }

            var testMethod = moduleClass.GetMethod(nameof(IBlockEntityBehaviorModule.IsApplicableTo), BindingFlags.Static | BindingFlags.Public);
            if (!(bool)testMethod.Invoke(null, new object[] { blockEntity }))
            {
                clientApi?.TriggerIngameError(typeof(Module), Constants.InvalidTarget, Lang.Get(Constants.InvalidTarget));
                return false;
            }

            return true;
        }

        public void Randomize(ICoreAPI api, ItemStack itemStack)
        {
            itemStack.Attributes ??= new TreeAttribute();
            if (itemStack.Attributes.HasAttribute(modulePropertiesKey)) return; //Already randomized

            var moduleClass = GetModuleClass(api, itemStack);
            if (moduleClass == null) return; //Unknown/invalid module

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
            if (props == null) return false;

            var moduleName = new AssetLocation(itemStack.Attributes.GetString("module") ?? "invalid");

            var effectBuilder = new StringBuilder();
            var langKeyBase = $"{itemStack.Collectible.Code.Domain}:module-{moduleName.Path}-effect-";
            foreach ((var key, var prop) in props)
            {
                var langKey = langKeyBase + key.ToLower();
                var languageStr = Lang.GetUnformatted(langKey);
                if (languageStr == langKey) continue;

                effectBuilder.AppendLine(string.Format(languageStr, prop.GetValue()));
            }

            if (effectBuilder.Length > 0)
            {
                if (SeperationLine) dsc.AppendLine();
                dsc.Append(effectBuilder);
                return true;
            }

            return false;
        }
    }
}
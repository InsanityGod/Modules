using InsanityLib.Attributes.Auto.Command;
using InsanityLib.Behaviors.BlockEntityBehaviors;
using InsanityLib.Enums.Auto.Commands;
using InsanityLib.Util;
using Modules.Code.BlockEntityBehaviors.Modules;
using Modules.Code.CollectibleBehaviors;
using Modules.Code.Interfaces.Modules;
using Modules.Config.Props;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace Modules.Code.Commands
{
    public static class DebugCommands
    {
        /// <summary>
        /// Adds the given module to the block entity (with randomized stats)
        /// </summary>
        /// <example>/module add AdditionalStorageModule</example>
        /// <example>/module add EngineModule</example>
        /// <example>/module add TransitionSpeedMutatorModule</example>
        [AutoCommand(RequiredPrivelege = "controlserver", Path = "module", Name = "add")]
        public static bool AddModule(ICoreServerAPI serverApi, [CommandParameter(Source = EParamSource.CallerTarget)] BlockEntityContainer blockEntity, [CommandParameter(Source = EParamSource.Specify)] string module)
        {
            var item = serverApi.World.GetItem("modules:module");
            var itemStack = new ItemStack(item)
            {
                Attributes = new TreeAttribute()
            };
            var modulePath = module.ToAssetLocation();
            if(!modulePath.HasDomain()) modulePath.Domain = "modules";
            itemStack.Attributes.SetString(Module.moduleBehaviorKey, modulePath);

            return Module.TryApply(serverApi, itemStack, blockEntity);
        }
        /// <summary>
        /// Removes the specified module from the block entity
        /// </summary>
        /// <example>/module remove AdditionalStorageModule</example>
        /// <example>/module remove EngineModule</example>
        /// <example>/module remove TransitionSpeedMutatorModule</example>
        [AutoCommand(RequiredPrivelege = "controlserver", Path = "module", Name = "remove")]
        public static string RemoveModule(ICoreServerAPI serverApi, [CommandParameter(Source = EParamSource.CallerTarget)] [Required(ErrorMessage = "Not targeting a blockentity")] BlockEntityContainer blockEntity, [CommandParameter(Source = EParamSource.Specify)] string module)
        {
            var manager = blockEntity.GetBehavior<PermanentBehaviorManager>();
            
            var modulePath = module.ToAssetLocation();
            if(!modulePath.HasDomain()) modulePath.Domain = "modules";
            var expectedBehavior = serverApi.ClassRegistry.GetBlockEntityBehaviorClass(modulePath);
            if(expectedBehavior == null || !typeof(IBlockEntityBehaviorModule).IsAssignableFrom(expectedBehavior)) return "Unknown module";
            
            var toRemove = manager?.Where(expectedBehavior.IsInstanceOfType).ToList();
            if(manager == null || toRemove.Count == 0) return "Blockentity does not have any modules";
            foreach(var behavior in toRemove) manager.RemoveBehavior(behavior);

            return $"Removed {toRemove.Count} behaviors";
        }

        /// <summary>
        /// Removes all modules from block entity
        /// </summary>
        /// <example>/module removeall</example>
        [AutoCommand(RequiredPrivelege = "controlserver", Path = "module", Name = "removeall", Side = EnumAppSide.Server)]
        public static string RemoveAllModules([CommandParameter(Source = EParamSource.CallerTarget)] [Required(ErrorMessage = "Not targeting a blockentity")] BlockEntityContainer blockEntity)
        {
            var manager = blockEntity.GetBehavior<PermanentBehaviorManager>();
            var toRemove = manager?.Where(beh => beh is IBlockEntityBehaviorModule).ToList();
            
            if(manager == null || toRemove.Count == 0) return "Blockentity does not have any modules";
            foreach(var behavior in toRemove) manager.RemoveBehavior(behavior);

            return $"Removed {toRemove.Count} behaviors";
        }

    }
}
using HarmonyLib;
using InsanityLib.Interfaces;
using Modules.Code.Interfaces.Modules;
using Modules.Config;
using Modules.Config.Props;
using System;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Util;
using Vintagestory.GameContent;
using Vintagestory.GameContent.Mechanics;

namespace Modules.Code.BlockEntityBehaviors.Modules
{
    public class AdditionalStorageModule : BlockEntityModuleBase, IBlockEntityBehaviorModule, IPermanentBehavior
    {
        public AdditionalStorageModuleProps Props { get; private set; }

        public AdditionalStorageModule(BlockEntity blockentity) : base(blockentity)
        {
        }

        public override void Initialize(ICoreAPI api, JsonObject properties)
        {
            base.Initialize(api, properties);
            Props = properties.AsObject<AdditionalStorageModuleProps>();
        }

        public void OnRuntimeAdded()
        {
            if (Api.Side != EnumAppSide.Server || Props.AdditionalSlotCount <= 0) return;

            try
            {
                var blockEntityContainer = Blockentity as BlockEntityContainer;
                var traverse = Traverse.Create(blockEntityContainer.Inventory);

                var slotsTraverse = traverse.Field("slots");
                if (!slotsTraverse.IsField) slotsTraverse = traverse.Property("slots");

                var slots = slotsTraverse.GetValue<ItemSlot[]>();
                if (slots == null) return;

                slotsTraverse.SetValue(slots.Append(blockEntityContainer.Inventory.GenEmptySlots(Props.AdditionalSlotCount)));
            }
            catch
            {
                //TODO logging
            }
        }

        //TODO allow for runtime removal
        public void OnRuntimeRemoved()
        {
            if(Props.AdditionalSlotCount <= 0) return;
            var blockEntityContainer = Blockentity as BlockEntityContainer;
            
            var traverse = Traverse.Create(blockEntityContainer.Inventory);

            var slotsTraverse = traverse.Field("slots");
            if (!slotsTraverse.IsField) slotsTraverse = traverse.Property("slots");

            var slots = slotsTraverse.GetValue<ItemSlot[]>();
            if (slots == null) return;

            var toKeep = slots[..^Props.AdditionalSlotCount];
            var toRemove = Enumerable.Range(toKeep.Length, Props.AdditionalSlotCount).ToArray();
            if(Api.Side == EnumAppSide.Server)
            {
                blockEntityContainer.Inventory.DropSlots(Pos.ToVec3d(), toRemove);
                foreach(var index in toRemove) blockEntityContainer.Inventory.dirtySlots.Remove(index); //Stop the game from trying to send upates on slots that no longer exist
            }
            else if(Api is ICoreClientAPI clientApi && blockEntityContainer.Inventory.HasOpened(clientApi.World.Player))
            {
                blockEntityContainer.Inventory.Close(clientApi.World.Player);
            }

            slotsTraverse.SetValue(toKeep);
            blockEntityContainer.MarkDirty();
        }

        public static bool IsApplicableTo(BlockEntity blockEntity) => 
            blockEntity is BlockEntityContainer
            && blockEntity is not BlockEntityItemFlow //No chutes and the like
            && blockEntity.GetBehavior<IMechanicalPowerNode>() == null; //No mechanical devices like querns

        public static void RandomizeAttributes(ITreeAttribute attributes)
        {
            attributes.SetInt(nameof(AdditionalStorageModuleProps.AdditionalSlotCount), (int)Math.Round(ModulesConfig.Instance.AdditionalStorage.AdditionalSlotCount.nextFloat()));
        }

    }
}
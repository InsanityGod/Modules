using HarmonyLib;
using InsanityLib.Interfaces;
using InsanityLib.Util;
using Modules.Code.Interfaces.Modules;
using Modules.Config;
using Modules.Config.Props;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace Modules.Code.BlockEntityBehaviors.Modules
{
    public class AdditionalStorageModule : BlockEntityModuleBase, IBlockEntityBehaviorModule, IPermanentBehavior
    {
        public AdditionalStorageModuleProps Props { get; private set;}
        
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
            if(Api.Side != EnumAppSide.Server || Props.AdditionalSlotCount <= 0) return;

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
        //public void OnRuntimeRemoved()
        
        public static bool IsApplicableTo(BlockEntity blockEntity) => blockEntity is BlockEntityContainer;

        public static void RandomizeAttributes(ITreeAttribute attributes)
        {
            attributes.SetInt(nameof(AdditionalStorageModuleProps.AdditionalSlotCount), (int)Math.Round(ModulesConfig.Instance.AdditionalStorage.AdditionalSlotCount.nextFloat()));
        }

        //TODO filter BlockEntityQuern, pulverizer, BlockEntityItemFlow (only chute not hopper) and other machines
    }
}

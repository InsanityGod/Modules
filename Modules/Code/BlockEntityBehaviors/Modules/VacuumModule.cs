using HarmonyLib;
using InsanityLib.Interfaces;
using Modules.Code.Interfaces.Modules;
using Modules.Config;
using Modules.Config.Props;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;

namespace Modules.Code.BlockEntityBehaviors.Modules
{
    public class VacuumModule : BlockEntityModuleBase , IBlockEntityBehaviorModule, IPermanentBehavior
    {
        public VacuumModuleProps Props { get; private set; }

        private readonly BlockEntityContainer container;

        private long listenerId;
        private IEnumerator<int> enumerator;

        public VacuumModule(BlockEntity blockentity) : base(blockentity)
        {
            container = blockentity as BlockEntityContainer;

        }

        public override void Initialize(ICoreAPI api, JsonObject properties)
        {
            base.Initialize(api, properties);
            Props = properties.AsObject<VacuumModuleProps>();

            if (api.Side != EnumAppSide.Server) return;

            listenerId = Blockentity.RegisterGameTickListener(Vacuum, ModulesConfig.Instance.Vacuum.DelayBetweenItterationsMs, api.World.Rand.Next(ModulesConfig.Instance.Vacuum.DelayBetweenItterationsMs));
            enumerator = CollectionEnumerator().GetEnumerator();
        }

        /// <summary>
        /// The ammount of itterations to wait for before running again (this is only set on server)
        /// </summary>
        private int ItterationSkipCount;

        private void Vacuum(float ms)
        {
            if(ItterationSkipCount-- > 0) return;
            
            enumerator.MoveNext();
            ItterationSkipCount = enumerator.Current;
        }

        private Entity[] FindNearbyItems() => Api.World.GetEntitiesAround(Pos.ToVec3d(), Props.Range, Props.Range, static entity => entity is EntityItem);
        
        private bool TryCollectItem(EntityItem itemEntity)
        {
            var weightedSlot = container.Inventory.GetBestSuitedSlot(itemEntity.Slot);
            if(weightedSlot.slot == null) return false;

            itemEntity.Slot.TryPutInto(Api.World, weightedSlot.slot, itemEntity.Slot.Itemstack.StackSize);
            if (itemEntity.Slot.Empty)
            {
                itemEntity.Die(EnumDespawnReason.PickedUp);
            }
            return true;
        }

        private IEnumerable<int> CollectionEnumerator() //TODO rename
        {
            //Find nearby items
            var entities = FindNearbyItems();
            
            do
            {
                foreach (var entity in entities) //Go over the items
                {
                    if(!entity.Alive || entity is not EntityItem itemEntity) continue; //Skip item entities that no longer exist
                    
                    //If collected item we can wait for next itteration
                    if(TryCollectItem(itemEntity)) yield return 0;
                }

                //Gone over all items so pause
                yield return ModulesConfig.Instance.Vacuum.ItterationSkipOnFinish;

                //Find nearby items again
                entities = Api.World.GetEntitiesAround(Pos.ToVec3d(), Props.Range, Props.Range, static entity => entity is EntityItem);

                //If there are no items nearby, hibernate
                if(entities.Length == 0)
                {
                    yield return ModulesConfig.Instance.Vacuum.ItterationSkipOnNoNearbyItems;
                }
            }
            while (true);
        }

        public static bool IsApplicableTo(BlockEntity blockEntity) => blockEntity is BlockEntityContainer;

        public static void RandomizeAttributes(ITreeAttribute attributes)
        {
            attributes.SetInt(nameof(VacuumModuleProps.Range), (int)Math.Round(ModulesConfig.Instance.Vacuum.Range.nextFloat()));
        }

        public void OnRuntimeRemoved()
        {
            if (Api.Side != EnumAppSide.Server) return;

            Blockentity.UnregisterGameTickListener(listenerId);
            enumerator.Dispose();
        }

        public override void OnBlockUnloaded()
        {
            if (Api.Side != EnumAppSide.Server) return;
            enumerator.Dispose();
        }
    }
}

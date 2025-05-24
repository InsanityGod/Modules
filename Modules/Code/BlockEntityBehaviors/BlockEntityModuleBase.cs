using InsanityLib.Behaviors.BlockEntityBehaviors;
using InsanityLib.Util;
using Modules.Code.CollectibleBehaviors;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace Modules.Code.BlockEntityBehaviors
{
    public abstract class BlockEntityModuleBase : BlockEntityBehavior
    {
        protected BlockEntityModuleBase(BlockEntity blockentity) : base(blockentity)
        {
        }

        public ItemStack Item { get; internal set; }

        private string id;
        public string Id => id ??= Blockentity.GetOrCreatePermanentBehaviorManager().GetId(this);

        public override void GetBlockInfo(IPlayer forPlayer, StringBuilder dsc)
        {
            if (Item != null)
            {
                dsc.AppendLine("- " + Item.GetName());
                if (Module.AppendModuleEffects(Item, dsc, false)) dsc.AppendLine();
            }
        }

        public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldAccessForResolve)
        {
            Item = tree
                .GetOrAddTreeAttribute(PermanentBehaviorManager.PermanentBehaviorTreeKey)
                .GetOrAddTreeAttribute(Id)
                .GetItemstack("item");

            Item?.ResolveBlockOrItem(worldAccessForResolve);
        }

        public override void ToTreeAttributes(ITreeAttribute tree) => tree
            .GetOrAddTreeAttribute(PermanentBehaviorManager.PermanentBehaviorTreeKey)
            .GetOrAddTreeAttribute(Id)
            .SetItemstack("item", Item);

        public override void OnBlockBroken(IPlayer byPlayer = null)
        {
            if (Item != null) Api.World.SpawnItemEntity(Item, Pos);
        }
    }
}

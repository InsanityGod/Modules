using InsanityLib.Interfaces;
using InsanityLib.Util;
using Modules.Code.Interfaces.Modules;
using Modules.Config;
using Modules.Config.Props;
using System;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;

namespace Modules.Code.BlockEntityBehaviors.Modules
{
    public class TransitionSpeedMutatorModule : BlockEntityModuleBase, IBlockEntityBehaviorModule, IPermanentBehavior
    {
        public TransitionSpeedMutatorModuleProps Props { get; private set; }

        public TransitionSpeedMutatorModule(BlockEntity blockentity) : base(blockentity)
        {
        }

        public override void Initialize(ICoreAPI api, JsonObject properties)
        {
            base.Initialize(api, properties);
            Props = properties.AsObject<TransitionSpeedMutatorModuleProps>();

            (Blockentity as BlockEntityContainer).Inventory.OnAcquireTransitionSpeed += ModifyTransitionSpeed;
        }

        private float ModifyTransitionSpeed(EnumTransitionType transType, ItemStack stack, float mulByConfig) => transType == Props.TransitionType ? Props.TransitionModifier : 1;

        public void OnRuntimeRemoved()
        {
            (Blockentity as BlockEntityContainer).Inventory.OnAcquireTransitionSpeed -= ModifyTransitionSpeed;
        }

        public static bool IsApplicableTo(BlockEntity blockEntity) => blockEntity is BlockEntityContainer;

        public static void RandomizeAttributes(ITreeAttribute attributes)
        {
            attributes.SetInt(nameof(TransitionSpeedMutatorModuleProps.TransitionType), ReflectionUtil.GetRandom<EnumTransitionType>());

            attributes.SetFloat(
                nameof(TransitionSpeedMutatorModuleProps.TransitionModifier),
                MathF.Round(
                    Random.Shared.NextSingle() > 0.5f ? ModulesConfig.Instance.TransitionSpeedMutator.Increase.nextFloat() : ModulesConfig.Instance.TransitionSpeedMutator.Reduction.nextFloat(),
                    2
                )
            );
        }
    }
}
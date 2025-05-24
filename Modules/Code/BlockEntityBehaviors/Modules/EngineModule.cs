using InsanityLib.Interfaces;
using Modules.Code.Interfaces.Modules;
using Modules.Config;
using Modules.Config.Props;
using System;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent.Mechanics;

namespace Modules.Code.BlockEntityBehaviors.Modules
{
    public class EngineModule : BlockEntityModuleBase, IBlockEntityBehaviorModule, IPermanentBehavior //TODO remove unnecesary interfaces
    {
        public EngineModuleProps Props { get; private set; }
        public EngineModule(BlockEntity blockentity) : base(blockentity)
        {
        }
        
        public override void Initialize(ICoreAPI api, JsonObject properties) //TODO see about clearing properties
        {
            base.Initialize(api, properties);
            Props = properties.AsObject<EngineModuleProps>();
            UpdateNetwork(); //TODO do this in while rebuilding network instead :P
        }

        public void UpdateNetwork() //TODO do this in while rebuilding network instead :P
        {
            if(Api?.Side != EnumAppSide.Server) return;

            var consumer = Blockentity.GetBehavior<BEBehaviorMPBase>();
            if(consumer.Network == null)
            {
                var network = Api.ModLoader.GetModSystem<MechanicalPowerMod>().CreateNetwork(consumer);
                consumer.JoinNetwork(network);
                if(Block is BlockMPBase mechanicalBlock)
                {
                    foreach(var facing in BlockFacing.ALLFACES)
                    {
                        if(mechanicalBlock.HasMechPowerConnectorAt(Api.World, Pos, facing))
                        {
                            consumer.CreateJoinAndDiscoverNetwork(facing); 
                        }
                    }
                }
                Blockentity.MarkDirty(true);
            }
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            UpdateNetwork(); //TODO do this in while rebuilding network instead :P
            base.ToTreeAttributes(tree);
        }

        protected double capableSpeed;

        internal float GetTorque(float speed, ref float resistance)
        {
            capableSpeed += (Props.TargetSpeed - capableSpeed) * Props.AccelerationFactor;

            float csFloat = (float)capableSpeed;
            float absSpeed = Math.Abs(speed);
            float excessSpeed = absSpeed - csFloat;
            
            if(excessSpeed > 0f)
            {
                resistance += 0.1f * Math.Min(1.2f, 1 + excessSpeed * excessSpeed * 80f);
            }
			
            float power = csFloat - absSpeed;
			return Math.Max(0f, power) * Props.TorqueFactor;
        }

        public static bool IsApplicableTo(BlockEntity blockEntity) => blockEntity.GetBehavior<BEBehaviorMPBase>() != null;

        public static void RandomizeAttributes(ITreeAttribute attributes)
        {
            attributes.SetFloat(nameof(EngineModuleProps.TorqueFactor), MathF.Round(ModulesConfig.Instance.Engine.TorqueFactor.nextFloat(), 2));
            attributes.SetFloat(nameof(EngineModuleProps.TargetSpeed), MathF.Round(ModulesConfig.Instance.Engine.TargetSpeed.nextFloat(), 2));
            attributes.SetFloat(nameof(EngineModuleProps.AccelerationFactor), MathF.Round(ModulesConfig.Instance.Engine.AccelerationFactor.nextFloat(), 2));
        }
    }
}

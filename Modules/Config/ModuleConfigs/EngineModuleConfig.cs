using Vintagestory.API.MathTools;

namespace Modules.Config.ModuleConfigs
{
    public class EngineModuleConfig : ModuleConfigBase
    {
        public NatFloat TorqueFactor { get; set; } = new NatFloat(1, 1, EnumDistribution.GAUSSIAN)
        {
            offset = 0.5f
        };

        public NatFloat TargetSpeed { get; set; } = new NatFloat(1, 1, EnumDistribution.GAUSSIAN)
        {
            offset = 0.5f
        };

        public NatFloat AccelerationFactor { get; set; } = new NatFloat(1, 1, EnumDistribution.GAUSSIAN)
        {
            offset = 0.5f
        };

        //TODO very hard to get legendary module (a lot of slots)
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.MathTools;

namespace Modules.Config.ModuleConfigs
{
    public class TransitionSpeedMutatorModuleConfig : ModuleConfigBase
    {

        /// <summary>
        /// This decides the modifier if the module reduces the speed
        /// </summary>
        public NatFloat Reduction { get; set; } = new NatFloat(0.5f, 0.4f, EnumDistribution.UNIFORM);

        /// <summary>
        /// This decides the modifier if the module increased the speed
        /// </summary>
        public NatFloat Increase { get; set; } = new NatFloat(1.5f, 0.4f, EnumDistribution.UNIFORM);

        //TODO very hard to get legendary module (instant transition / complete stop of transition)
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.MathTools;

namespace Modules.Config.ModuleConfigs
{
    public class AdditionalStorageModuleConfig : ModuleConfigBase
    {

        /// <summary>
        /// This decides how many slots the module will add
        /// Note: the number is rounded to the nearest full number
        /// </summary>
        public NatFloat AdditionalSlotCount { get; set; } = new NatFloat(6, 6, EnumDistribution.UNIFORM)
        {
            offset = 2,
        };

        //TODO very hard to get legendary module (a lot of slots)
    }
}

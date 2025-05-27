using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.MathTools;

namespace Modules.Config.ModuleConfigs
{
    public class VacuumModuleConfig : ModuleConfigBase
    {
        /// <summary>
        /// This decides how large the range of the vacuum module is
        /// Note: the number is rounded to the nearest full number
        /// </summary>
        public NatFloat Range { get; set; } = new NatFloat(4, 4, EnumDistribution.UNIFORM)
        {
            offset = 1,
        };

        /// <summary>
        /// How many ms should between each attempt to pick up an item
        /// </summary>
        [DefaultValue(100)]
        public int DelayBetweenItterationsMs { get; set; } = 100;

        /// <summary>
        /// How many itterations should be skipped when the vacuum went through all nearby items
        /// (This is essentially a kind of sleep mode that happens when the vacuum module has gone over all nearby items or could not pick up any items)
        /// </summary>
        [DefaultValue(100)]
        public int ItterationSkipOnFinish { get; set; } = 100;

        /// <summary>
        /// How many itterations should be skipped when there are no nearby items to pick up
        /// (this is essentially a kind of sleep mode that happens when there are no items nearby to pickup)
        /// </summary>
        [DefaultValue(300)]
        public int ItterationSkipOnNoNearbyItems { get; set; } = 300;

    }
}

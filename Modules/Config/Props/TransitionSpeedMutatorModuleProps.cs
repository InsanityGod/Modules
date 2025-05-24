using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;

namespace Modules.Config.Props
{
    public class TransitionSpeedMutatorModuleProps
    {
        public EnumTransitionType TransitionType { get; set; }

        public float TransitionModifier { get; set; }
    }
}

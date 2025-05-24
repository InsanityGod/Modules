using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Config.Props
{
    public class EngineModuleProps
    {
        public float TorqueFactor { get; set; } = 1;

        public float TargetSpeed { get; set; } = 1;
        public float AccelerationFactor { get; set; } = 0.5f;
    }
}

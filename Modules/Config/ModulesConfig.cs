using InsanityLib.Attributes.Auto.Config;
using Modules.Config.ModuleConfigs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.MathTools;

namespace Modules.Config
{
    public class ModulesConfig
    {
        [AutoConfig("ModulesConfig.json", ServerSync = true)]
        public static ModulesConfig Instance { get; private set; }

        /// <summary>
        /// The configuration for the Transition Speed Mutator Module
        /// </summary>
        public TransitionSpeedMutatorModuleConfig TransitionSpeedMutator { get; set; } = new();

        /// <summary>
        /// The configuration for the Additional Storage Module
        /// </summary>
        public AdditionalStorageModuleConfig AdditionalStorage { get; set; } = new();

        /// <summary>
        /// The configuration for the Engine Module
        /// </summary>
        public EngineModuleConfig Engine { get; set; } = new();
    }
}

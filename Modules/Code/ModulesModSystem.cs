using InsanityLib.Attributes.Auto;
using Modules.Code.BlockEntityBehaviors.Modules;
using System;
using System.Collections.Generic;
using Vintagestory.API.Common;

[assembly: AutoRegistry("modules")]
[assembly: AutoPatcher("modules")]

namespace Modules.Code
{
    public class ModulesModSystem : ModSystem
    {
        public static readonly Dictionary<string, Type> ModuleTypeMapping = new()
        {
            {nameof(AdditionalStorageModule), typeof(AdditionalStorageModule)},
            {nameof(TransitionSpeedMutatorModule), typeof(TransitionSpeedMutatorModule)},
            {nameof(EngineModule), typeof(EngineModule)},
        }; //TODO
    }
}
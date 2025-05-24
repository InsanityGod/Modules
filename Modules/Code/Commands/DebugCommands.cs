using InsanityLib.Attributes.Auto.Command;
using InsanityLib.Enums.Auto.Commands;
using InsanityLib.Util;
using Modules.Code.BlockEntityBehaviors.Modules;
using Modules.Code.Interfaces.Modules;
using Modules.Config.Props;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;

namespace Modules.Code.Commands
{
    public static class DebugCommands
    {
        [AutoCommand(Path = "module/debug", Name = "additionalstorage")]
        public static bool AddExtraStorage([CommandParameter(Source = EParamSource.CallerTarget)] [Required(ErrorMessage = "Not Applicable to target block")] BlockEntityContainer blockEntityContainer, [CommandParameter(Source = EParamSource.Specify)] int amount)
        {
            if(!AdditionalStorageModule.IsApplicableTo(blockEntityContainer)) return false;
            
            var module = blockEntityContainer.TryAddPermanentbehavior("modules:AdditionalStorageModule", new JsonObject(JToken.FromObject(new AdditionalStorageModuleProps
            {
                AdditionalSlotCount = amount
            })));

            return module != null;
        }
    }
}

using HarmonyLib;
using Modules.Code.BlockEntityBehaviors.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.GameContent.Mechanics;

namespace Modules.Code.HarmonyPatches
{
    [HarmonyPatch]
    public static class EngineModulePatches
    {
        [HarmonyTargetMethods]
        public static IEnumerable<MethodBase> TargetMethods()
        {

            yield return AccessTools.Method(typeof(BEBehaviorMPBase), nameof(BEBehaviorMPBase.GetTorque));
            yield return AccessTools.Method(typeof(BEBehaviorMPRotor), nameof(BEBehaviorMPBase.GetTorque));
        }

        [HarmonyPostfix]
        public static void ApplyEngineModuleEffect(BEBehaviorMPBase __instance, ref float __result, float speed, ref float resistance)
        {
            var engine = __instance.Blockentity.GetBehavior<EngineModule>();
            if(engine == null) return;
            __result += engine.GetTorque(speed, ref resistance);
        }
    }
}

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Util;

namespace Modules.Code.HarmonyPatches
{
    [HarmonyPatch]
    public static class AdditionalStorageModulePatches
    {
        [HarmonyPatch(typeof(InventoryBase), nameof(InventoryBase.SlotsFromTreeAttributes))]
        [HarmonyPrefix]
        public static void AutoIncreaseSize(InventoryBase __instance, ITreeAttribute tree, ref ItemSlot[] slots)
        {
            if (slots == null) return;

            var slotCount = tree.GetInt("qslots", 0);
            if (slots.Length >= slotCount) return;

            slots = slots.Append(__instance.GenEmptySlots(slotCount - slots.Length));
        }
    }
}

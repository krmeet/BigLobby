using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using GameNetcodeStuff;
using HarmonyLib;
using BepInEx;
using UnityEngine;
namespace BigLobby.Patches
{
    [HarmonyPatch]
    public class ListSizeTranspilers {
        [HarmonyPatch(typeof(HUDManager), "SyncAllPlayerLevelsServerRpc")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> SyncLevelsRpc(IEnumerable<CodeInstruction> instructions) {
            var codes = new List<CodeInstruction>(instructions);
            
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Newarr)
                {
                    if (codes[i - 1].opcode == OpCodes.Ldc_I4_4) {
                        codes[i - 1].opcode = OpCodes.Ldc_I4_S;
                        codes[i - 1].operand = Plugin.MaxPlayers;
                    }
                }
            }
            return codes.AsEnumerable();
        }
        /*[HarmonyPatch(typeof(StartOfRound), "SyncShipUnlockablesServerRpc")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> SyncUnlockablesRpc(IEnumerable<CodeInstruction> instructions) {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Newarr)
                {
                    if (codes[i - 1].opcode == OpCodes.Ldc_I4_4)
                    {
                        codes[i - 1].opcode = OpCodes.Ldc_I4_S;
                        codes[i - 1].operand = Plugin.MaxPlayers;
                        break;
                    }
                }
            }
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Blt_S)
                {
                    if (codes[i - 1].opcode == OpCodes.Ldc_I4_4)
                    {
                        codes[i - 1].opcode = OpCodes.Ldc_I4_S;
                        codes[i - 1].operand = Plugin.MaxPlayers;
                        break;
                    }
                }
            }
            return codes.AsEnumerable();
        }*/
        [HarmonyPatch(typeof(PlayerControllerB), "SendNewPlayerValuesServerRpc")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> SendPlayerValuesRpc(IEnumerable<CodeInstruction> instructions) {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Blt_S)
                {
                    if (codes[i - 1].opcode == OpCodes.Ldc_I4_4)
                    {
                        codes[i - 1].opcode = OpCodes.Ldc_I4_S;
                        codes[i - 1].operand = Plugin.MaxPlayers;
                        break;
                    }
                }
            }
            return codes.AsEnumerable();
        }
        [HarmonyPatch(typeof(PlayerControllerB), "SendNewPlayerValuesServerRpc")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> SendNewPlayerValuesServerRpc(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Blt_S)
                {
                    if (codes[i - 1].opcode == OpCodes.Ldc_I4_4)
                    {
                        codes[i - 1].opcode = OpCodes.Ldc_I4_S;
                        codes[i - 1].operand = Plugin.MaxPlayers;
                        break;
                    }
                }
            }
            return codes.AsEnumerable();
        }
        [HarmonyPatch(typeof(HUDManager), "FillEndGameStats")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> FillEndGameStats(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Blt)
                {
                    codes[i].opcode = OpCodes.Bgt;
                    break;
                }
            }
            return codes.AsEnumerable();
        }

        [HarmonyPatch(typeof(PlayerControllerB), "SpectateNextPlayer")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> SpectateNextPlayer(IEnumerable<CodeInstruction> instructions) {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_I4_4)
                {
                    codes[i].opcode = OpCodes.Ldc_I4_S;
                        codes[i].operand = Plugin.MaxPlayers;
                        break;
                }
            }
            return codes.AsEnumerable();
        }
    }
}

using GameNetcodeStuff;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;
using System.Reflection;
using System;
using System.Collections.Generic;
using Netcode.Transports.Facepunch;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine.Audio;
using Steamworks.Ugc;
using UnityEngine.Assertions;

namespace BigLobby.Patches
{
    [HarmonyPatch]
    internal class PlayerObjects
    {
        [HarmonyPatch(typeof(StartOfRound), "Awake")]
        [HarmonyPostfix]
        public static void ResizeLists(ref StartOfRound __instance)
        {
            __instance.allPlayerObjects = Helper.ResizeArray(__instance.allPlayerObjects, Plugin.MaxPlayers);
            __instance.allPlayerScripts = Helper.ResizeArray(__instance.allPlayerScripts, Plugin.MaxPlayers);
            __instance.gameStats.allPlayerStats = Helper.ResizeArray(__instance.gameStats.allPlayerStats, Plugin.MaxPlayers);
            for (int j = 4; j < Plugin.MaxPlayers; j++)
            {
                __instance.gameStats.allPlayerStats[j] = new PlayerStats();
            }
            Debug.Log(__instance.playerSpawnPositions.Length);
            Debug.Log("Yeahg");
            Debug.Log(__instance.allPlayerScripts.Length);
        }
        [HarmonyPatch(typeof(SoundManager), "Awake")]
        [HarmonyPostfix]
        public static void SoundWake(ref SoundManager __instance)
        {
            __instance.playerVoiceMixers = Helper.ResizeArray(__instance.playerVoiceMixers, Plugin.MaxPlayers);
            for (int j = 4; j < Plugin.MaxPlayers; j++)
            {
                __instance.playerVoiceMixers[j] = UnityEngine.Object.Instantiate(__instance.playerVoiceMixers[0]);
                //__instance.playerVoiceMixers[j].
            }
        }
        [HarmonyPatch(typeof(SoundManager), "Start")]
        [HarmonyPostfix]
        public static void ResizeSoundManagerLists(ref SoundManager __instance)
        {
            __instance. playerVoicePitchLerpSpeed = new float[Plugin.MaxPlayers + 1];
            __instance.playerVoicePitchTargets = new float[Plugin.MaxPlayers + 1];
            __instance.playerVoicePitches = new float[Plugin.MaxPlayers+1];
            for (int i = 1; i < Plugin.MaxPlayers+1; i++)
            {
                __instance.playerVoicePitchLerpSpeed[i] = 3f;
                __instance.playerVoicePitchTargets[i] = 1f;
                __instance.playerVoicePitches[i] = 1f;
            }
        }
        private static StartOfRound startOfRound;
        private static bool instantiating = false;
        private static int nextClientId = 0;
        private static PlayerControllerB referencePlayer;
        [HarmonyPatch(typeof(StartOfRound), "Start")]
        [HarmonyPrefix]
        public static void AddPlayers(ref StartOfRound __instance)
        {
            startOfRound = __instance;
            referencePlayer = __instance.allPlayerObjects[0].GetComponent<PlayerControllerB>();
            var playerPrefab = __instance.playerPrefab;
            var playerContainer = __instance.allPlayerObjects[0].transform.parent;
            var spawnMethod = typeof(NetworkSpawnManager).GetMethod(
                "SpawnNetworkObjectLocally",
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                CallingConventions.Any,
                new Type[] { typeof(NetworkObject), typeof(ulong), typeof(bool), typeof(bool), typeof(ulong), typeof(bool) },
                null
            );
                Plugin.instantiating = true;
            for (int i = 4; i < Plugin.MaxPlayers; i++)
            {
                nextClientId = i;
                var newPlayer = GameObject.Instantiate<GameObject>(playerPrefab, playerContainer);
                var newScript = newPlayer.GetComponent<PlayerControllerB>();
                var netObject = newPlayer.GetComponent<NetworkObject>();
                Debug.Log(netObject.OwnerClientId);
                Debug.Log("[BigLobby] Trying to spawn new player");
                __instance.allPlayerObjects[i] = newPlayer;
                __instance.allPlayerScripts[i] = newScript;
                (typeof(NetworkObject)).GetProperty("NetworkObjectId", BindingFlags.Instance | BindingFlags.Public).SetValue(netObject, (uint)1234567890ul + (ulong)i);
                //Plugin.dothethe(newPlayer);
                spawnMethod.Invoke(NetworkManager.Singleton.SpawnManager, new object[]{
                        netObject,
                        1234567890ul + (ulong)i,
                        true,//this needs to  be true or everything fucking shits itself. i think this might be the problem aswell. partciularly weird cuz apparently theres nested netobjs but i dont see any?
                        true,
                        netObject.OwnerClientId,
                        false
                    });
            }
            Plugin.instantiating = false;
        }
        [HarmonyPatch(typeof(PlayerControllerB), "Update")]
        [HarmonyPrefix]

        public static bool ShitAssFix(ref PlayerControllerB __instance)
        {
            if (__instance.transform.parent.gameObject.name == "HangarShip") {
                __instance.isPlayerControlled = true;
            }
            return (true);
        }//Bizzlemip rolls worlds shittiest PATCH. Asked to leave MODDING COMMUNITY

        /*[HarmonyPatch(typeof(HUDManager), "FillEndGameStats")]
        [HarmonyPrefix]
        public static bool ShitAssFix2ElecticBoogaloo(ref HUDManager __instance, EndOfGameStats stats)
        {
            return (false);
        }//Bizzlemip rolls worlds shittiest PATCH. Asked to leave MODDING COMMUNITY*/
        [HarmonyPatch(typeof(PlayerControllerB), "Awake")]
        [HarmonyPrefix]

        public static bool FixPlayerObject(ref PlayerControllerB __instance)
        {
            if (!Plugin.instantiating) return(true);
            __instance.gameObject.name = $"ExtraPlayer{nextClientId}";
            __instance.playerClientId = (ulong)nextClientId;
            __instance.actualClientId = (ulong)nextClientId;

            StartOfRound.Instance.allPlayerObjects[nextClientId] = __instance.transform.parent.gameObject;
            StartOfRound.Instance.allPlayerScripts[nextClientId] = __instance;
            var fields = typeof(PlayerControllerB).GetFields();
            foreach (FieldInfo field in fields) 
            {
                var myValue = field.GetValue(__instance);
                var referenceValue = field.GetValue(referencePlayer);
                if (myValue == null && referenceValue != null)
                    field.SetValue(__instance, referenceValue);
            }
            __instance.enabled = true;
            return (true);
        }

        [HarmonyPatch(typeof(StartOfRound), "GetPlayerSpawnPosition")]
        [HarmonyTranspiler]

        public static IEnumerable<CodeInstruction> GetPlayerSpawnPosition(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            codes[0].opcode = OpCodes.Ldc_I4_1;
            return codes.AsEnumerable();
        }
    }
}

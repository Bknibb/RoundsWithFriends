using HarmonyLib;
using Landfall.Network;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnboundLib;
using UnityEngine;

namespace RWF.Patches
{
    [HarmonyPatch(typeof(ClientSteamLobby), "ShowInviteScreenWhenConnected")]
    class ClientSteamLobby_Patch_ShowInviteScreenWhenConnected
    {
        static bool Prefix(ClientSteamLobby __instance) {
            // Allow inviting multiple times in the same room
            if (__instance.CurrentLobby != CSteamID.Nil) {
                SteamFriends.ActivateGameOverlayInviteDialog(__instance.CurrentLobby);
                return false;
            }

            return true;
        }
    }
    [HarmonyPatch(typeof(ClientSteamLobby), "OnLobbyEnter")]
    class ClientSteamLobby_Patch_OnLobbyEnter
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            List<CodeInstruction> list = instructions.ToList();

            LocalBuilder roomCodeLocal = il.DeclareLocal(typeof(string));
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].LoadsField(AccessTools.Field(typeof(NetworkConnectionHandler), "instance")) && list[i + 5].Calls(AccessTools.Method(typeof(NetworkConnectionHandler), "ForceRegionJoin")))
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Ldstr, "RoomCode");
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SteamMatchmaking), "GetLobbyData"));
                    yield return new CodeInstruction(OpCodes.Stloc, roomCodeLocal.LocalIndex);
                }
                yield return list[i];
                if (list[i].opcode == OpCodes.Ldloc_1 && list[i+4].Calls(AccessTools.Method(typeof(NetworkConnectionHandler), "ForceRegionJoin"))) {
                    yield return new CodeInstruction(OpCodes.Ldloc, roomCodeLocal.LocalIndex);
                    i += 3;
                }
            }
        }
    }
}

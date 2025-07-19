using System.Reflection;
using System.Collections;
using System;
using Photon.Realtime;
using Landfall.Network;
using UnboundLib;
using HarmonyLib;

namespace RWF
{
    public static class NetworkConnectionHandlerExtensions
    {
        public static void SetSearchingQuickMatch(this NetworkConnectionHandler instance, bool value) {
            instance.SetFieldValue("m_searchingType", value ? 1 : 0);
        }

        public static bool IsSearchingQuickMatch(this NetworkConnectionHandler instance) {
            return (int)instance.GetFieldValue("m_searchingType") == 1;
        }

        public static void SetSearchingTwitch(this NetworkConnectionHandler instance, bool value) {
            instance.SetFieldValue("m_searchingType", value ? 4 : 0);
        }

        public static bool IsSearchingTwitch(this NetworkConnectionHandler instance) {
            return (int) instance.GetFieldValue("m_searchingType") == 4;
        }

        public static void SetForceRegion(this NetworkConnectionHandler instance, bool value) {
            instance.SetFieldValue("m_ForceRegion", value);
        }

        public static void HostPrivate(this NetworkConnectionHandler instance) {
            instance.SetSearchingQuickMatch(false);
            instance.SetSearchingTwitch(false);

            TimeHandler.instance.gameStartTime = 1f;
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = (byte) RWFMod.instance.MaxPlayers;
            options.IsOpen = true;
            options.IsVisible = false;

            Action createRoomFn = () => instance.InvokeMethod("CreateRoom", options);
            instance.StartCoroutine((IEnumerator) instance.InvokeMethod("DoActionWhenConnected", createRoomFn));
        }
    }
}

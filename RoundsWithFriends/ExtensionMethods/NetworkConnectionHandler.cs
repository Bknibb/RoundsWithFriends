using HarmonyLib;
using Landfall.Network;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Reflection;
using UnboundLib;

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
            options.CustomRoomPropertiesForLobby = new string[] { NetworkConnectionHandler.ROOM_CODE };
		    
            Action createRoomFn = () =>
            {
                string text = string.Empty;
                text += NetworkConnectionHandler.RegionToCode(PhotonNetwork.CloudRegion).ToString();
                text += instance.InvokeMethod("CreateRandomName", new object[] { 5, true} );
                options.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() {
                    {
                        NetworkConnectionHandler.ROOM_CODE,
                        text
                    }
                };
                TypedLobby LOBBY_ROOMCODE = (TypedLobby)ExtensionMethods.GetStaticFieldValue(typeof(NetworkConnectionHandler), "LOBBY_ROOMCODE");
                PhotonNetwork.CreateRoom(text, options, LOBBY_ROOMCODE, null);
                LoadingScreen.instance.SetRoomCode(text);
            };
            instance.StartCoroutine((IEnumerator) instance.InvokeMethod("DoActionWhenConnected", createRoomFn));
        }
    }
}

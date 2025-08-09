using HarmonyLib;
using Landfall.Network;
using Photon.Pun;
using Photon.Realtime;
using Steamworks;
using System;
using System.Collections;
using System.Reflection;
using UnboundLib;
using UnityEngine;

namespace RWF
{
    public static class NetworkConnectionHandlerExtensions
    {
        public enum SearchingType
        {
            // Token: 0x04000EF4 RID: 3828
            None,
            // Token: 0x04000EF5 RID: 3829
            Quickmatch,
            // Token: 0x04000EF6 RID: 3830
            HostRoom,
            // Token: 0x04000EF7 RID: 3831
            FriendInvite,
            // Token: 0x04000EF8 RID: 3832
            Twitch
        }
        public static SearchingType GetSearchingType(this NetworkConnectionHandler instance) {
            return (SearchingType)instance.GetFieldValue("m_searchingType");
        }
        public static void SetSearchingType(this NetworkConnectionHandler instance, SearchingType searchingType)
        {
            instance.SetFieldValue("m_searchingType", (int) searchingType);
        }

        public static bool IsSearchingTwitch(this NetworkConnectionHandler instance) {
            return (int) instance.GetFieldValue("m_searchingType") == 4;
        }

        public static void SetForceRegion(this NetworkConnectionHandler instance, bool value) {
            instance.SetFieldValue("m_ForceRegion", value);
        }

        public static void HostPrivate(this NetworkConnectionHandler instance) {
            instance.SetSearchingType(SearchingType.HostRoom);

            TimeHandler.instance.gameStartTime = 1f;
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = (byte) RWFMod.instance.MaxPlayers;
            options.IsOpen = true;
            options.IsVisible = true;
            options.CustomRoomPropertiesForLobby = new string[] { NetworkConnectionHandler.ROOM_CODE };
		    
            Action createRoomFn = () =>
            {
                ClientSteamLobby lobby = (ClientSteamLobby) ExtensionMethods.GetStaticFieldValue(typeof(NetworkConnectionHandler), "m_SteamLobby");
                lobby.CreateLobby(
                    RWFMod.instance.MaxPlayers,
                    (roomName) =>
                    {
                        string text = string.Empty;
                        text += NetworkConnectionHandler.RegionToCode(PhotonNetwork.CloudRegion).ToString();
                        text += (string) instance.InvokeMethod("CreateRandomName", new object[] { 5, true });
                        options.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() {
                            {
                                NetworkConnectionHandler.ROOM_CODE,
                                text
                            }
                        };
                        TypedLobby LOBBY_ROOMCODE = (TypedLobby) ExtensionMethods.GetStaticFieldValue(typeof(NetworkConnectionHandler), "LOBBY_ROOMCODE");
                        PhotonNetwork.CreateRoom(text, options, LOBBY_ROOMCODE, null);
                        SteamMatchmaking.SetLobbyData(lobby.CurrentLobby, "RoomCode", text);
                        LoadingScreen.instance.SetRoomCode(text);
                    }
                );
                
            };
            instance.StartCoroutine((IEnumerator) instance.InvokeMethod("DoActionWhenConnected", createRoomFn));
        }
    }
}

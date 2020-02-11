using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;

using Hashtable = ExitGames.Client.Photon.Hashtable;
using ExitGames.Client.Photon;

public class NetworkController : MonoBehaviourPunCallbacks {

    public byte playerRoomMax = 2;
    public Lobby lobbyScript;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    #region --- PUN CALLBACKS ---

    public override void OnConnected()
    {
        print("On connected to server.");
    }
    public override void OnConnectedToMaster()
    {
        print("Connection validated with server.");

        lobbyScript.ShowPanelLobby();

        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        print("Joinned a room.");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print("Failled in joinning a room. Creating one...");

        string roomName = "room" + Random.Range(100, 10000);

        RoomOptions roomOptions = new RoomOptions() { MaxPlayers = playerRoomMax };
        PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        print("Novo jogador entrou: " + newPlayer.NickName);

        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            print("Número máximo de jogadores alcançado. Iniciando partida...");

            Hashtable props = new Hashtable()
            {
                { CountdownTimer.CountdownStartTime, (float) PhotonNetwork.Time }
            };

            PhotonNetwork.CurrentRoom.SetCustomProperties(props);

            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(CountdownTimer.CountdownStartTime))
        {
            lobbyScript.textCountdown.gameObject.SetActive(true);
        }
    }

    #endregion
}

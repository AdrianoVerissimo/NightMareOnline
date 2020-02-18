using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Lobby : MonoBehaviourPunCallbacks
{
    public PhotonView photonView;

    public GameObject panelLogin;
    public GameObject panelLobby;
    public InputField inputName;
    public Text textConnecting;
    public Text textCountdown;

    public Text textNumberPlayersRoom;
    public Text textPleaseWait;
    public GameObject buttonStartMatch;

    private string prefixNumberPlayersRoom;

    // Use this for initialization
    void Start()
    {
        string playerName;

        if (!string.IsNullOrWhiteSpace(PhotonNetwork.NickName))
            playerName = PhotonNetwork.NickName;
        else
            playerName = "Player" + Random.Range(100, 10000);

        inputName.text = playerName;

        prefixNumberPlayersRoom = textNumberPlayersRoom.text;
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region --- PUN CALLBACKS ---

    public override void OnEnable()
    {
        CountdownTimer.OnCountdownTimerHasExpired += OnCountdownTimeIsExpired;
    }

    public override void OnDisable()
    {
        CountdownTimer.OnCountdownTimerHasExpired -= OnCountdownTimeIsExpired;
    }


    #endregion

    public void OnCountdownTimeIsExpired()
    {
        StartGame();
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel("Level 01");
    }


    public void Login()
    {
        textConnecting.gameObject.SetActive(true);

        PhotonNetwork.NickName = inputName.text;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();

    }

    public void ShowPanelLogin()
    {
        panelLogin.SetActive(true);
        panelLobby.SetActive(false);
    }
    public void ShowPanelLobby()
    {
        panelLogin.SetActive(false);
        panelLobby.SetActive(true);

        bool isMasterClient = PhotonNetwork.IsMasterClient;

        UpdateShowStartButton();
    }
    public void UpdateShowStartButton()
    {
        Room currentRoom = PhotonNetwork.CurrentRoom;
        if (currentRoom != null)
        {
            if (currentRoom.PlayerCount >= 2 && PhotonNetwork.IsMasterClient)
            {
                buttonStartMatch.SetActive(true);
                textPleaseWait.gameObject.SetActive(false);
                return;
            }

            buttonStartMatch.SetActive(false);
            textPleaseWait.gameObject.SetActive(true);
        }
    }
}

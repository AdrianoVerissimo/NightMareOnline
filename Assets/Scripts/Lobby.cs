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
    public GameObject panelLogin;
    public GameObject panelLobby;
    public InputField inputName;
    public Text textConnecting;
    public Text textCountdown;
    

    // Use this for initialization
    void Start()
    {
        string playerName = "Player" + Random.Range(100, 10000);

        inputName.text = playerName;
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
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;

using Photon.Pun.UtilityScripts;
using System.Linq;
using UnityEngine.SceneManagement;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameControllerGamePlay : MonoBehaviourPunCallbacks
{
    public GameObject myPlayer;
    public Transform[] spawnPlayer;

    public GameObject canvasCountdown;

    public GameObject canvasGameOver;
    public GameObject canvasGameOverFinish;
    public GameObject canvasGameOverPlayScore;

    private bool isGameOver = false;


    // Use this for initialization
    void Start()
    {
        isGameOver = false;

        int i = Random.Range(0, spawnPlayer.Length);

        PhotonNetwork.Instantiate(myPlayer.name, spawnPlayer[i].position, spawnPlayer[i].rotation);

        CheckPlayers();

        if (canvasCountdown && !canvasCountdown.activeInHierarchy)
        {
            canvasCountdown.SetActive(true);
        }
    }


    // Update is called once per frame
    void Update()
    {

    }

    private void CheckPlayers()
    {
        if (isGameOver)
            return;

        if (PhotonNetwork.PlayerList.Length < 2)
        {
            GameOver();
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        print(otherPlayer.NickName + " left the room!!");
        CheckPlayers();
    }

    public void GameOver()
    {
        if (isGameOver)
            return;

        isGameOver = true;

        canvasGameOver.SetActive(true);

        var dictionary = new Dictionary<string, int>();
        foreach (var item in PhotonNetwork.PlayerList)
        {
            dictionary.Add(item.NickName, item.GetScore());
        }

        //ordena os valores do Dictionary de acordo com os filtros estabelecidos na expressão em Linq
        var items = from pair in dictionary
                    orderby pair.Value descending
                    select pair;

        foreach (var item in items)
        {
            GameObject playerScoreTemp = Instantiate(canvasGameOverPlayScore);
            playerScoreTemp.transform.position = Vector3.zero;
            playerScoreTemp.transform.SetParent(canvasGameOverFinish.transform);
            playerScoreTemp.GetComponent<PlayerScore>().SetData(item.Key, item.Value.ToString());
        }

        Hashtable props = new Hashtable
                    {
                        {"isGameOver", true}
                    };

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        foreach (var item in PhotonNetwork.PlayerList)
        {
            item.SetScore(0);
        }

        canvasCountdown.SetActive(false);
    }

    public void ButtonDisconnect()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene("Lobby");
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
    }
}

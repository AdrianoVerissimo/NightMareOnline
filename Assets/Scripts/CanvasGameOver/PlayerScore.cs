using UnityEngine;
using System.Collections;

using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class PlayerScore : MonoBehaviourPunCallbacks
{
    public Text nicknameText;
    public Text scoreText;

    private PhotonView photonView;

    // Use this for initialization
    void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    public void SetData(string nickname, string score)
    {
        nicknameText.text = nickname;
        scoreText.text = score;
    }
}

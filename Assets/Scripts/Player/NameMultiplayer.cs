using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Photon.Pun;


public class NameMultiplayer : MonoBehaviourPunCallbacks
{
    public Text playerNameText;
    public GameObject objCanvas;

    private PhotonView photonView;


    // Use this for initialization
    void Start()
    {
        photonView = GetComponent<PhotonView>();

        playerNameText.text = photonView.Owner.NickName;
    }

    // Update is called once per frame
    void Update()
    {
        playerNameText.transform.LookAt(Camera.main.transform);
    }
}

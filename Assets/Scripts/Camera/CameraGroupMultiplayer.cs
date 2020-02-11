using UnityEngine;
using System.Collections;

using Cinemachine;

public class CameraGroupMultiplayer : MonoBehaviour
{

    public GameObject mainCamera;
    public CinemachineVirtualCamera cinemachineVirtualCamera;


    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetFollowObject(Transform transformFollow)
    {
        cinemachineVirtualCamera.Follow = transformFollow;
    }
}

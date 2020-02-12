using UnityEngine;
using System.Collections;

using UnityEngine.UI;
using Photon.Pun;

namespace CompleteProject
{
    public class RespawnCountdown : MonoBehaviour
    {

        float countdown = 5f;
        float startTime;

        public string respawnMSG = "Respawn in";
        public Text respawnText;

        private Coroutine waitForDestroyCoroutine;

        // Use this for initialization
        void Start()
        {
            startTime = (float)PhotonNetwork.Time;
            waitForDestroyCoroutine = StartCoroutine(WaitForDestroy(countdown));
        }

        // Update is called once per frame
        void Update()
        {
            if (GameControllerGamePlay.Instance.GetIsGameOver())
            {
                if (waitForDestroyCoroutine != null)
                    StopCoroutine(waitForDestroyCoroutine);
                Destroy(gameObject);
                return;
            }

            float timer = (float)PhotonNetwork.Time - startTime;
            float countdownTemp = countdown - timer;

            string seconds = (countdownTemp % 60f).ToString("0");

            if (countdownTemp < 0.0f)
            {
                return;
            }

            respawnText.text = respawnMSG + "\n" + seconds;

        }

        IEnumerator WaitForDestroy(float time)
        {
            yield return new WaitForSeconds(time);

            Destroy(gameObject);
        }
    }
}
using UnityEngine;
using UnityEngine.UI; //necessário para utilizar componentes da UI
using System.Collections;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using ExitGames.Client.Photon;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerHealth : MonoBehaviourPunCallbacks
{
    public int startingHealth = 100; //energia inicial
    public int currentHealth; //energia atual
    public Slider healthSlider; //referência para o slider da energia
    public Image damageImage; //referência para imagem mostrada ao levar dano
    public AudioClip deathClip; //audio tocado quando o jogador morre
    public float flashSpeed = 5f; //velocidade em que a imagem de dano aparecerá
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f); //cor da imagem de dano


    Animator anim; //referência para componente de animação
    AudioSource playerAudio; //referência para o componente Audio Source
    PlayerMovement playerMovement; //referência para o script playerMovement
    PlayerShooting playerShooting; //referência para o script de tiro
    bool isDead; //diz se o jogador está morto
    bool damaged; //diz se o jogadro levou dano


    // ### Multiplayer
    private PhotonView photonView;
    public ParticleSystem hitParticles;
    public Canvas canvasHUD;
    public Text playerScoreText;
    public RuntimeAnimatorController animatorController;
    public AudioClip hurtClip;

    public GameObject canvasRespawn;

    public SkinnedMeshRenderer playerRenderer;
    public SkinnedMeshRenderer gunRenderer;
    public GameObject canvasGameplay;

    void Awake ()
    {
        anim = GetComponent <Animator> ();
        playerAudio = GetComponent <AudioSource> ();
        playerMovement = GetComponent <PlayerMovement> (); //pega referência ao script atrelado ao objeto do jogador como componente
        playerShooting = GetComponentInChildren <PlayerShooting> (); //pega referência do script de tiro
        currentHealth = startingHealth;

        photonView = GetComponent<PhotonView>();

        if (!GameControllerGamePlay.Instance.GetIsOffline())
        {
            if (!photonView.IsMine)
            {
                canvasHUD.gameObject.SetActive(false);
            }

            if (photonView.Owner.IsMasterClient)
            {
                foreach (var item in PhotonNetwork.PlayerList)
                {
                    if (item.IsMasterClient)
                    {
                        Hashtable props = new Hashtable
                        {
                            {CountdownEndGame.CountdownStartTime, (float) PhotonNetwork.Time}
                        };

                        PhotonNetwork.CurrentRoom.SetCustomProperties(props);

                        return;
                    }
                }
            }
        }
        else
        {
            canvasHUD.gameObject.SetActive(false);
        }
    }


    void Update ()
    {
        if (!GameControllerGamePlay.Instance.GetIsOffline())
        {
            if (!photonView.IsMine)
                return;
        }

		//levou dano
        if(damaged)
        {
			//coloca uma cor na imagem de dano
            damageImage.color = flashColour;
        }
        else
        {
			//faz um fading para a imagem de dano sumir
            damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
        damaged = false; //define como falso para já começar no próximo frame o efeito de fading na imagem de dano
    }

	//método utilizado para fazer o jogador levar o dano passado por parâmetro
    public void TakeDamage (int amount, Vector3 hitPoint, Player origin)
    {
        if (!GameControllerGamePlay.Instance.GetIsOffline())
        {
            photonView.RPC("TakeDamageNetwork", RpcTarget.All, amount, hitPoint, origin);
        }
        else
        {
            TakeDamageNetwork(amount, hitPoint, origin);
        }
    }

    [PunRPC]
    public void TakeDamageNetwork(int amount, Vector3 hitPoint, Player origin)
    {
        //pontos
        origin.AddScore(amount);

        hitParticles.transform.position = hitPoint;
        hitParticles.Play();

        damaged = true; //define que levou dano

        currentHealth -= amount; //subtrai o dano da energia atual

        healthSlider.value = currentHealth; //atualiza o slider da energia

        playerAudio.Play(); //toca o áudio de dano

        //se a energia for 0 ou menor, o jogador morreu
        if (currentHealth <= 0 && !isDead)
        {
            Death();
            origin.AddScore(amount);
        }
    }

	//define se o jogador morreu
    void Death ()
    {
        isDead = true;

        playerShooting.DisableEffects ();

        anim.SetTrigger ("Die"); //aciona o trigger Die definido no animator, para rodar a animação que morreu

        GetComponent<CapsuleCollider>().isTrigger = true;

		//áudio de que o jogador morreu
        playerAudio.clip = deathClip;
        playerAudio.Play ();
        

        playerMovement.enabled = false; //desabilita a movimentação do jogador
        playerShooting.enabled = false; //desabilita o tiro do jogador

        if (!GameControllerGamePlay.Instance.GetIsOffline())
        {
            //countdown respawn
            if (photonView.IsMine && !GameControllerGamePlay.Instance.GetIsGameOver())
            {
                Instantiate(canvasRespawn);
            }
        }

        StartCoroutine(DeathEffect(2f));
    }

	//reinicia a fase
    public void RestartLevel ()
    {
		//SceneManager.LoadScene("Level 01");
    }

    private IEnumerator DeathEffect(float time)
    {
        yield return new WaitForSeconds(time);
        
        GetComponent<Rigidbody>().isKinematic = true;
        transform.Translate(new Vector3(0, -60f, 0) * 2.5f * Time.deltaTime);

        /*playerRenderer.enabled = false;
        gunRenderer.enabled = false;
        canvasGameplay.SetActive(false);*/

        StartCoroutine(WaitForRespawn(3f));
    }

    private IEnumerator WaitForRespawn(float time)
    {
        yield return new WaitForSeconds(time);

        Respawn();
    }

    private void Respawn()
    {
        if (GameControllerGamePlay.Instance.GetIsGameOver())
        {
            return;
        }

        isDead = false;

        playerAudio.clip = hurtClip;
        GetComponent<CapsuleCollider>().isTrigger = false;
        GetComponent<Rigidbody>().isKinematic = false;

        Transform[] spawnPlayer = GameObject.Find("GameControllerGamePlay").GetComponent<GameControllerGamePlay>().spawnPlayer;

        int i = Random.Range(0, spawnPlayer.Length);

        transform.position = spawnPlayer[i].position;
        transform.rotation = Quaternion.identity;

        anim.runtimeAnimatorController = null;
        anim.runtimeAnimatorController = animatorController;

        currentHealth = startingHealth;
        healthSlider.value = currentHealth;

        playerMovement.enabled = true;
        playerShooting.enabled = true;
        
        /*playerRenderer.enabled = true;
        gunRenderer.enabled = true;
        canvasGameplay.SetActive(true);*/
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (!GameControllerGamePlay.Instance.GetIsOffline())
        {
            //verifica se este objeto é o mesmo jogador (targetPlayer) que está no parâmetro deste método
            if (photonView.Owner.ActorNumber != targetPlayer.ActorNumber)
                return;

            object tempValue;
            changedProps.TryGetValue("score", out tempValue);
            playerScoreText.text = "Score: " + tempValue.ToString();
        }

    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        object isGameOver;

        if (propertiesThatChanged.TryGetValue("isGameOver", out isGameOver))
        {
            if ((bool)isGameOver)
            {
                GameOverPlayer();
            }
        }
    }

    public void GameOverPlayer()
    {
        playerMovement.enabled = false;
        playerShooting.enabled = false;
        canvasHUD.gameObject.SetActive(false);
    }
}

using UnityEngine;
using System.Collections;

using Photon.Pun;

public class PlayerShooting : MonoBehaviour
{
    public int damagePerShot = 20; //dano de cada tiro
    public float timeBetweenBullets = 0.15f; //tempo entre os tiros
    public float range = 100f; //distância que o tiro alcança


    float timer; //contador utilizado para atirar somente quando puder
    Ray shootRay = new Ray(); //raio utilizado para o tiro
    RaycastHit shootHit; //dados retornados do raio ao acertar algo
    int shootableMask; //máscara que é possível ser acertada pelo tiro
    ParticleSystem gunParticles; //partículas do tiro
    LineRenderer gunLine; //referência para o Line Renderer
    AudioSource gunAudio; //referência para o áudio
	Light gunLight; //referência para a luz do tiro
    float effectsDisplayTime = 0.2f; //quanto tempo os efeitos estarão visíveis

    private PhotonView photonView; // ### Multiplayer


    void Awake ()
    {
        shootableMask = LayerMask.GetMask ("Shootable"); //pega as máscaras que estão na camada "Shootable"
        //pega os componentes
		gunParticles = GetComponent<ParticleSystem> ();
        gunLine = GetComponent <LineRenderer> ();
        gunAudio = GetComponent<AudioSource> ();
        gunLight = GetComponent<Light> ();

        photonView = GetComponent<PhotonView>();
    }


    void Update ()
    {
        if (!photonView.IsMine)
            return;

        ShootUpdate();
    }

    private void ShootUpdate()
    {
        timer += Time.deltaTime; //atualiza o contador de tempo

        //se estiver apertando botão de atirar e está dentro do tempo para atirar
        if (Input.GetButton("Fire1") && timer >= timeBetweenBullets && Time.timeScale != 0)
        {
            Shoot(); //atirar
        }

        //se passou o tempo especificado, desabilitar efeitos do tiro dado anteriormente
        /*if (timer >= timeBetweenBullets * effectsDisplayTime)
        {
            DisableEffects();
        }*/
    }

	//desabilita os efeitos do tiro
    public void DisableEffects ()
    {
        gunLine.enabled = false;
        gunLight.enabled = false;
    }

	//atirar
    void Shoot ()
    {
        /*
        timer = 0f; //reseta o contador de tempo de tiro

        gunAudio.Play (); //toca o áudio do tiro

        gunLight.enabled = true; //habilita a luz do tiro

		//mostra a animação das partículas
        gunParticles.Stop ();
        gunParticles.Play ();

		//ativa o Line Renderer
        gunLine.enabled = true;
        gunLine.SetPosition (0, transform.position); //define a posição do início do Line Renderer
        */

        shootRay.origin = transform.position; //origem do raio
        shootRay.direction = transform.forward; //direção do raio

		//faz um raio shootRay, retorna os dados em quem encostou como shootHit, para uma distância "range", detectando colisões somente em "shootableMask"
        if(Physics.Raycast (shootRay, out shootHit, range, shootableMask)) //acertou algo
        {
            Vector3 hitPoint = shootHit.point;
            hitPoint = Camera.main.WorldToViewportPoint(hitPoint);
            if (hitPoint.x > 0 && hitPoint.x < 1 && hitPoint.y > 0 && hitPoint.y < 1)
            {
                //pega a energia do inimigo em que acertou
                //EnemyHealth enemyHealth = shootHit.collider.GetComponent <EnemyHealth> ();
                PlayerHealth playerHealth = shootHit.collider.GetComponent<PlayerHealth>();

                /*
                if (enemyHealth != null) //inimigo ainda tem energia
                {
                    playerHealth.TakeDamage (damagePerShot, shootHit.point); //faz o inimigo perder energia
                }
			    gunLine.SetPosition (1, shootHit.point); //termina o final da linha se encostar em algo
                */
                if (playerHealth != null && photonView.IsMine) //ainda tem energia
                {
                    playerHealth.TakeDamage(damagePerShot, shootHit.point, photonView.Owner); //faz o inimigo perder energia
                }

                ShootEffect(shootHit.point);
            }
        }
        else
        {
            //gunLine.SetPosition (1, shootRay.origin + shootRay.direction * range); //desenha o tiro normalmente
            ShootEffect(shootRay.origin + shootRay.direction * range);
        }
    }

    private void ShootEffect(Vector3 hitPoint)
    {
        photonView.RPC("ShootEffectNetwork", RpcTarget.All, hitPoint);
    }

    [PunRPC]
    private void ShootEffectNetwork(Vector3 hitPoint)
    {
        timer = 0f; //reseta o contador de tempo de tiro

        gunAudio.Play(); //toca o áudio do tiro

        gunLight.enabled = true; //habilita a luz do tiro

        //mostra a animação das partículas
        gunParticles.Stop();
        gunParticles.Play();

        //ativa o Line Renderer
        gunLine.enabled = true;
        gunLine.SetPosition(0, transform.position); //define a posição do início do Line Renderer

        gunLine.SetPosition(1, hitPoint); //define a posição do final do Line Renderer

        //aguardar para desabilitar
        StartCoroutine(WaitAndDisableEffects(timeBetweenBullets * effectsDisplayTime));
    }

    private IEnumerator WaitAndDisableEffects(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        DisableEffects();
    }
}

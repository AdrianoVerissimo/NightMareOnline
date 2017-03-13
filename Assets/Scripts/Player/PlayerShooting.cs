using UnityEngine;

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


    void Awake ()
    {
        shootableMask = LayerMask.GetMask ("Shootable"); //pega as máscaras que estão na camada "Shootable"
        //pega os componentes
		gunParticles = GetComponent<ParticleSystem> ();
        gunLine = GetComponent <LineRenderer> ();
        gunAudio = GetComponent<AudioSource> ();
        gunLight = GetComponent<Light> ();
    }


    void Update ()
    {
        timer += Time.deltaTime; //atualiza o contador de tempo

		//se estiver apertando botão de atirar e está dentro do tempo para atirar
		if(Input.GetButton ("Fire1") && timer >= timeBetweenBullets && Time.timeScale != 0)
        {
            Shoot (); //atirar
        }

		//se passou o tempo especificado, desabilitar efeitos do tiro dado anteriormente
        if(timer >= timeBetweenBullets * effectsDisplayTime)
        {
            DisableEffects ();
        }
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
        timer = 0f; //reseta o contador de tempo de tiro

        gunAudio.Play (); //toca o áudio do tiro

        gunLight.enabled = true; //habilita a luz do tiro

		//mostra a animação das partículas
        gunParticles.Stop ();
        gunParticles.Play ();

		//ativa o Line Renderer
        gunLine.enabled = true;
        gunLine.SetPosition (0, transform.position); //define a posição do início do Line Renderer

        shootRay.origin = transform.position; //origem do raio
        shootRay.direction = transform.forward; //direção do raio

		//faz um raio shootRay, retorna os dados em quem encostou como shootHit, para uma distância "range", detectando colisões somente em "shootableMask"
        if(Physics.Raycast (shootRay, out shootHit, range, shootableMask)) //acertou algo
        {
			//pega a energia do inimigo em que acertou
            EnemyHealth enemyHealth = shootHit.collider.GetComponent <EnemyHealth> ();
            if(enemyHealth != null) //inimigo ainda tem energia
            {
                enemyHealth.TakeDamage (damagePerShot, shootHit.point); //faz o inimigo perder energia
            }
			gunLine.SetPosition (1, shootHit.point); //termina o final da linha se encostar em algo
        }
        else
        {
            gunLine.SetPosition (1, shootRay.origin + shootRay.direction * range); //desenha o tiro normalmente
        }
    }
}

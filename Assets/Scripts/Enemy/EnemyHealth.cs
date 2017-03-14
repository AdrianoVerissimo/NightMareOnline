using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int startingHealth = 100;
    public int currentHealth;
    public float sinkSpeed = 2.5f; //velocidade com que afunda no chão
    public int scoreValue = 10; //score ganho ao matar este inimigo
    public AudioClip deathClip; //áudio que roda ao este inimigo morrer


    Animator anim;
    AudioSource enemyAudio; //áudio de quando o inimigo leva dano
    ParticleSystem hitParticles;
    CapsuleCollider capsuleCollider;
    bool isDead; //diz se está morto
    bool isSinking; //diz se está afundando no chão


    void Awake ()
    {
        anim = GetComponent <Animator> ();
        enemyAudio = GetComponent <AudioSource> ();
        hitParticles = GetComponentInChildren <ParticleSystem> (); //busca neste objeto ou em seus filhos o primeiro componente ParticleSystem que encontrar
        capsuleCollider = GetComponent <CapsuleCollider> ();

        currentHealth = startingHealth;
    }


    void Update ()
    {
		//está afundando no chão
        if(isSinking)
        {
			//move ele para baixo
            transform.Translate (-Vector3.up * sinkSpeed * Time.deltaTime);
        }
    }

	//diz o quando levou de dano e onde foi acertado
    public void TakeDamage (int amount, Vector3 hitPoint)
    {
        if(isDead)
            return;

        enemyAudio.Play ();

        currentHealth -= amount;

		//move a partícula de hit para a posição onde o hit do tiro ocorreu
        hitParticles.transform.position = hitPoint;
        hitParticles.Play(); //executa animação da partícula

		//não tem mais energia
        if(currentHealth <= 0)
        {
            Death (); //define que morreu
        }
    }

	//método chamado quando o inimigo morre
    void Death ()
    {
        isDead = true; //define que está morto

        capsuleCollider.isTrigger = true; //define que a colisão é um trigger, para não haver bloqueio de passagem

        anim.SetTrigger ("Dead"); //ativa animação de que morreu

        enemyAudio.clip = deathClip; //atribui o áudio do inimigo morrendo
        enemyAudio.Play (); //toca o áudio
    }

	//método responsável por fazer o inimigo afundar no chão
    public void StartSinking ()
    {
        GetComponent <UnityEngine.AI.NavMeshAgent> ().enabled = false; //desabilita o Mesh Agent
        GetComponent <Rigidbody> ().isKinematic = true; //diz que o Rigidbody é kinematic
        isSinking = true; //diz que está afundando
        ScoreManager.score += scoreValue;
        Destroy (gameObject, 2f); //destrói o objeto após 2 segundos
    }
}

using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    public float timeBetweenAttacks = 0.5f; //tempo para efetuar próximo ataque, após atacar
    public int attackDamage = 10; //quantidade de dano


    Animator anim; //referência para o Animator
    GameObject player; //referência para o jogador
    PlayerHealth playerHealth; //referência para o script de energia do jogador
    EnemyHealth enemyHealth;
    bool playerInRange; //indica se o jogador está ao alcance
    float timer;


    void Awake ()
    {
		player = GameObject.FindGameObjectWithTag ("Player"); //resgata referência para o objeto do jogador
        playerHealth = player.GetComponent <PlayerHealth> (); //pega referência para o script de energia do jogador
        enemyHealth = GetComponent<EnemyHealth>();
        anim = GetComponent <Animator> (); //referência para o componente Animator
    }

	//aciona este trigger quando algo colide
    void OnTriggerEnter (Collider other)
    {
		//se o outro objeto é o jogador
        if(other.gameObject == player)
        {
            playerInRange = true; //define que o jogador está ao alcance para um ataque
        }
    }

	//aciona este trigger quando algo sai da colisão
    void OnTriggerExit (Collider other)
    {
        if(other.gameObject == player)
        {
            playerInRange = false; //define que o jogador não está mais ao alcance para um ataque
        }
    }


    void Update ()
    {
		//incrementa o tempo
        timer += Time.deltaTime;

		//o tempo que passou é maior ou igual ao tempo para atacar, o jogador está ao alcance e o inimigo ainda possui energia
        if(timer >= timeBetweenAttacks && playerInRange && enemyHealth.currentHealth > 0)
        {
            Attack (); //atacar
        }

		//energia do jogador é 0 ou menor
        if(playerHealth.currentHealth <= 0)
        {
            anim.SetTrigger ("PlayerDead"); //aciona animação de que o jogador morreu
        }
    }

	//método para atacar
    void Attack ()
    {
        timer = 0f; //reseta o tempo passado

		//o jogador está vivo
        if(playerHealth.currentHealth > 0)
        {
            playerHealth.TakeDamage (attackDamage); //faz o jogador levar dano
        }
    }
}

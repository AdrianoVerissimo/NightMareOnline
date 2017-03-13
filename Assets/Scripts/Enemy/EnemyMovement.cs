using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    Transform player;
    PlayerHealth playerHealth;
    EnemyHealth enemyHealth;
    UnityEngine.AI.NavMeshAgent nav;

	//executado ao iniciar, mesmo o script estando inativado
    void Awake ()
    {
        player = GameObject.FindGameObjectWithTag ("Player").transform;
        playerHealth = player.GetComponent <PlayerHealth> ();
        enemyHealth = GetComponent <EnemyHealth> ();
        nav = GetComponent <UnityEngine.AI.NavMeshAgent> (); //pega o agente a ser usado para fazer a perseguição
    }


    void Update ()
    {
		//se o player e o inimigo estão vivos
        if(enemyHealth.currentHealth > 0 && playerHealth.currentHealth > 0)
        {
            nav.SetDestination (player.position); //faz o inimigo seguir a posição do jogador
        }
        else
        {
			nav.enabled = false; //desabilita o Nav Mesh Renderer
        }
    }
}

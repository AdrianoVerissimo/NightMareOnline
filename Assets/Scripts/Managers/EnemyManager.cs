using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public PlayerHealth playerHealth; //referência para energia do jogador
    public GameObject enemy; //referência para o inimigo que queremos dar spawn
    public float spawnTime = 3f; //tempo para dar spawn em um novo inimigo
    public Transform[] spawnPoints; //guarda os pontos de spawns


    void Start ()
    {
        InvokeRepeating ("Spawn", spawnTime, spawnTime); //chama repetitivamente o método Spawn
    }

	//cria o inimigo em um determinado lugar da fase
    void Spawn ()
    {
		//se o jogador morreu, não dar mais spawns
        if(playerHealth.currentHealth <= 0f)
        {
            return;
        }

        int spawnPointIndex = Random.Range (0, spawnPoints.Length); //pega aleatoriamente um dos pontos de spawn passados como parâmetro

		//instancia o inimigo
        Instantiate (enemy, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
    }
}

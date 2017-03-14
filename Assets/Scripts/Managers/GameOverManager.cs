using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public PlayerHealth playerHealth;
	public float restartDelay;

    Animator anim;
	float restartTimer;


    void Awake()
    {
        anim = GetComponent<Animator>();
    }


    void Update()
    {
		//se o jogador morreu
        if (playerHealth.currentHealth <= 0)
        {
			anim.SetTrigger("GameOver"); //acionar trigger para animação de Game Over

			restartTimer += Time.deltaTime;

			if (restartTimer >= restartDelay)
			{
				playerHealth.RestartLevel ();
			}
        }
    }
}

using UnityEngine;
using UnityEngine.UI; //necessário para utilizar componentes da UI
using System.Collections;
using UnityEngine.SceneManagement;


public class PlayerHealth : MonoBehaviour
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


    void Awake ()
    {
        anim = GetComponent <Animator> ();
        playerAudio = GetComponent <AudioSource> ();
        playerMovement = GetComponent <PlayerMovement> (); //pega referência ao script atrelado ao objeto do jogador como componente
        playerShooting = GetComponentInChildren <PlayerShooting> (); //pega referência do script de tiro
        currentHealth = startingHealth;
    }


    void Update ()
    {
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
    public void TakeDamage (int amount)
    {
        damaged = true; //define que levou dano

        currentHealth -= amount; //subtrai o dano da energia atual

        healthSlider.value = currentHealth; //atualiza o slider da energia

        playerAudio.Play (); //toca o áudio de dano

		//se a energia for 0 ou menor, o jogador morreu
        if(currentHealth <= 0 && !isDead)
        {
            Death ();
        }
    }

	//define se o jogador morreu
    void Death ()
    {
        isDead = true;

        playerShooting.DisableEffects ();

        anim.SetTrigger ("Die"); //aciona o trigger Die definido no animator, para rodar a animação que morreu

		//áudio de que o jogador morreu
        playerAudio.clip = deathClip;
        playerAudio.Play ();

        playerMovement.enabled = false; //desabilita a movimentação do jogador
        playerShooting.enabled = false; //desabilita o tiro do jogador
    }

	//reinicia a fase
    public void RestartLevel ()
    {
		SceneManager.LoadScene("Level 01");
    }
}

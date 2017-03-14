using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static int score; //pontuação


    Text text; //referência para o componente de texto


    void Awake ()
    {
        text = GetComponent <Text> (); //pega referência para o componente de texto
        score = 0; //reseta a pontuação
    }


    void Update ()
    {
        text.text = "Score: " + score; //atualiza o texto da pontuação
    }
}

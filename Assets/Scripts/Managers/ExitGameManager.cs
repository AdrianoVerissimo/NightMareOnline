using UnityEngine;
using System.Collections;
using UnityEditor;

public class ExitGameManager : MonoBehaviour
{

    public virtual void ExitGame()
    {
        if (!Application.isEditor)
            Application.Quit();
        else
            EditorApplication.isPlaying = false;
    }
}

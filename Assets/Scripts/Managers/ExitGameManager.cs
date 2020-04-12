using UnityEngine;
using System.Collections;

public class ExitGameManager : MonoBehaviour
{
    public virtual void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}

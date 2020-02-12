using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class FocusSoundVolume : MonoBehaviour
{
    public AudioMixer audioMixerMaster;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Application.isFocused)
        {
            audioMixerMaster.SetFloat("master", ConvertValueToAudioMixer(1));
        }
        else
        {
            audioMixerMaster.SetFloat("master", ConvertValueToAudioMixer(0.0001f));
        }
    }

    public static float ConvertValueToAudioMixer(float value)
    {
        return Mathf.Log10(value) * 20;
    }
}

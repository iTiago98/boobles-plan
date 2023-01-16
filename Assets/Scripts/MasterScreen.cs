using Booble;
using Booble.Managers;
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MasterScreen : MonoBehaviour
{
    [SerializeField] private EventReference _masterScreenSoundReference;

    private void Awake()
    {
        StartCoroutine(SoundCoroutine());
    }

    private IEnumerator SoundCoroutine()
    {
        EventReference eventReference = _masterScreenSoundReference;
        EventInstance instance;
        instance = RuntimeManager.CreateInstance(eventReference);
        instance.start();
        instance.release();

        yield return new WaitForSeconds(6);

        SceneManager.LoadScene(Scenes.MAIN_MENU);
    }

}

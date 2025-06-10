using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    //Dictionary
    Dictionary<string, AudioSource> soundEffects = new Dictionary<string, AudioSource>();
    Dictionary<string, AudioSource> soundTracks = new Dictionary<string, AudioSource>();

    //ETC
    private AudioSource currentBGM;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Transform parent_soundEffect = transform.Find("SoundEffects");
        foreach (Transform child in parent_soundEffect)
        {
            soundEffects.Add(child.name.Replace("audio_", ""), child.GetComponent<AudioSource>());
        }

        Transform parent_soundTrack = transform.Find("SoundTracks");
        foreach (Transform child in parent_soundTrack)
        {
            soundTracks.Add(child.name.Replace("audio_", ""), child.GetComponent<AudioSource>());
        }
    }

    public void PrintSoundEffect(string audioName)
    {
        AudioSource audioSource = soundEffects[audioName];
        audioSource.PlayOneShot(audioSource.clip);
    }

    public void PlaySoundTrack(string audioName, float volumn = 1f, float fadeDuration = 1f)
    {
        if (!soundTracks.ContainsKey(audioName))
        {
            Debug.LogWarning($"SoundTrack '{audioName}' not found.");
            return;
        }
        AudioSource newBGM = soundTracks[audioName];

        if (currentBGM == newBGM)
            return;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeToNewBGM(newBGM, volumn, fadeDuration));
    }

    private IEnumerator FadeToNewBGM(AudioSource newBGM, float targetVolumn, float duration)
    {
        float time = 0f;
        
        if (currentBGM != null)
        {
            float startVolume = currentBGM.volume;

            while (time < duration)
            {
                time += Time.unscaledDeltaTime;
                currentBGM.volume = Mathf.Lerp(startVolume, 0f, time / duration);
                yield return null;
            }

            currentBGM.Stop();
        }

        yield return new WaitForSeconds(1f);

        currentBGM = newBGM;
        float newBGMVolume = targetVolumn;
        currentBGM.volume = 0f;
        currentBGM.Play();
        time = 0f;
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            currentBGM.volume = Mathf.Lerp(0f, newBGMVolume, time / duration);
            yield return null;
        }

        currentBGM.volume = newBGMVolume;
    }
}

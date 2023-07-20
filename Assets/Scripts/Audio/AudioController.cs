using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    public SettingsController settingsController;

    [Tooltip("The main audio mixer.")]
    public AudioMixer audioMixer;

    [Tooltip("The audio source for background songs.")]
    public AudioSource bgSongSource;
    public readonly string BG_SONG_VOLUME_PARAM = "MusicVolume";

    public readonly string EFFECTS_VOLUME_PARAM = "EffectsVolume";
    [Tooltip("The container for effect game objects.")]
    public GameObject effectsContainer;
    [Tooltip("The audio source prefab for effects.")]
    public GameObject effectsPrefab;

    public float maxMusicVolume = 1.0f;
    public float minMusicVolume = 0.0001f;

    public float maxEffectsVolume = 1.0f;
    public float minEffectsVolume = 0.0001f;

    public void Awake()
    {
        audioMixer.SetFloat(BG_SONG_VOLUME_PARAM, 0f);
    }

    /*public void Start()
    {
        _settingsController = PlayerSingleton.Instance.settingsController;
    }*/

    public void FadeInEffects(float duration)
    {
        float settingsMaxEffectsVolume = settingsController.settings.volumeEffects;
        StartCoroutine(StartFade(audioMixer, EFFECTS_VOLUME_PARAM, duration, settingsMaxEffectsVolume));
    }

    public void FadeOutEffects(float duration)
    {
        StartCoroutine(StartFade(audioMixer, EFFECTS_VOLUME_PARAM, duration, minEffectsVolume));
    }

    public void FadeInBgSong(float duration)
    {
        float settingsMaxBgVolume = settingsController.settings.volumeMusic;
        StartCoroutine(StartFade(audioMixer, BG_SONG_VOLUME_PARAM, duration, settingsMaxBgVolume));
    }

    public void FadeOutBgSong(float duration)
    {
        StartCoroutine(StartFade(audioMixer, BG_SONG_VOLUME_PARAM, duration, minMusicVolume));
    }

    private static IEnumerator StartFade(
        AudioMixer audioMixer,
        string exposedParam,
        float duration,
        float targetVolume)
    {
        float currentTime = 0;
        audioMixer.GetFloat(exposedParam, out float currentVol);
        currentVol = Mathf.Pow(10, currentVol / 20);
        float targetValue = Mathf.Clamp(targetVolume, 0.0001f, 1);
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
            audioMixer.SetFloat(exposedParam, Mathf.Log10(newVol) * 20);
            yield return null;
        }

        yield break;
    }

    public void PlayEffect(AudioClip clip)
    {
        // TODO: create new GameObject as child of effectsContainer with AudioSource (Prefab?)
        //       destroy GameObject when finished playing
    }
}

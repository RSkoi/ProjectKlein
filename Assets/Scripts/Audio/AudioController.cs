using System.Collections;
using System.Collections.Generic;
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

    private readonly List<GameObject> _audioEffects = new();

    public void Awake()
    {
        audioMixer.SetFloat(BG_SONG_VOLUME_PARAM, 0f);
        audioMixer.SetFloat(EFFECTS_VOLUME_PARAM, 0f);
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

    public void SwitchBgSong(AudioClip song, float fadeDuration = 2)
    {
        float settingsMaxBgVolume = settingsController.settings.volumeMusic;
        StartCoroutine(SwitchBgSongFadeOut(song, fadeDuration, settingsMaxBgVolume));
    }

    private IEnumerator SwitchBgSongFadeOut(AudioClip song, float fadeDuration, float maxBgVolume)
    {
        float splitFadeTimeInSeconds = fadeDuration / 2;
        StartCoroutine(StartFade(audioMixer, BG_SONG_VOLUME_PARAM, splitFadeTimeInSeconds, minMusicVolume));

        yield return new WaitForSeconds(splitFadeTimeInSeconds);

        bgSongSource.Stop();
        bgSongSource.clip = song;
        bgSongSource.Play();
        StartCoroutine(StartFade(audioMixer, BG_SONG_VOLUME_PARAM, splitFadeTimeInSeconds, maxBgVolume));
    }

    public void PlayEffect(AudioEffectDataTypeCollection effects)
    {
        foreach (AudioEffectDataType effect in effects.effects)
            PlayEffect(effect);
    }

    public void PlayEffect(List<AudioEffectDataType> effects)
    {
        foreach (AudioEffectDataType effect in effects)
            PlayEffect(effect);
    }

    public void PlayEffect(AudioEffectDataType effect)
    {
        GameObject newEffect = Instantiate(effectsPrefab, effectsContainer.transform, false);
        _audioEffects.Add(newEffect);

        AudioSource source = newEffect.GetComponent<AudioSource>();
        source.priority = effect.priority;
        source.clip = effect.clip;
        source.loop = effect.loop;
        source.volume = effect.volume;
        source.pitch = effect.pitch;
        source.panStereo = effect.stereoPan;
        source.Stop();
        source.Play();

        if (!effect.loop)
            StartCoroutine(DeleteEffect(effect.clip.length, newEffect));
    }

    public void DestroyAllTrackedEffects()
    {
        // TODO: fade out each effect with random duration, then destroy it
        foreach (GameObject go in _audioEffects)
            Destroy(go, Random.Range(0.1f, 0.5f));
    }

    private static IEnumerator DeleteEffect(float pauseBeforeDeletion, GameObject effect) {
        yield return new WaitForSeconds(pauseBeforeDeletion);

        Destroy(effect);
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
}

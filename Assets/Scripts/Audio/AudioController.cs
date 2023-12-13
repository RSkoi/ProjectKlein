using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    public SettingsController settingsController;

    public readonly string MASTER_VOLUME_PARAM = "MasterVolume";
    public readonly string BG_SONG_VOLUME_PARAM = "MusicVolume";
    public readonly string EFFECTS_VOLUME_PARAM = "EffectsVolume";
    public readonly string UI_EFFECTS_VOLUME_PARAM = "UIEffectsVolume";

    [Tooltip("The main audio mixer.")]
    public AudioMixer audioMixer;

    [Tooltip("The audio source for background songs.")]
    public AudioSource bgSongSource;

    [Tooltip("The container for effect game objects.")]
    public GameObject effectsContainer;
    [Tooltip("The audio source prefab for effects.")]
    public GameObject effectsPrefab;

    [Tooltip("The audio source for UI effects.")]
    public AudioSource uiAudioSource;
    [Tooltip("The audio clip for UI effects.")]
    public AudioClip uiAudioClip;

    private Coroutine _switchBgSongCoroutine;
    private Coroutine _fadeBgSongCoroutine;
    private Coroutine _fadeEffectsCoroutine;
    private Coroutine _stopLoopingEffectsCoroutine;

    public float maxMusicVolume = 1.0f;
    public float minMusicVolume = 0.0001f;

    public float maxEffectsVolume = 1.0f;
    public float minEffectsVolume = 0.0001f;

    private readonly List<AudioSource> _audioEffects = new();

    public void Awake()
    {
        audioMixer.SetFloat(MASTER_VOLUME_PARAM, 0f);
        audioMixer.SetFloat(BG_SONG_VOLUME_PARAM, 0f);
        audioMixer.SetFloat(EFFECTS_VOLUME_PARAM, 0f);
        audioMixer.SetFloat(UI_EFFECTS_VOLUME_PARAM, 0f);
    }

    public void FadeInEffects(float duration)
    {
        ResetEffectsFadeCoroutines();

        float settingsMaxEffectsVolume = settingsController.currentSettings.volumeEffects;
        _fadeEffectsCoroutine = StartCoroutine(StartFade(audioMixer, EFFECTS_VOLUME_PARAM, duration, settingsMaxEffectsVolume));
    }

    public void FadeOutEffects(float duration)
    {
        ResetEffectsFadeCoroutines();

        _fadeEffectsCoroutine = StartCoroutine(StartFade(audioMixer, EFFECTS_VOLUME_PARAM, duration, minEffectsVolume));
    }

    public void FadeInBgSong(float duration)
    {
        ResetBgSongFadeCoroutines();

        float settingsMaxBgVolume = settingsController.currentSettings.volumeMusic;
        _fadeBgSongCoroutine = StartCoroutine(StartFade(audioMixer, BG_SONG_VOLUME_PARAM, duration, settingsMaxBgVolume));
    }

    public void FadeOutBgSong(float duration)
    {
        ResetBgSongFadeCoroutines();

        _fadeBgSongCoroutine = StartCoroutine(StartFade(audioMixer, BG_SONG_VOLUME_PARAM, duration, minMusicVolume));
    }

    public void SwitchBgSong(BgSongDataType song, float fadeDuration = 2)
    {
        ResetSwitchBgSongCoroutines();

        float settingsMaxBgVolume = settingsController.currentSettings.volumeMusic;
        _switchBgSongCoroutine = StartCoroutine(SwitchBgSongFadeOut(song, fadeDuration, settingsMaxBgVolume));
    }

    private IEnumerator SwitchBgSongFadeOut(BgSongDataType song, float fadeDuration, float maxBgVolume)
    {
        float splitFadeTimeInSeconds = fadeDuration / 2;
        _fadeBgSongCoroutine = StartCoroutine(StartFade(audioMixer, BG_SONG_VOLUME_PARAM, splitFadeTimeInSeconds, minMusicVolume));

        yield return new WaitForSeconds(splitFadeTimeInSeconds);

        bgSongSource.Stop();
        if (song != null)
        {
            bgSongSource.clip = song.clip;
            bgSongSource.priority = song.priority;
            bgSongSource.loop = song.loop;
            bgSongSource.volume = song.volume;
            bgSongSource.pitch = song.pitch;
            bgSongSource.panStereo = song.stereoPan;
            bgSongSource.Play();
        }

        ResetBgSongFadeCoroutines();
        _fadeBgSongCoroutine = StartCoroutine(StartFade(audioMixer, BG_SONG_VOLUME_PARAM, splitFadeTimeInSeconds, maxBgVolume));
    }

    public void PlayNewEffect(AudioEffectDataType effect)
    {
        Populate(1);

        // assuming that non-looping and not playing effects can be overwritten
        for (int i = 0; i < _audioEffects.Count; i++)
            if (!_audioEffects[i].loop && !_audioEffects[i].isPlaying)
                PlayEffect(effect, i);
    }

    public void PlayEffect(AudioEffectDataTypeCollection effects)
    {
        PlayEffect(effects.effects);
    }

    public void PlayEffect(AudioEffectDataType[] effects)
    {
        int count = CountInactiveEffects();
        if (effects.Length > count)
            Populate(effects.Length - count);

        List<int> freeIndexes = GetFreeIndexes();
        foreach (AudioEffectDataType effect in effects)
        {
            /*if (effect.forceNewInit)
            {
                PlayNewEffect(effect, false);
                continue;
            }*/

            PlayEffect(effect, freeIndexes[0]);
            freeIndexes.RemoveAt(0);
        }
    }

    public void PlayEffect(AudioEffectDataType effect, int poolIndex)
    {
        AudioSource source = _audioEffects[poolIndex];

        //source.Stop();
        source.priority = effect.priority;
        source.clip = effect.clip;
        source.loop = effect.loop;
        source.volume = effect.volume;
        source.pitch = effect.pitch;
        source.panStereo = effect.stereoPan;
        source.Play();

        if (!effect.loop)
            StartCoroutine(StopEffect(effect.clip.length, source));
    }

    public int CountInactiveEffects()
    {
        int res = 0;
        // assuming that non-looping and not playing effects can be overwritten
        foreach (AudioSource effect in _audioEffects)
            if (!effect.loop && !effect.isPlaying)
                res++;
        return res;
    }

    public List<int> GetFreeIndexes()
    {
        List<int> res = new();
        // assuming that non-looping and not playing effects can be overwritten
        for (int i = 0; i < _audioEffects.Count; i++)
            if (!_audioEffects[i].loop && !_audioEffects[i].isPlaying)
                res.Add(i);
        return res;
    }

    public void StopAllLoopingEffectsWithFade(
        Func<AudioEffectDataTypeCollection, int> callback = null,
        AudioEffectDataTypeCollection callbackArg = null,
        float fadeDuration = 0.5f)
    {
        ResetLoopingEffectsCoroutines();

        _stopLoopingEffectsCoroutine = StartCoroutine(StopAllLoopingEffects(callback, callbackArg, fadeDuration));
    }

    private IEnumerator StopAllLoopingEffects(
        Func<AudioEffectDataTypeCollection, int> callback,
        AudioEffectDataTypeCollection callbackArg,
        float fadeDuration)
    {
        FadeOutEffects(fadeDuration);

        yield return new WaitForSeconds(fadeDuration);

        foreach (AudioSource effect in _audioEffects)
            if (effect.loop)
            {
                effect.Stop();
                effect.loop = false;
            }

        callback?.Invoke(callbackArg);

        FadeInEffects(fadeDuration);
    }

    private static IEnumerator StopEffect(float pauseBeforeStop, AudioSource effect) {
        yield return new WaitForSeconds(pauseBeforeStop);

        effect.Stop();
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

    private void Populate(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject go = Instantiate(effectsPrefab, effectsContainer.transform, false);
            _audioEffects.Add(go.GetComponent<AudioSource>());
        }
    }

    private void ResetLoopingEffectsCoroutines()
    {
        ResetCoroutine(_stopLoopingEffectsCoroutine);
        _stopLoopingEffectsCoroutine = null;
    }

    private void ResetEffectsFadeCoroutines()
    {
        ResetCoroutine(_fadeEffectsCoroutine);
        _fadeEffectsCoroutine = null;
    }

    private void ResetSwitchBgSongCoroutines()
    {
        ResetCoroutine(_switchBgSongCoroutine);
        _switchBgSongCoroutine = null;
    }

    private void ResetBgSongFadeCoroutines()
    {
        ResetCoroutine(_fadeBgSongCoroutine);
        _fadeBgSongCoroutine = null;
    }

    private void ResetCoroutine(Coroutine routine)
    {
        if (routine != null)
            StopCoroutine(routine);
    }

    public void PreloadClip(AudioClip clip)
    {
        clip.LoadAudioData();
    }

    private void PlayUiEffect(AudioClip clip)
    {
        // TODO: have different ui effects for different ui elements
        uiAudioSource.PlayOneShot(clip);
    }

    public void PlayUIAudioOneshot()
    {
        PlayUiEffect(uiAudioClip);
    }
}

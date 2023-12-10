using System.Collections;
using UnityEngine;

public class AudioEffectCollector : MonoBehaviour
{
    private AudioSource _source;

    public AudioEffectCollectorData collectorData;

    public void Start()
    {
        _source = GetComponent<AudioSource>();

        StartCoroutine(StartCollector());
    }

    private IEnumerator StartCollector()
    {
        while (true)
        {
            float pause = !collectorData.isRandom
                ? collectorData.randomRateMin
                : Random.Range(collectorData.randomRateMin, collectorData.randomRateMax);
            yield return new WaitForSeconds(pause);

            _source.clip = collectorData.clips[
                Random.Range(0, System.Math.Clamp(collectorData.clips.Count - 1, 0, int.MaxValue))];
            _source.panStereo = Random.Range(collectorData.randomStereoPanMin, collectorData.randomStereoPanMax);
            _source.volume = Random.Range(collectorData.randomVolumeMin, collectorData.randomVolumeMax);
            _source.Play();
        }
    }
}

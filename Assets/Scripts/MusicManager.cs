using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    [SerializeField] private GameObject RecordScratchSFX;   
    private AudioSource[] _audioSources;
    private AudioSource _normalSource;
    private AudioSource _rushSource;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _audioSources = GetComponents<AudioSource>();
        _normalSource = _audioSources[0];
        _rushSource = _audioSources[1];
    }

    public void PlayNormalMusic()
    {
        _normalSource.UnPause();
        _rushSource.volume = 0f;
    }

    public void PlayRushMusic()
    {
        StartCoroutine(TransitionToRushMusic());
    }

    IEnumerator TransitionToRushMusic()
    {
        float timeOfStopSFX = 1.0f;
        _normalSource.Pause();
        Instantiate(RecordScratchSFX, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(timeOfStopSFX);
        _rushSource.volume = 0.184f;
    }


}

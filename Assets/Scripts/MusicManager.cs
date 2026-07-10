using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    [SerializeField] private GameObject RecordScratchSFX;
    [SerializeField] private AudioClip NormalMusic;
    [SerializeField] private AudioClip RushMusic;
    private AudioSource _audioSource;

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

        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = NormalMusic;
        _audioSource.loop = true;
        _audioSource.Play();
    }

    public void PlayNormalMusic()
    {
        _audioSource.clip = NormalMusic;
        _audioSource.Play();
    }

    public void PlayRushMusic()
    {
        StartCoroutine(TransitionToRushMusic());
    }

    IEnumerator TransitionToRushMusic()
    {
        float timeOfStopSFX = 1.0f;
        _audioSource.Pause();
        Instantiate(RecordScratchSFX, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(timeOfStopSFX);
        _audioSource.clip = RushMusic;
        _audioSource.Play();
    }


}

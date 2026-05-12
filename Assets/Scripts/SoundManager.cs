using UnityEngine;

public enum SoundType
{
    Intro,
    TakeOrder,
    Mixing,
    Oven,
    Drinks,
    AfterDrinksAndOven,
    Serving,
    Walking
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] soundList;
    private static SoundManager instance;
    private AudioSource audioSrc;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        audioSrc = GetComponent<AudioSource>();
    }

    public static void PlaySound(SoundType type, float volume = 1f)
    {
        instance.audioSrc.clip = instance.soundList[(int)type];
        instance.audioSrc.volume = volume;
        instance.audioSrc.Play();
    }

    public static void PauseSound()
    {
        instance.audioSrc.Pause();
    }

    public static bool TutorialIsPlaying()
    {
        return instance.audioSrc.isPlaying;
    }
}
using UnityEngine;
using DG.Tweening;

public class AudioTrack : MonoBehaviour
{
    public bool isPlaying;
    public float clipTime;//音频片段限制时间

    [HideInInspector] public AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0;
    }

    private void Start()
    {
        audioSource.Play();
        if (MusicController.instance.tracks[0] == this)
            audioSource.time = MusicController.instance.initialTime;
    }

    public void Play()
    {
        isPlaying = true;
        if (MusicController.instance.tracks[0] != this)
            audioSource.time = MusicController.instance.tracks[0].audioSource.time;
        DOTween.To(() => audioSource.volume, x => audioSource.volume = x, 1, 1f);
    }

    public void Stop()
    {
        DOTween.To(() => audioSource.volume, x => audioSource.volume = x, 0, 0.5f);
        isPlaying = false;
    }
}

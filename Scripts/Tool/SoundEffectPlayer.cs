using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SoundEffectPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioClip[] soundEffects;
    public AudioMixerGroup mixerGroup;
    //public AudioClips_SO[] animationEventSoundEffects;

    public void PlaySoundEffect(int index)
    {
        SetAudioSource();
        audioSource.PlayOneShot(soundEffects[index]);
        StartCoroutine(OnClipPlayComplete(audioSource, soundEffects[index].length));
    }//播放指定音频

    public void PlaySoundEffect(int index, float volume)
    {
        SetAudioSource();
        audioSource.PlayOneShot(soundEffects[index], volume);
        StartCoroutine(OnClipPlayComplete(audioSource, soundEffects[index].length));
    }//播放指定音频，并且跳转至指定时刻

    public void PlaySoundEffect(int startIndex, int lastIndex)
    {
        SetAudioSource();
        int randomIndex = Random.Range(startIndex, lastIndex + 1);
        audioSource.PlayOneShot(soundEffects[randomIndex], 0.5f);
        StartCoroutine(OnClipPlayComplete(audioSource, soundEffects[randomIndex].length));
    }//随机播放指定区间内音频

    //public void PlayRadomSoundEffect_AnimationEvent(int index)
    //{
    //    SetAudioSource();
    //    int randomIndex = Random.Range(0, animationEventSoundEffects.Length);
    //    audioSource.PlayOneShot(animationEventSoundEffects[index].audioClips[randomIndex]);
    //    StartCoroutine(OnClipPlayComplete(audioSource, animationEventSoundEffects[index].audioClips[randomIndex].length));
    //}//动画事件播放指定组中随机音频

    private void SetAudioSource()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = mixerGroup;
    }

    public void StopAudio()
    {
        GetComponent<AudioSource>()?.Stop();
    }

    IEnumerator OnClipPlayComplete(AudioSource audioSource, float clipLength)
    {
        yield return new WaitForSeconds(clipLength);
        Destroy(audioSource);
        audioSource = null;
    }

    public void SpikeAudio()
    {
        GetComponent<AudioSource>().Play();
    }
}

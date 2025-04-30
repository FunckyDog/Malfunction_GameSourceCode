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
    }//����ָ����Ƶ

    public void PlaySoundEffect(int index, float volume)
    {
        SetAudioSource();
        audioSource.PlayOneShot(soundEffects[index], volume);
        StartCoroutine(OnClipPlayComplete(audioSource, soundEffects[index].length));
    }//����ָ����Ƶ��������ת��ָ��ʱ��

    public void PlaySoundEffect(int startIndex, int lastIndex)
    {
        SetAudioSource();
        int randomIndex = Random.Range(startIndex, lastIndex + 1);
        audioSource.PlayOneShot(soundEffects[randomIndex], 0.5f);
        StartCoroutine(OnClipPlayComplete(audioSource, soundEffects[randomIndex].length));
    }//�������ָ����������Ƶ

    //public void PlayRadomSoundEffect_AnimationEvent(int index)
    //{
    //    SetAudioSource();
    //    int randomIndex = Random.Range(0, animationEventSoundEffects.Length);
    //    audioSource.PlayOneShot(animationEventSoundEffects[index].audioClips[randomIndex]);
    //    StartCoroutine(OnClipPlayComplete(audioSource, animationEventSoundEffects[index].audioClips[randomIndex].length));
    //}//�����¼�����ָ�����������Ƶ

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

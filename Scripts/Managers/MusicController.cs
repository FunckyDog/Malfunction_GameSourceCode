using UnityEngine;
using UnityEngine.Audio;

public class MusicController : Singleton<MusicController>
{
    [Header("�۲�����")]
    public float currentTime;
    [HideInInspector] public float startTime;
    [HideInInspector] public double masterTime;

    [Header("����")]
    public AudioTrack[] tracks;
    public float initialTime;
    public float checkTime;
    public AudioMixer mixer;

    [HideInInspector] public float checkWaitTime;

    protected override void Awake()
    {
        base.Awake();
        startTime = (float)AudioSettings.dspTime;
    }

    private void Start()
    {
        AddMusic(1);
    }

    private void FixedUpdate()
    {
        TrackCheck();
    }

    private void Update()
    {

        if (tracks[0].audioSource.time >= 144)
        {
            tracks[0].audioSource.time = 0;
        }
    }

    void TrackCheck()
    {
        if (checkWaitTime < checkTime)
            checkWaitTime += Time.deltaTime;
        else
        {
            checkWaitTime = 0;
            for (int i = 1; i < tracks.Length; i++)
            {
                if (tracks[i].audioSource.clip.length >= tracks[0].audioSource.time)
                    tracks[i].audioSource.time = tracks[0].audioSource.time;
            }
        }
    }

    public void DescreaseAllMumic()
    {
        for (int i = 0; i < tracks.Length; i++)
            tracks[i].Stop();
    }//ȥ��ȫ������

    public void AddAllMusic()
    {
        for (int i = 0; i < tracks.Length; i++)
            tracks[i].Play();
    }

    public void AddMusic(int index)
    {
        tracks[index].Play();
    }//�������

    public void DescreaseMusic(int index)
    {
        tracks[index].Stop();
    }//ȥ������

}

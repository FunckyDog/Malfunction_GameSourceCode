using UnityEngine;
using UnityEngine.Audio;

public class MusicController : Singleton<MusicController>
{
    [Header("观测数据")]
    public float currentTime;
    [HideInInspector] public float startTime;
    [HideInInspector] public double masterTime;

    [Header("属性")]
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
    }//去除全部音轨

    public void AddAllMusic()
    {
        for (int i = 0; i < tracks.Length; i++)
            tracks[i].Play();
    }

    public void AddMusic(int index)
    {
        tracks[index].Play();
    }//添加音轨

    public void DescreaseMusic(int index)
    {
        tracks[index].Stop();
    }//去除音轨

}

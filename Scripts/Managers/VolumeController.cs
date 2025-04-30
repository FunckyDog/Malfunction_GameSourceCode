using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VolumeController : Singleton<VolumeController>
{
    private Volume volume;
    [HideInInspector] public ChromaticAberration chromaticAberration;

    protected override void Awake()
    {
        base.Awake();
        volume = GetComponent<Volume>();
        volume.profile.TryGet(out chromaticAberration);
    }

    public void FadeValue(float value, float targetValue, float duaring)
    {
        DOTween.To(() => value, tv => value = tv, targetValue, duaring);
    }

    public void ChromaticAberrationShake(float targetValue, float shakeTime)
    {
        chromaticAberration.intensity.value = targetValue;
        DOTween.To(() => chromaticAberration.intensity.value, tv => chromaticAberration.intensity.value = tv, 0, shakeTime);
    }
}

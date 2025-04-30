using UnityEngine;

public class ShooterAnimation : ChaserAnimation
{
    public Transform firePosTrans, shootParticleTrans;
    
    protected override void Start()
    {
        base.Start();
        firePosTrans.SetParent(transform.GetChild(6));
        shootParticleTrans.SetParent(transform.GetChild(6));
        shootParticleTrans.localPosition = firePosTrans.localPosition = Vector2.right * 0.7f;
    }

    protected override void AnimationSwitch()
    {
        eyesAnim.SetBool("Dead", GetComponentInParent<Enemy>().isDead);
    }
}

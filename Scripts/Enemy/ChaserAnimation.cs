public class ChaserAnimation : EnemyAnimation
{
    protected override void AnimationSwitch()
    {
        eyesAnim.SetBool("Dead", GetComponentInParent<Enemy>().isDead);
    }
}

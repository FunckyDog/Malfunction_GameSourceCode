using System.Collections;
using UnityEngine;

public class Chaser : Enemy
{
    public Collider2D attackArea;

    protected override void OnEnable()
    {
        base.OnEnable();
        attackArea.enabled = true;
    }


    protected override void Move()
    {
        rb.velocity = (PlayerController.instance.transform.position - transform.position).normalized * speed;
    }

    protected override IEnumerator Dead()
    {
        attackArea.enabled = false;
        yield return base.Dead();

        CameraController.instance.enemyDeadCIS.GenerateImpulse();
        GetComponentInChildren<SoundEffectPlayer>().PlaySoundEffect(0);
    }
}

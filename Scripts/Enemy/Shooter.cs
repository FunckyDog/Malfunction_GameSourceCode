using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : Enemy
{
    public float circulateRange;
    public float moveInterval;
    public BoxCollider2D attackArea;


    private float moveIntervalWaitTime;
    private Vector2 moveDir;

    protected override void OnEnable()
    {
        base.OnEnable();
        attackArea.enabled = true;
        if (PlayerController.instance)
            moveDir = new Vector2(Random.Range(0f, (PlayerController.instance.transform.position - transform.position).normalized.x), Random.Range(0f, (PlayerController.instance.transform.position - transform.position).normalized.y));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, circulateRange);
    }

    protected override void Move()
    {
        if (Vector2.Distance(transform.position, PlayerController.instance.transform.position) >= circulateRange)
        {
            moveIntervalWaitTime += Time.deltaTime;
            if (moveIntervalWaitTime > moveInterval)
                rb.velocity = moveDir * speed;
            if (moveIntervalWaitTime > moveInterval * Random.Range(1.5f, 2f))
            {
                moveIntervalWaitTime = 0;
                moveDir = new Vector2(Random.Range(0f, (PlayerController.instance.transform.position - transform.position).normalized.x), Random.Range(0f, (PlayerController.instance.transform.position - transform.position).normalized.y));
            }
        }
    }

    protected override IEnumerator Dead()
    {
        attackArea.enabled = false;
        yield return base.Dead();

        CameraController.instance.enemyDeadCIS.GenerateImpulse();
    }
}

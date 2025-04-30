using System.Linq;
using UnityEngine;

public class EnemyBullet : Bullet
{
    public int damage;
    public float lifeTime;

    private void Update()
    {
        if (lifeWaitTime <= lifeTime)
            lifeWaitTime += Time.deltaTime;

        else
            OnlifeOver();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (targetTags.Contains(collision.tag))
        {
            lifeWaitTime = lifeTime;
            collision.GetComponentInParent<IGetHurt>()?.GetHurt(transform.position, attackForce, damage);
        }
    }
}

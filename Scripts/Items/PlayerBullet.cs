using System.Linq;
using UnityEngine;

public class PlayerBullet : Bullet
{
    public AttributeData_SO damage;
    public AttributeData_SO bulletLife;

    private void Update()
    {
        if (lifeWaitTime <= bulletLife._currentValue)
        {
            lifeWaitTime += Time.deltaTime;
            bulletLife.SetValue(bulletLife.consumptionValue*Time.deltaTime);
        }

        else
            OnlifeOver();
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (targetTags.Contains(collision.tag))
        {
            OnlifeOver();
            collision.GetComponentInParent<IGetHurt>()?.GetHurt(transform.position, attackForce, (int)damage._currentValue);
        }
        if (collision.tag == "Enemy")
            damage.SetValue(damage.consumptionValue);
    }
}

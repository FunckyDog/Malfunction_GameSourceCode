using System.Collections;
using UnityEngine;

public class PlayerWeapon : Weapon
{
    public float chargeWaitTime;

    public PlayerWeaponData_SO weaponData;

    private float _shootRate;

    protected virtual void Update()
    {
        if (shootIntervalWaitTime <= _shootRate)
            shootIntervalWaitTime += Time.deltaTime;

        if (chargeWaitTime == 0)
            _shootRate = weaponData.shootRate._currentValue;
    }

    public void Charge()
    {
        chargeWaitTime += Time.deltaTime;
    }

    public IEnumerator ChargeShoot()
    {
        _shootRate = weaponData.shootRate._currentValue * 0.5f;

        while (chargeWaitTime > 0)
        {
            chargeWaitTime -= Time.deltaTime;
            yield return Shoot();
            yield return null;
        }

        chargeWaitTime = 0;
        _shootRate = weaponData.shootRate._currentValue;
    }

    public override IEnumerator Shoot()
    {
        if (shootIntervalWaitTime > _shootRate)
        {
            for (int i = 0; i < weaponData.bulletCount; i++)
            {
                GameObject _bullet = PoolManager.instance.GetObject(weaponData.bulletPrefab);
                _bullet.transform.position = firePointTrans.position;
                _bullet.GetComponentInChildren<Fake3DStacker>().spriteTransRight = (PlayerController.instance.mousePos - transform.position).normalized;
                _bullet.GetComponent<Bullet>().rb.AddForce((PlayerController.instance.mousePos - transform.position).normalized * weaponData.bulletSpeed, ForceMode2D.Impulse);
                shootFrequencyWaitTime = 0;

                while (shootFrequencyWaitTime < weaponData.shootFrequency)
                {
                    shootFrequencyWaitTime += Time.deltaTime;
                    yield return null;
                }
            }
            weaponData.shootRate.SetValue(weaponData.shootRate.consumptionValue);
            shootIntervalWaitTime = 0;

            PlayerAnimation.instance.eyesAnim.SetTrigger("Shoot");
            PlayerAnimation.instance.shootParticleTrans.GetComponent<ParticleSystem>().Play();
            CameraController.instance.playerShootCIS.GenerateImpulse(transform.right * 0.25f);
            transform.parent.parent.parent.GetComponentInChildren<SoundEffectPlayer>().PlaySoundEffect(2,0.6f);
        }
    }
}

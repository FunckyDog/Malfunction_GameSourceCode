using System.Collections;
using UnityEngine;

public class EnemyWeapon : Weapon
{
    public EnemyWeaponData_SO weaponData;

    private float _shootRate;

    private void Awake()
    {
        _shootRate = Random.Range(weaponData.shootRate / 2, weaponData.shootRate);
    }

    void Update()
    {
        if (PlayerController.instance && !PlayerController.instance.isDead && !GetComponentInParent<Enemy>().isDead)
        {
            if (shootIntervalWaitTime < _shootRate)
                shootIntervalWaitTime += Time.deltaTime;

            else
                StartCoroutine(Shoot());
        }
    }

    public override IEnumerator Shoot()
    {
        if (shootIntervalWaitTime > _shootRate)
        {
            for (int i = 0; i < weaponData.bulletCount; i++)
            {
                GameObject _bullet = PoolManager.instance.GetObject(weaponData.bulletPrefab);
                _bullet.transform.position = firePointTrans.position;
                _bullet.GetComponentInChildren<Fake3DStacker>().spriteTransRight = firePointTrans.right;
                _bullet.GetComponent<Bullet>().rb.AddForce(firePointTrans.right * weaponData.bulletSpeed, ForceMode2D.Impulse);
                shootFrequencyWaitTime = 0;

                while (shootFrequencyWaitTime < weaponData.shootFrequency)
                {
                    shootFrequencyWaitTime += Time.deltaTime;
                    yield return null;
                }
            }
            shootIntervalWaitTime = 0;
            _shootRate = Random.Range(weaponData.shootRate / 2, weaponData.shootRate);
            GetComponentInParent<ChaserAnimation>().eyesAnim.SetTrigger("Shoot");
            transform.parent.parent.GetComponentInChildren<ShooterAnimation>().shootParticleTrans.GetComponent<ParticleSystem>().Play();
            GetComponentInParent<SoundEffectPlayer>().PlaySoundEffect(1, 0.5f);
        }
    }
}

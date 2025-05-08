using System.Collections;
using UnityEngine;

public class Self_Destructioner : Chaser
{
    [SerializeField] private bool isDestruction;
    public float destructionTime;

    private float destructionWaitTime;
    private WaitForSeconds shiningInterval = new WaitForSeconds(0.1f);

    protected override void OnEnable()
    {
        base.OnEnable();
        isDestruction = false;
        destructionWaitTime = 0;
        attackArea.enabled = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red - new Color(0, 0, 0, 0.5f);
        Gizmos.DrawWireSphere(transform.position, GetComponentInChildren<CircleCollider2D>().radius);
    }

    private void Update()
    {
        if (isDestruction && destructionWaitTime < destructionTime)
        {
            destructionWaitTime += Time.deltaTime;
        }
        if ( destructionWaitTime > destructionTime)
            GetHurt(transform.position, 0, currentHealth);
    }

    protected override void Move()
    {
        if (!isDestruction)
        {
            if (Vector2.Distance(PlayerController.instance.transform.position, transform.position) >= 1f)
                rb.velocity = (PlayerController.instance.transform.position - transform.position).normalized * speed;
            else
            {
                rb.velocity = Vector2.zero;
                isDestruction = true;
                StartCoroutine(Shining());
            }
        }
    }

    IEnumerator Shining()
    {
        while (destructionWaitTime < destructionTime)
        {
            GetComponentInChildren<Self_DestructionerAnimation>().bodyStacker.ObjectColor(Color.black);
            yield return shiningInterval;
            GetComponentInChildren<Self_DestructionerAnimation>().bodyStacker.ObjectColor(Color.white);
            yield return shiningInterval;
        }
    }

    protected override IEnumerator Dead()
    {
        attackArea.enabled = true;
        yield return new WaitForFixedUpdate();
        GameManager.instance.grindEnemyCountInCurrentWave--;

        if (Random.Range(0f, 1f) < 1f * GameManager.instance.supplyProbability)
        {
            Supply _lootSupply = PoolManager.instance.GetObject(lootPrefab).GetComponent<Supply>();
            _lootSupply.transform.position = transform.position;
            _lootSupply._particle = PoolManager.instance.GetObject(_lootSupply.particlePrefab);
            _lootSupply._particle.transform.position = _lootSupply.transform.position;
        }

        PoolManager.instance.GetObject(GetComponentInChildren<EnemyAnimation>().deadParticlePrefab).transform.position = transform.position;
        CameraController.instance.enemyDestructionCIS.GenerateImpulse();
        GetComponentInChildren<Self_DestructionerAnimation>().bodyStacker.ObjectColor(Color.white);
        PoolManager.instance.PushObject(gameObject);
    }
}

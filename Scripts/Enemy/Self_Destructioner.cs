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
        if (destructionWaitTime > destructionTime)
        {
            attackArea.enabled = true;
            GetHurt(transform.position, 0, currentHealth);
        }
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
        GameManager.instance.grindEnemyCountInCurrentWave--;
        yield return new WaitForFixedUpdate();

        if (Random.Range(0f, 1f * GameManager.instance.supplyProbability) < 1f * GameManager.instance.supplyProbability)
            PoolManager.instance.GetObject(lootPrefab).transform.position = transform.position;

        PoolManager.instance.GetObject(GetComponentInChildren<EnemyAnimation>().deadParticlePrefab).transform.position = transform.position;
        CameraController.instance.enemyDestructionCIS.GenerateImpulse();
        PoolManager.instance.PushObject(gameObject);
    }
}

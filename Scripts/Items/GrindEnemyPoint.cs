using UnityEngine;

public class GrindEnemyPoint : MonoBehaviour
{
    public float lifeTime;
    public GameObject grindParticlePrefab;

    private float lifeWaitTime;

    private void OnEnable()
    {
        lifeWaitTime = 0;
    }

    private void Update()
    {
        if (lifeWaitTime < lifeTime)
            lifeWaitTime += Time.deltaTime;
        else
        {
            PoolManager.instance.GetObject(grindParticlePrefab).transform.position = transform.position;
            PoolManager.instance.PushObject(gameObject);
        }
    }
}

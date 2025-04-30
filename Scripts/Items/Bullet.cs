using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float attackForce;
    [Tooltip("¹¥»÷¶ÔÏó")] public string[] targetTags;

    public GameObject deadParticlePrefab;

    [HideInInspector] public Rigidbody2D rb;
    protected float lifeWaitTime;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        GetComponentInChildren<Fake3DStacker>().StackObject();
    }

    protected virtual void OnEnable()
    {
        lifeWaitTime = 0;
    }

    private void Update()
    {

    }

    protected virtual void OnlifeOver()
    {
        PoolManager.instance.GetObject(deadParticlePrefab).transform.position = transform.position;
        PoolManager.instance.PushObject(gameObject);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {

    }
}

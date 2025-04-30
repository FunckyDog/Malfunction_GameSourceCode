using System.Collections;
using System.Linq;
using UnityEngine;

public class Enemy : MonoBehaviour, IGetHurt
{
    [Header("×´Ì¬")]
    public bool isDead;

    [Header("ÊôÐÔ")]
    public int health;
    public float speed;
    public float attackForce;
    public int damage;
    public GameObject lootPrefab;

    [Tooltip("¹¥»÷¶ÔÏó")] public string[] targetTags;
    public Transform bodyTrans;
    public BoxCollider2D hurtArea;

    protected Rigidbody2D rb;
    protected int currentHealth;
    private WaitForSeconds waitForDead = new WaitForSeconds(1);

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void OnEnable()
    {
        isDead = false;
        hurtArea.enabled = true;
        currentHealth = health;
    }

    private void FixedUpdate()
    {
        if (PlayerController.instance && !PlayerController.instance.isDead && Vector2.Distance(PlayerController.instance.transform.position, transform.position) >= 0.5f && !isDead)
        {
            Move();
            Direction();
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (targetTags.Contains(collision.tag))
            collision.GetComponentInParent<IGetHurt>()?.GetHurt(transform.position, attackForce, damage);
    }

    public void GetHurt(Vector2 bulletPos, float force, int damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, health);
        rb.AddForce(((Vector2)transform.position - bulletPos).normalized * force, ForceMode2D.Impulse);

        if (currentHealth == 0)
            StartCoroutine(Dead());
    }

    protected virtual void Direction()
    {
        Vector2 dirRelativetoPlayer = (PlayerController.instance.transform.position - transform.position).normalized;

        GetComponentInChildren<EnemyAnimation>().bodyStacker.spriteTransRight = dirRelativetoPlayer;
    }

    protected virtual void Move()
    {

    }

    protected virtual IEnumerator Dead()
    {
        isDead = true;
        hurtArea.enabled = false;
        yield return waitForDead;
        GameManager.instance.grindEnemyCountInCurrentWave--;
        if (Random.Range(0f, 1f * GameManager.instance.supplyProbability) < 1f * GameManager.instance.supplyProbability)
        {
            Supply _lootSupply = PoolManager.instance.GetObject(lootPrefab).GetComponent<Supply>();
            _lootSupply.transform.position = transform.position;
            _lootSupply._particle = PoolManager.instance.GetObject(_lootSupply.particlePrefab);
            _lootSupply._particle.transform.position = _lootSupply.transform.position;
        }

        PoolManager.instance.GetObject(GetComponentInChildren<EnemyAnimation>().deadParticlePrefab).transform.position = transform.position;
        PoolManager.instance.PushObject(gameObject);
    }
}

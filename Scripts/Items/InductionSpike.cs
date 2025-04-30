using System.Linq;
using UnityEngine;

public class InductionSpike : MonoBehaviour
{
    [SerializeField] private bool hasChecktarget;

    [Tooltip("ºÏ≤‚∂‘œÛ")] public string[] targetTags;
    public float attackTime;
    public float attackForce;
    public int damage;
    public BoxCollider2D attackArea;

    private float attackWaitTime;
    private Animator anim;

    private void Awake()
    {
        attackArea.enabled = false;
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (hasChecktarget)
        {
            attackWaitTime += Time.deltaTime;
            if (attackWaitTime > attackTime)
            {
                attackArea.enabled = true;

                anim.SetBool("Thrust", true);
            }

            if (attackWaitTime > attackTime * 2f)
            {
                attackArea.enabled = false;
                hasChecktarget = false;
                attackWaitTime = 0;

                anim.SetBool("Checked", false);
                anim.SetBool("Thrust", false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (targetTags.Contains(collision.tag) && collision.transform.parent.name != "Enemy_FlyChaser(Clone)")
        {
            if (!hasChecktarget)
            {
                hasChecktarget = true;

                anim.SetBool("Checked", true);
            }

            if (attackArea.enabled)
                collision.GetComponentInParent<IGetHurt>()?.GetHurt(transform.position, attackForce, damage);
        }
    }
}

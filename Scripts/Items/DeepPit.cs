using System.Collections;
using System.Linq;
using UnityEngine;

public class DeepPit : MonoBehaviour
{
    [Tooltip("ºÏ≤‚∂‘œÛ")] public string[] targetTags;

    public int damage;

    private Vector2 checkedPos, playerDir;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (targetTags.Contains(collision.tag))
        {
            checkedPos = transform.position;
            playerDir = (PlayerController.instance.transform.position - transform.position).normalized;
            PlayerController.instance.Fall();
            StartCoroutine(RecoverPlayer());
        }
    }

    IEnumerator RecoverPlayer()
    {
        yield return new WaitForSeconds(2);
        PlayerController.instance.GetHurt(transform.position, 0, damage);
        PlayerController.instance.transform.position = checkedPos + playerDir * 2f;
        PlayerController.instance.transform.localScale = Vector3.one;
    }
}

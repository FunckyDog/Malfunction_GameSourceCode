using System.Collections;
using UnityEngine;

public class Spider_like_MovingArthropod : MonoBehaviour
{
    public Rigidbody2D rb;
    [Tooltip("节肢")] public Transform[] arthropods;
    [Tooltip("节肢原本位置")] public Vector2[] arthropodInitializedPoses;
    [Tooltip("节肢预制体")] public GameObject arthropodPrefab;

    [Header("节肢属性")]
    [Tooltip("节肢长度")] public float arthropodsLength;
    [Tooltip("调整距离")] public float adjustDistance;
    [Tooltip("调整速度")] public float adjustSpeed;
    [Tooltip("调整曲线Y轴偏移量")] public float curveYOffet;
    [Tooltip("调整等待时间")] public float adjustTimeMin, adjustTimeMax;

    [Tooltip("节肢是否可旋转")] public bool arthropodIsRotate;
    [Tooltip("旋转是否影响节肢方向")] public bool arthropodDirIsRotate;

    private Vector2[] arthropodTargetPoses;//节肢目标位置
    private Vector2[] arthropodCurrentTargetPoses;//节肢当前目标位置
    private float[] adjustTimes;
    private float[] adjustWaitTimes;

    private void Awake()
    {
        adjustWaitTimes = new float[arthropodInitializedPoses.Length];
        adjustTimes = new float[arthropodInitializedPoses.Length];
        arthropodTargetPoses = new Vector2[arthropodInitializedPoses.Length];
        arthropodCurrentTargetPoses = new Vector2[arthropodInitializedPoses.Length];
    }

    private void OnEnable()
    {
        if (arthropodInitializedPoses != null && PoolManager.isInitialized)
        {
            arthropods = new Transform[arthropodInitializedPoses.Length];
            for (int i = 0; i < arthropodInitializedPoses.Length; i++)
                arthropods[i] = PoolManager.instance.GetObject(arthropodPrefab).transform;
        }

        DirectionlyAdjustArthropods();
    }

    private void OnDisable()
    {
        if (PoolManager.instance)
            for (int i = 0; i < arthropodInitializedPoses.Length; i++)
                PoolManager.instance.PushObject(arthropods[i].gameObject);
    }

    private void OnDrawGizmos()
    {
        if (arthropodInitializedPoses != null)
            for (int i = 0; i < arthropodInitializedPoses.Length; i++)
            {
                Gizmos.color = Color.yellow - new Color(0, 0, 0, 0.5f);
                Gizmos.DrawSphere((Vector2)transform.localPosition + arthropodInitializedPoses[i].normalized * arthropodsLength, adjustDistance);
                Gizmos.color = Color.blue - new Color(0, 0, 0, 0.8f);
            }
    }

    private void Update()
    {
        ArthropodAdjust();
        TargetPosMove();
    }

    void ArthropodAdjust()
    {
        for (int i = 0; i < arthropods.Length; i++)
        {
            if (Vector2.Distance(arthropodTargetPoses[i], arthropodCurrentTargetPoses[i]) > adjustDistance && adjustTimes[i] != 0)
            {
                if (adjustWaitTimes[i] < adjustTimes[i])
                    adjustWaitTimes[i] += Time.deltaTime;
                else
                {
                    StartCoroutine(BezierPoint(i));
                    adjustTimes[i] = 0;
                }
            }

            if (Vector2.Distance(arthropods[i].position, arthropodCurrentTargetPoses[i]) <= 0.1f && adjustTimes[i] == 0)
            {
                adjustWaitTimes[i] = 0;
                adjustTimes[i] = Random.Range(adjustTimeMin, adjustTimeMax);
            }
        }
    }

    void TargetPosMove()
    {
        for (int i = 0; i < arthropods.Length; i++)
        {
            arthropodTargetPoses[i] = Vector2.Lerp(arthropodTargetPoses[i], (Vector2)(transform.position + (arthropodIsRotate ? (transform.right * arthropodInitializedPoses[i].normalized.x + transform.up * arthropodInitializedPoses[i].normalized.y) * arthropodsLength : arthropodInitializedPoses[i].normalized * arthropodsLength)) + rb.velocity / adjustSpeed, Time.deltaTime * 100);
        }
    }

    IEnumerator BezierPoint(int index)
    {
        float t = 0;
        Vector2 pStart = (Vector2)arthropods[index].position;
        Vector2 pCenter = (pStart + arthropodTargetPoses[index]) / 2 + Vector2.up * curveYOffet;
        arthropodCurrentTargetPoses[index] = arthropodTargetPoses[index];

        while (Vector2.Distance(arthropods[index].position, arthropodCurrentTargetPoses[index]) > 0.1f)
        {
            t = Mathf.Clamp(t + Time.deltaTime * adjustSpeed, 0, 1);
            arthropods[index].position = Mathf.Pow(1 - t, 2) * pStart + 2 * (1 - t) * t * pCenter + Mathf.Pow(t, 2) * arthropodCurrentTargetPoses[index];
            if (arthropodIsRotate && arthropodDirIsRotate)
                arthropods[index].transform.up = Vector2.Lerp(arthropods[index].transform.up, transform.up, Vector2.Distance(arthropods[index].position, arthropodTargetPoses[index]) / Vector2.Distance(pStart, arthropodTargetPoses[index]));
            yield return new WaitForEndOfFrame();
        }
    }

    public void DirectionlyAdjustArthropods()
    {
        for (int i = 0; i < arthropods.Length; i++)
        {
            arthropodCurrentTargetPoses[i] = arthropods[i].position = arthropodTargetPoses[i] = transform.position + (arthropodIsRotate ? (transform.right * arthropodInitializedPoses[i].normalized.x + transform.up * arthropodInitializedPoses[i].normalized.y) * arthropodsLength : arthropodInitializedPoses[i].normalized * arthropodsLength);
            adjustTimes[i] = Random.Range(adjustTimeMin, adjustTimeMax);
        }
    }
}

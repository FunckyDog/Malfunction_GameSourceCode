using System.Collections;
using UnityEngine;

public class Spider_like_MovingArthropod : MonoBehaviour
{
    public Rigidbody2D rb;
    [Tooltip("��֫")] public Transform[] arthropods;
    [Tooltip("��֫ԭ��λ��")] public Vector2[] arthropodInitializedPoses;
    [Tooltip("��֫Ԥ����")] public GameObject arthropodPrefab;

    [Header("��֫����")]
    [Tooltip("��֫����")] public float arthropodsLength;
    [Tooltip("��������")] public float adjustDistance;
    [Tooltip("�����ٶ�")] public float adjustSpeed;
    [Tooltip("��������Y��ƫ����")] public float curveYOffet;
    [Tooltip("�����ȴ�ʱ��")] public float adjustTimeMin, adjustTimeMax;

    [Tooltip("��֫�Ƿ����ת")] public bool arthropodIsRotate;
    [Tooltip("��ת�Ƿ�Ӱ���֫����")] public bool arthropodDirIsRotate;

    private Vector2[] arthropodTargetPoses;//��֫Ŀ��λ��
    private Vector2[] arthropodCurrentTargetPoses;//��֫��ǰĿ��λ��
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

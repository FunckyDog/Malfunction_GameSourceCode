using UnityEngine;

public class Supply : MonoBehaviour
{
    public float suppleMul;

    public float rotateAniSpeed;

    public GameObject particlePrefab;

    private Fake3DStacker stacker;
    private float x;
    [HideInInspector] public GameObject _particle;

    private void Awake()
    {
        stacker = GetComponent<Fake3DStacker>();
    }

    private void OnDisable()
    {
        if (_particle != null)
            _particle?.GetComponent<PushParticleInPool>().Stop();
    }

    private void Start()
    {
        stacker.StackObject();
    }

    private void Update()
    {
        x += Time.deltaTime;

        if (x > 2 * Mathf.PI)
            x = 0;

        stacker.spriteTransRight = new Vector2(Mathf.Cos(rotateAniSpeed * x), Mathf.Sin(rotateAniSpeed * x));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.parent.name == "///Player")
        {
            GameManager.instance.consumpteAttribute._currentValue += Mathf.Sign(GameManager.instance.consumpteAttribute.consumptionValue) * (GameManager.instance.consumpteAttribute.maxValue - GameManager.instance.consumpteAttribute._currentValue) * suppleMul;
            MusicController.instance.GetComponent<SoundEffectPlayer>().PlaySoundEffect(0);
            PoolManager.instance.PushObject(gameObject);
        }
    }
}

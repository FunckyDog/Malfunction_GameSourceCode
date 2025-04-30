using DG.Tweening;
using System.Collections;
using System.Linq;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>, IGetHurt
{
    [Header("×´Ì¬")]
    [SerializeField] public Vector3 mousePos;
    public bool allowInput;
    public bool isWall;//ÊÇ·ñÅö±Ú
    public bool isHurt;
    public bool isDead;

    [Header("ÊôÐÔ")]
    public AttributeData_SO health;
    public AttributeData_SO speed;
    public Transform bodyTrans, feetTrans;
    public PlayerWeapon weapon;
    public AttributeData_SO invincibilityTime;
    public float blinkSpeed;
    public int offsetDir;

    [Header("¼ì²â")]
    public Vector2 wallCheckPos;
    public Vector2 wallCheckScale;
    public LayerMask groundLayer;
    public BoxCollider2D hurtArea, bodyArea;

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Vector2 movement;
    private int feetDir;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        //Refresh();//HACK:²âÊÔÓÃ£¬¼ÇµÃÉ¾
        //Dead();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green - new Color(0, 0, 0, 0.5f);
        Gizmos.DrawCube((Vector2)transform.position + wallCheckPos * movement, wallCheckScale);//Åö±Ú¼ì²âÇøÓò
    }

    private void Update()
    {
        if (allowInput)
            GetInput();
    }

    private void FixedUpdate()
    {
        Wallcheck();

        Move();
        Direction();
    }

    private void GetInput()
    {
        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetMouseButton(0))
            StartCoroutine(weapon.Shoot());

        if (Input.GetMouseButtonDown(1))
            GetComponentInChildren<SoundEffectPlayer>().PlaySoundEffect(3, 0.5f);
        else if (Input.GetMouseButton(1))
            weapon.Charge();
        else if (Input.GetMouseButtonUp(1))
            StartCoroutine(weapon.ChargeShoot());

        mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
    }

    public void UnGetInput()
    {
        allowInput = false;
        movement = Vector2.zero;
        Move();
    }

    void Direction()
    {
        if (movement.x != 0)
        {
            if (movement.x > 0)
                feetDir = 1;
            else
                feetDir = -1;
        }
        if (feetDir != 0)
            feetTrans.localScale = new Vector3(Mathf.Abs(feetTrans.lossyScale.x) * feetDir, feetTrans.lossyScale.y, 1);

        PlayerAnimation.instance.bodyStacker.spriteTransRight = (mousePos - transform.position).normalized;

        CameraController.instance.followTargetTrans.position = (Vector2)transform.position + Vector2.ClampMagnitude(mousePos - transform.position, 2);
    }

    private void Move()
    {
        if (movement != Vector2.zero)
        {
            speed.SetValue(speed.consumptionValue * Time.deltaTime);

            rb.velocity = movement * speed._currentValue;
        }
    }

    public void GetHurt(Vector2 bulletPos, float force, int damage)
    {
        hurtArea.enabled = false;
        UnGetInput();
        if (GameManager.instance.consumpteAttribute != health)
            health._currentValue -= damage;
        rb.AddForce(((Vector2)transform.position - bulletPos).normalized * force, ForceMode2D.Impulse);

        if (GameManager.instance.consumpteAttribute)
        {
            GameManager.instance.consumpteAttribute.SetValue(Mathf.Sign(GameManager.instance.consumpteAttribute.consumptionValue) *
                (Mathf.Sign(GameManager.instance.consumpteAttribute.consumptionValue) >= 0 ? GameManager.instance.consumpteAttribute._currentValue - GameManager.instance.consumpteAttribute.minValue
                : GameManager.instance.consumpteAttribute.minValue + GameManager.instance.consumpteAttribute.maxValue - GameManager.instance.consumpteAttribute._currentValue)
                * damage / 10);

            if (Mathf.InverseLerp(GameManager.instance.consumpteAttribute.minValue, GameManager.instance.consumpteAttribute.maxValue, GameManager.instance.consumpteAttribute._currentValue) > 0)
                StartCoroutine(Hurt());

            else
                Dead();
        }

        PlayerAnimation.instance.hurtParticle.Play();
        CameraController.instance.playerHurtCIS.GenerateImpulse(((Vector2)transform.position - bulletPos).normalized * 0.25f);
        VolumeController.instance.ChromaticAberrationShake(1, 2f);
    }

    IEnumerator Hurt()
    {
        isHurt = true;
        GetComponentInChildren<SoundEffectPlayer>().PlaySoundEffect(0, 0.5f);

        yield return new WaitForSeconds(0.5f);
        allowInput = true;
        isHurt = false;

        for (int i = 0; i < invincibilityTime._currentValue / blinkSpeed; i++)
        {
            PlayerAnimation.instance.feetSR.enabled = !PlayerAnimation.instance.feetSR.enabled;
            PlayerAnimation.instance.eyesSR.enabled = PlayerAnimation.instance.feetSR.enabled;
            PlayerAnimation.instance.bodySpriteTrans.gameObject.SetActive(PlayerAnimation.instance.feetSR.enabled);
            invincibilityTime.SetValue(invincibilityTime.consumptionValue);
            yield return new WaitForSeconds(blinkSpeed);
        }

        hurtArea.enabled = true;
        PlayerAnimation.instance.feetSR.enabled = PlayerAnimation.instance.eyesSR.enabled = true;
        PlayerAnimation.instance.bodySpriteTrans.gameObject.SetActive(true);
    }

    public void Fall()
    {
        UnGetInput();
        hurtArea.enabled = false;
        transform.DOScale(Vector3.zero, 0.5f);
        transform.DOMove((Vector2)transform.position + Vector2.down * 1.5f, 0.5f);

        GetComponentInChildren<SoundEffectPlayer>().PlaySoundEffect(1, 0.5f);
    }

    private void Dead()
    {
        isDead = true;
        UnGetInput();

        GameManager.instance.currentLevelData = null;
        GameManager.instance.currentLevelCount = Mathf.Clamp(GameManager.instance.currentLevelCount - 2, 0, GameManager.instance.currentLevelCount);
        GameManager.instance.isAllowGrindEnemy = false;

        for (int i = 0; i < 2 && GameManager.instance.extractedLevelData.Count > 0; i++)
            GameManager.instance.ReturnExtractedLevelDataToGroups(GameManager.instance.extractedLevelData.Last());
        StartCoroutine(UIManager.instance.FadeScene());
    }

    void Wallcheck()
    {
        isWall = Physics2D.OverlapBox((Vector2)transform.position + wallCheckPos * movement, wallCheckScale, 0, groundLayer);
    }//Åö±Ú¼ì²â

    public void Refresh()
    {
        isDead = false;
        GameManager.instance.InitializeAttributes();
        GameManager.instance.leftOffsetAttribute = GameManager.instance.rightOffsetAttribute = GameManager.instance.consumpteAttribute = null;
        hurtArea.enabled = true;
        offsetDir = 1;
        allowInput = true;
    }
}

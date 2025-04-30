using UnityEngine;

public class PlayerAnimation : Singleton<PlayerAnimation>
{
    public Animator feetAnim;
    public Transform bodySpriteTrans;
    public SpriteRenderer feetSR, eyesSR;
    public Transform firePosTrans, shootParticleTrans;
    public ParticleSystem nearDeadParticle, hurtParticle, moveParticle, chargeParticle;

    public Animator eyesAnim => GetComponent<Fake3DAnimationControler>().eyesAnim;
    [HideInInspector] public Fake3DStacker bodyStacker;

    protected override void Awake()
    {
        base.Awake();
        bodyStacker = GetComponent<Fake3DStacker>();
    }

    private void Start()
    {
        bodyStacker.StackObject();
        firePosTrans.SetParent(bodySpriteTrans.GetChild(6));
        shootParticleTrans.SetParent(bodySpriteTrans.GetChild(6));
        shootParticleTrans.localPosition = firePosTrans.localPosition = new Vector2(0.7f, -0.5f);
    }

    private void Update()
    {
        AnimationSwitch();

        feetSR.transform.localScale = new Vector3(Mathf.Sign(bodyStacker.spriteTransRight.x), 1, 1);

        if (GameManager.instance.consumpteAttribute)
        {
            if (Mathf.InverseLerp(GameManager.instance.consumpteAttribute.minValue, GameManager.instance.consumpteAttribute.maxValue, PlayerController.instance.health._currentValue) < 0.25f)
                nearDeadParticle.Play();

            else if (Mathf.InverseLerp(GameManager.instance.consumpteAttribute.minValue, GameManager.instance.consumpteAttribute.maxValue, PlayerController.instance.health._currentValue) >= 0.25f && nearDeadParticle.isPlaying)
                nearDeadParticle.Stop();
        }

        if (PlayerController.instance.weapon.chargeWaitTime != 0 && chargeParticle.isStopped)
            chargeParticle.Play();

        if (PlayerController.instance.weapon.chargeWaitTime ==0 && !Input.GetMouseButton(1))
            chargeParticle.Stop();

        if (PlayerController.instance.movement != Vector2.zero && moveParticle.isStopped)
            moveParticle.Play();

        else if (PlayerController.instance.movement == Vector2.zero)
            moveParticle.Stop();
    }

    public void ChangeLayer(string layerName)
    {
        for (int i = 0; i < bodySpriteTrans.childCount; i++)
            bodySpriteTrans.GetChild(i).GetComponent<SpriteRenderer>().sortingLayerName = layerName;
        feetSR.sortingLayerName = eyesSR.sortingLayerName = layerName;
    }

    void AnimationSwitch()
    {
        feetAnim.SetBool("Move", PlayerController.instance.rb.velocity != Vector2.zero);
        eyesAnim.SetBool("Dead", PlayerController.instance.isDead);
        eyesAnim.SetBool("Hurt", PlayerController.instance.isHurt);
        feetAnim.SetBool("Hurt", PlayerController.instance.isHurt);
        feetAnim.SetFloat("MoveSpeed", PlayerController.instance.rb.velocity.magnitude);
    }

}

using UnityEngine.Animations;
using UnityEngine;

public class Fake3DAnimationControler : MonoBehaviour
{
    public Fake3DStacker stacker;
    public Animator eyesAnim;
    public RuntimeAnimatorController frontEyesAC;
    public AnimatorOverrideController SideEyesAC;
    public float sideAngle, activeAngle;

    private void Update()
    {
        AnimationSwitch();
    }

    void AnimationSwitch()
    {
        eyesAnim.transform.localScale = new Vector3(Mathf.Sign(stacker.spriteTransRight.x), 1, 1);

        eyesAnim.runtimeAnimatorController = Vector2.Angle(new Vector2(Mathf.Abs(stacker.spriteTransRight.x), stacker.spriteTransRight.y), -transform.up) >= sideAngle ? SideEyesAC : frontEyesAC;
        eyesAnim.GetComponent<SpriteRenderer>().enabled = Vector2.Angle(new Vector2(Mathf.Abs(stacker.spriteTransRight.x), stacker.spriteTransRight.y), -transform.up) <= activeAngle;
    }
}

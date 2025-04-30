using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    public Animator eyesAnim => GetComponent<Fake3DAnimationControler>().eyesAnim;
    [HideInInspector] public Fake3DStacker bodyStacker;

    public GameObject deadParticlePrefab;

    private void Awake()
    {
        bodyStacker = GetComponent<Fake3DStacker>();
    }

    protected virtual void Start()
    {
        bodyStacker.StackObject();
    }

    private void Update()
    {
        AnimationSwitch();
    }

    protected virtual void AnimationSwitch()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ValueImpulselySwitch : MonoBehaviour
{
    public float targetValue;
    public float switchTime;

    protected float currentValue;

    private Light2D light2d;

    private void Awake()
    {
        light2d = GetComponent<Light2D>();
    }

    private void OnEnable()
    {
        currentValue = light2d.pointLightOuterRadius;
    }

    private void Update()
    {
        if (Mathf.Abs(currentValue - targetValue) >= 0.1f)
        {
            if (targetValue > currentValue)
                currentValue += Time.deltaTime * switchTime;
            else
                currentValue -= Time.deltaTime * switchTime;

            light2d.pointLightOuterRadius = light2d.pointLightInnerRadius = Random.Range(currentValue - 0.1f, currentValue + 0.1f);

        }

        else
            PoolManager.instance.PushObject(gameObject);
    }

}

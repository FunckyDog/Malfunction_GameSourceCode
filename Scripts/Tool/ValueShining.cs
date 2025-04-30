using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ValueShining : MonoBehaviour
{
    public float minShiningInterval, maxShiningInterval, minIntensity, maxIntensity,shiningTime;

    private float randomShiningInterval, shiningWaitTime, initializeValue;

    private void Awake()
    {
        randomShiningInterval = Random.Range(minShiningInterval, maxShiningInterval);
        initializeValue = GetComponent<Light2D>().pointLightOuterRadius;
    }

    private void Update()
    {
        shiningWaitTime += Time.deltaTime;

        if (shiningWaitTime >= randomShiningInterval)
            GetComponent<Light2D>().pointLightOuterRadius = GetComponent<Light2D>().pointLightInnerRadius =  Random.Range(minIntensity, maxIntensity);

        if (shiningWaitTime >= randomShiningInterval + shiningTime)
        {
            shiningWaitTime = 0;
            GetComponent<Light2D>().pointLightOuterRadius = GetComponent<Light2D>().pointLightInnerRadius = initializeValue; 
            randomShiningInterval = Random.Range(minShiningInterval, maxShiningInterval);
        }
    }
}

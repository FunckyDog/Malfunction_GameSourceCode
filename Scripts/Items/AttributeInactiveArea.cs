using UnityEngine;
using UnityEngine.UI;

public class AttributeInactiveArea : MonoBehaviour
{
    public bool isInactiveLeftAttribute;
    public float attributeconsumptionValue;

    private Vector2 areaSize;
    private ParticleSystem partice;
    ParticleSystem.MainModule mainModule;
    ParticleSystem.EmissionModule emissionModule;
    ParticleSystem.ShapeModule shapeModule;

    private void Awake()
    {
        partice = GetComponentInChildren<ParticleSystem>();
        areaSize = transform.lossyScale;
        mainModule = partice.main;
        emissionModule = partice.emission;
        shapeModule = partice.shape;

        mainModule.startColor = isInactiveLeftAttribute ? GameManager.instance.leftOffsetAttribute.displayColor : GameManager.instance.rightOffsetAttribute.displayColor;
        emissionModule.rateOverTime = areaSize.x;
        shapeModule.scale = areaSize;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Body Area")
        {
            attributeconsumptionValue = isInactiveLeftAttribute ? GameManager.instance.leftOffsetAttribute.consumptionValue : GameManager.instance.rightOffsetAttribute.consumptionValue;
            UIManager.instance.offsetBarRectTrans.GetComponentInChildren<Image>().color = isInactiveLeftAttribute ? GameManager.instance.rightOffsetAttribute.displayColor : GameManager.instance.leftOffsetAttribute.displayColor;
            if (isInactiveLeftAttribute)
                GameManager.instance.leftOffsetAttribute.consumptionValue = 0;
            else
                GameManager.instance.rightOffsetAttribute.consumptionValue = 0;

            CameraController.instance.attributeAreaCIS.GenerateImpulse();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "Body Area")
        {
            if (isInactiveLeftAttribute)
                GameManager.instance.leftOffsetAttribute.consumptionValue = attributeconsumptionValue;
            else
                GameManager.instance.rightOffsetAttribute.consumptionValue = attributeconsumptionValue;

            UIManager.instance.offsetBarRectTrans.GetComponentInChildren<Image>().color = Color.white;
        }
    }
}

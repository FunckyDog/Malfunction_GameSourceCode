using UnityEngine;

public class AttributeReversalArea : MonoBehaviour
{
    private Vector2 areaSize;
    private ParticleSystem partice;
    ParticleSystem.EmissionModule emissionModule;
    ParticleSystem.ShapeModule shapeModule;

    private void Awake()
    {
        partice = GetComponentInChildren<ParticleSystem>();
        areaSize = transform.lossyScale;
        emissionModule = partice.emission;
        shapeModule = partice.shape;

        emissionModule.rateOverTime = areaSize.x;
        shapeModule.scale = areaSize;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Body Area")
        {
            PlayerController.instance.offsetDir *= -1;
            UIManager.instance.leftAtrributeText.color = GameManager.instance.rightOffsetAttribute.displayColor;
            UIManager.instance.rightAtrributeText.color = GameManager.instance.leftOffsetAttribute.displayColor;

            CameraController.instance.attributeAreaCIS.GenerateImpulse();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "Body Area")
        {
            PlayerController.instance.offsetDir *= -1;
            UIManager.instance.leftAtrributeText.color = UIManager.instance.rightAtrributeText.color = Color.black;
        }
    }
}

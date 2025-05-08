using UnityEngine;

[CreateAssetMenu(fileName = "New Attribute Data", menuName = "Data/Attribute Data")]
public class AttributeData_SO : ScriptableObject
{
    public string attributeName;
    public int textSize;
    public float minValue, maxValue, consumptionValue;
    public Color displayColor;

    [SerializeField] private float currentValue;

    public float _currentValue
    {
        get
        { return currentValue; }

        set
        {
            currentValue = Mathf.Clamp(value, minValue, maxValue);
        }
    }

    public void InitializeValue()
    {
        if (GameManager.instance.consumpteAttribute == this)
        {
            _currentValue = Mathf.Sign(consumptionValue) >= 0 ? maxValue : minValue;
        }

        else if (GameManager.instance.leftOffsetAttribute == this)
        {
            _currentValue = (maxValue + minValue) / 2;
            GameManager.instance.rightOffsetAttribute._currentValue = Mathf.Lerp(GameManager.instance.rightOffsetAttribute.minValue, GameManager.instance.rightOffsetAttribute.maxValue, 0.5f);
        }

        else
        {
            _currentValue = (maxValue + minValue) / 2;
        }

    }

    public void SetValue(float value)
    {
        value *= PlayerController.instance.offsetDir;

        if (GameManager.instance.leftOffsetAttribute == this || GameManager.instance.rightOffsetAttribute == this)
            _currentValue -= value;

        if (GameManager.instance.leftOffsetAttribute && GameManager.instance.rightOffsetAttribute)
        {
            if (GameManager.instance.leftOffsetAttribute.consumptionValue * GameManager.instance.rightOffsetAttribute.consumptionValue < 0)
            {
                if (GameManager.instance.leftOffsetAttribute == this)
                    GameManager.instance.rightOffsetAttribute._currentValue = Mathf.Lerp(GameManager.instance.rightOffsetAttribute.minValue, GameManager.instance.rightOffsetAttribute.maxValue, Mathf.InverseLerp(minValue, maxValue, _currentValue));
                if (GameManager.instance.rightOffsetAttribute == this)
                    GameManager.instance.leftOffsetAttribute._currentValue = Mathf.Lerp(GameManager.instance.leftOffsetAttribute.minValue, GameManager.instance.leftOffsetAttribute.maxValue, Mathf.InverseLerp(minValue, maxValue, _currentValue));
            }

            else
            {
                if (GameManager.instance.leftOffsetAttribute == this)
                    GameManager.instance.rightOffsetAttribute._currentValue = Mathf.Lerp(GameManager.instance.rightOffsetAttribute.minValue, GameManager.instance.rightOffsetAttribute.maxValue, 1 - Mathf.InverseLerp(minValue, maxValue, _currentValue));
                if (GameManager.instance.rightOffsetAttribute == this)
                    GameManager.instance.leftOffsetAttribute._currentValue = Mathf.Lerp(GameManager.instance.leftOffsetAttribute.minValue, GameManager.instance.leftOffsetAttribute.maxValue, 1 - Mathf.InverseLerp(minValue, maxValue, _currentValue));
            }
        }
    }
}

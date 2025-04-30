using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fake3DStacker : MonoBehaviour
{
    public Sprite[] sprites;
    public string layerName;
    [Tooltip("Sprite图片的Y轴间隔")] public float spriteIntervalY;
    public float spriteRoation;
    public Vector2 spriteTransRight;
    public Material spriteMat;
    public Color spriteColor;

    private List<GameObject> spriteObjects = new List<GameObject>();
    private void Awake()
    {
        if (spriteIntervalY == 0)
            spriteIntervalY = 1f / (sprites.Length - 1f);
    }

    private void Update()
    {
        RotateObject();

        //AdjustYInterval();//HACK:测试用，记得删
    }

    public void StackObject()
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            GameObject _spriteObject = new GameObject(sprites[i].name);
            SpriteRenderer _SR = _spriteObject.AddComponent<SpriteRenderer>();
            _SR.sprite = sprites[i];
            _SR.material = spriteMat;
            _SR.sortingLayerName = layerName;
            _SR.sortingOrder = i;
            _spriteObject.transform.SetParent(transform);
            spriteObjects.Add(_spriteObject);

            if (sprites.Length % 2 == 0)
                _spriteObject.transform.localPosition = Vector2.up * (i - (sprites.Length - 1) / 2 - 0.5f) * spriteIntervalY;
            else
                _spriteObject.transform.localPosition = Vector2.up * (i - (sprites.Length - 1) / 2) * spriteIntervalY;
        }
    }

    void RotateObject()
    {
        //for (int i = 0; i < spriteObjects.Count; i++)
        //{
        //    spriteObjects[i].transform.localRotation = Quaternion.Euler(0, 0, spriteRoation);
        //}

        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).right = spriteTransRight;
    }

    void AdjustYInterval()
    {
        for (int i = 0; i < spriteObjects.Count; i++)
        {
            if (sprites.Length % 2 == 0)
                spriteObjects[i].transform.localPosition = Vector2.up * (i - (sprites.Length - 1) / 2 - 0.5f) * spriteIntervalY;
            else
                spriteObjects[i].transform.localPosition = Vector2.up * (i - (sprites.Length - 1) / 2) * spriteIntervalY;
        }
    }

    public void ObjectColor(Color color)
    {
        foreach (var sprite in spriteObjects)
            sprite.GetComponent<SpriteRenderer>().color = color;
    }
}

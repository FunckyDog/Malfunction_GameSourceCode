using UnityEngine;

public class Confiner : Singleton<Confiner>
{
    public void SetConfiner()
    {
        CameraController.instance.CC.m_BoundingShape2D = GetComponent<PolygonCollider2D>();
    }
}

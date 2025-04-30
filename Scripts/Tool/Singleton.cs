using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T Instance;

    public static T instance
    { get { return Instance; } }

    public static bool isInitialized
    { get { return Instance != null; } }


    protected virtual void Awake()
    {
        if (Instance == null)
            Instance = (T)this;
        else
            Destroy(gameObject);
    }


    protected virtual void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}

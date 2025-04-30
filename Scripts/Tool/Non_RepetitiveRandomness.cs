using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Non_RepetitiveRandomness<T>
{
    public List<T> objects;
    [HideInInspector] public List<T> currentObjects;

    public T RandomExtract()
    {
        if (currentObjects.Count == 0)
            foreach (var obj in objects)
                currentObjects.Add(obj);

        T _randomObject = currentObjects[Random.Range(0, currentObjects.Count)];
        currentObjects.Remove(_randomObject);

        return _randomObject;
    }

    public void ReBackObjectToList(T obj)
    {
        if (objects.Contains(obj) && !currentObjects.Contains(obj))
            currentObjects.Add(obj);
    }
}

using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    private Dictionary<string, Queue<GameObject>> poolDict = new Dictionary<string, Queue<GameObject>>();

    private GameObject poolsParent;//所有池的父物体

    [Header("对象池数据库")]
    public List<PoolData> poolDataList = new List<PoolData>();

    protected override void Awake()
    {
        base.Awake();
        poolsParent = new GameObject("//PoolsParent");
        SetPools();
    }

    private void OnEnable()
    {
        EventsHandler.BeforeSceneLoad += PushAllObject;
    }

    private void OnDisable()
    {
        EventsHandler.BeforeSceneLoad -= PushAllObject;
    }

    public void SetPools()
    {
        for (int i = 0; i < poolDataList.Count; i++)
        {
            if (!poolDict.ContainsKey(poolDataList[i].prefab.name))
                InitializePool(poolDataList[i]);
        }
    }

    public PoolData GetPoolData(GameObject prefab)
    {
        return poolDataList.Find(p => p.prefab == prefab);
    }

    void InitializePool(PoolData poolData)
    {

        Transform poolParent = new GameObject("#Poll : " + poolData.prefab.name).transform;
        poolParent.SetParent(poolsParent.transform);

        if (!poolDict.ContainsKey(poolData.prefab.name))
            poolDict.Add(poolData.prefab.name, new Queue<GameObject>());

        for (int i = 0; i < poolData.poolSize; i++)
        {
            GameObject objectInPool = Instantiate(poolData.prefab, poolParent);
            objectInPool.transform.SetParent(poolParent.transform);
            PushObject(objectInPool);
        }
    }

    public GameObject GetObject(GameObject prefab)
    {
        GameObject _object;
        PoolData poolData = GetPoolData(prefab);

        if (poolData == null)
        {
            InitializePool(poolData);
            GetObject(prefab);
        }

        if (poolDict[poolData.prefab.name].Count == 0)
        {
            _object = GameObject.Instantiate(poolData.prefab);
            PushObject(_object);
        }

        _object = poolDict[poolData.prefab.name].Dequeue();
        _object.SetActive(true);
        return _object;
    }

    public void PushObject(GameObject gameObject)
    {
        string _name = gameObject.name.Replace("(Clone)", "");
        if (!poolDict.ContainsKey(_name))
            poolDict.Add(_name, new Queue<GameObject>());

        else
        {
            if (poolDict[_name].Contains(gameObject))
                return;

            else
            {
                Transform poolParent = GameObject.Find("#Poll : " + _name).transform;
                gameObject.transform.SetParent(poolParent);

                poolDict[_name].Enqueue(gameObject);
                gameObject.SetActive(false);
            }
        }
    }

    public void PushAllObject()
    {
        for (int i = 0; i < poolsParent.transform.childCount; i++)
            for (int j = 0; j < poolsParent.transform.GetChild(i).childCount; j++)
                    PushObject(poolsParent.transform.GetChild(i).GetChild(j).gameObject);
    }
}

[System.Serializable]
public class PoolData
{
    public GameObject prefab;
    public int poolSize;
}

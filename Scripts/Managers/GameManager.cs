using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("观测数据")]
    public int currentLevelCount;
    public LevelData_SO currentLevelData;
    public WaveGrindEnemyData_SO currentGrindEnemyData;
    public AttributeData_SO leftOffsetAttribute, rightOffsetAttribute, consumpteAttribute;
    public int currentWaveCount;
    public int grindEnemyCountInCurrentWave;
    public bool isAllowGrindEnemy;
    public List<LevelData_SO> extractedLevelData = new List<LevelData_SO>();

    [Header("属性")]
    public float supplyProbability;
    public Non_RepetitiveRandomness<AttributeData_SO> attributeGroup;
    public int[] difficultyRatingByWaveCount;
    public Non_RepetitiveRandomness<LevelData_SO> easyLevelDataGroup, midddleLevelDataGroup, hardLevelDataGroup;

    [Header("效果")]
    public GameObject grindEnemyPointPrefab;

    private void Start()
    {
        //RandomAttribute();//HACK:测试用，记得删

        //InitializeAttributes();
        Restart();
    }

    private void OnDrawGizmos()
    {
        if (currentGrindEnemyData)
        {
            foreach (GrindEnemyInfo info in currentGrindEnemyData.grindEnemyInfos)
            {
                Gizmos.color = Color.red - new Color(0, 0, 0, 0.5f);
                Gizmos.DrawWireSphere((Vector2)transform.position + info.grindPos, 0.5f);
            }
        }
    }

    private void Update()
    {
        if (currentLevelData && isAllowGrindEnemy && grindEnemyCountInCurrentWave == 0)
        {
            currentWaveCount++;
            if (currentWaveCount == currentLevelData.waveGrindEnemyDatas.Length + 1)
            {
                EventsHandler.CallLevelFinished();
                isAllowGrindEnemy = false;
            }

            else
                StartCoroutine(GrindEnemy());
        }
    }

    public void RandomLevel()
    {
        for (int i = 0; i < difficultyRatingByWaveCount.Length - 1; i++)
        {
            if (currentLevelCount >= difficultyRatingByWaveCount[i] && currentLevelCount < difficultyRatingByWaveCount[i + 1])
                switch (i)
                {
                    case 0:
                        currentLevelData = easyLevelDataGroup.RandomExtract();
                        break;
                    case 1:
                        currentLevelData = midddleLevelDataGroup.RandomExtract();
                        break;
                }
        }
        if (currentLevelCount >= difficultyRatingByWaveCount.Last())
            currentLevelData = hardLevelDataGroup.RandomExtract();

        extractedLevelData.Add(currentLevelData);
        currentWaveCount = 0;
        grindEnemyCountInCurrentWave = 0;
    }

    public void RandomAttribute()
    {
        consumpteAttribute = attributeGroup.RandomExtract();
        leftOffsetAttribute = attributeGroup.RandomExtract();
        rightOffsetAttribute = attributeGroup.RandomExtract();
        attributeGroup.currentObjects.Clear();

        supplyProbability = Mathf.Clamp(0.8f - currentLevelCount * 0.05f, 0.3f, 0.8f);
    }

    public void InitializeAttributes()
    {
        foreach (AttributeData_SO attribute in attributeGroup.objects)
            attribute.InitializeValue();
    }

    IEnumerator GrindEnemy()
    {
        grindEnemyCountInCurrentWave = currentLevelData.waveGrindEnemyDatas[currentWaveCount - 1].grindEnemyInfos.Length;

        yield return new WaitForSeconds(1);

        foreach (GrindEnemyInfo info in currentLevelData.waveGrindEnemyDatas[currentWaveCount - 1].grindEnemyInfos)
            PoolManager.instance.GetObject(grindEnemyPointPrefab).transform.position = info.grindPos;

        yield return new WaitForSeconds(grindEnemyPointPrefab.GetComponent<GrindEnemyPoint>().lifeTime);

        foreach (GrindEnemyInfo info in currentLevelData.waveGrindEnemyDatas[currentWaveCount - 1].grindEnemyInfos)
        {
            GameObject _enemy = PoolManager.instance.GetObject(info.enemyPrefab);
            _enemy.transform.position = info.grindPos;
            _enemy.GetComponentInChildren<Spider_like_MovingArthropod>()?.DirectionlyAdjustArthropods();
        }
    }

    public void ReturnExtractedLevelDataToGroups(LevelData_SO levelData)
    {
        easyLevelDataGroup.ReBackObjectToList(levelData);
        midddleLevelDataGroup.ReBackObjectToList(levelData);
        hardLevelDataGroup.ReBackObjectToList(levelData);
        extractedLevelData.Remove(levelData);
    }

    public void Restart()
    {
        instance.currentLevelCount = 0;
        LoadManager.instance.SceneLoadAction(19);
        UIManager.instance.gamFinishedPanelCG.alpha = 0;
        UIManager.instance.mainMenuPanelObject.SetActive(true);
        UIManager.instance.gamePanelAnim.gameObject.SetActive(false);
        UIManager.instance.pauseButton.gameObject.SetActive(false);
        UIManager.instance.HTPButton.gameObject.SetActive(false);
        CameraController.instance.followTargetTrans.position = PlayerController.instance.transform.position = LoadManager.instance.firstPlayerLoadPos;
        EventsHandler.CallLevelFinished();
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "New Wave Grind Enemy Data", menuName = "Data/Wave Grind Enemy Data")]
public class WaveGrindEnemyData_SO : ScriptableObject
{
    public GrindEnemyInfo[] grindEnemyInfos;
}

[System.Serializable]
public class GrindEnemyInfo
{
    public Vector2 grindPos;
    public GameObject enemyPrefab;
}

using UnityEngine;

[CreateAssetMenu(fileName = "New Level Data", menuName = "Data/Level Data")]
public class LevelData_SO : ScriptableObject
{
    public int sceneIndex;
    public Vector2 exitPosBeforeEnterScene, exitPosAfterLeaveScene;
    public WaveGrindEnemyData_SO[] waveGrindEnemyDatas;
}

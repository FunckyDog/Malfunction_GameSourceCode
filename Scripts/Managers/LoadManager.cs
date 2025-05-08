using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManager : Singleton<LoadManager>
{
    public int currentSceneIndex;
    public LevelData_SO firstSceneData;

    public Vector2 firstPlayerLoadPos;

    private void Start()
    {

    }

    public void SceneLoadAction(int sceneIndex)
    {
        StartCoroutine(UnloadScene(sceneIndex));
    }
    public void LevelLoadAction(LevelData_SO levelData) => StartCoroutine(UnloadScene(levelData.sceneIndex));

    public IEnumerator UnloadScene(int sceneIndex)
    {
        if (currentSceneIndex != 0)
        {
            yield return SceneManager.UnloadSceneAsync(currentSceneIndex);
        }

        EventsHandler.CallBeforeSceneLoad();

        yield return SceneLoad(sceneIndex);

    }//卸载场景的协程

    IEnumerator SceneLoad(int sceneIndex)
    {
        yield return SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);

        currentSceneIndex = sceneIndex;

        EventsHandler.CallAfterSceneLoad();

    }//加载场景的协程
}

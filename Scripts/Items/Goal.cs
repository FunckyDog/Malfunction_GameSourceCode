using DG.Tweening;
using UnityEngine;

public class Goal : Singleton<Goal>
{
    private void Update()
    {
        if (UIManager.instance.gamFinishedPanelCG.alpha == 1 && Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.instance.gamFinishedPanelCG.alpha = 0;
            LoadManager.instance.SceneLoadAction(LoadManager.instance.firstSceneData.sceneIndex);
            UIManager.instance.mainMenuPanelObject.SetActive(true);
            UIManager.instance.gamePanelAnim.gameObject.SetActive(false);
            UIManager.instance.titleText.gameObject.SetActive(true);
            PlayerController.instance.transform.position = LoadManager.instance.firstPlayerLoadPos;
            GameManager.instance.currentLevelData = null;
            GameManager.instance.currentLevelCount = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController.instance.UnGetInput();
        UIManager.instance.allowPause = false;
        UIManager.instance.gamFinishedPanelCG.DOFade(1, 4);
        GetComponent<Collider2D>().enabled = false;
    }
}

using DG.Tweening;
using UnityEngine;

public class Goal : Singleton<Goal>
{
    private void Update()
    {
        if (UIManager.instance.gamFinishedPanelCG.alpha == 1 && Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.instance.Restart();
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

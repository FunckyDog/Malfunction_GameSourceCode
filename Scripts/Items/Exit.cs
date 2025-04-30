using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Exit : MonoBehaviour
{
    public Transform platfromTrans;

    private BoxCollider2D exitArea;
    private Rigidbody2D rb;
    private Animator anim;
    private ParticleSystem particle;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        exitArea = GetComponentInChildren<BoxCollider2D>();
        particle = GetComponentInChildren<ParticleSystem>();
        exitArea.enabled = false;
    }

    private void OnEnable()
    {
        EventsHandler.LevelFinished += ExitOpen;
        EventsHandler.BeforeSceneLoad += OnBeforeSceneLoad;
    }

    private void OnDisable()
    {
        EventsHandler.LevelFinished -= ExitOpen;
        EventsHandler.BeforeSceneLoad -= OnBeforeSceneLoad;
    }

    private void OnBeforeSceneLoad()
    {
        if (GameManager.instance.currentLevelData)
            platfromTrans.localPosition = GameManager.instance.currentLevelData.exitPosBeforeEnterScene;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Body Area" && collision.transform.parent.name == "///Player")
        {
            exitArea.enabled = false;
            PlayerController.instance.bodyArea.enabled = PlayerController.instance.hurtArea.enabled = false;
            GameManager.instance.currentLevelCount++;
            PlayerAnimation.instance.ChangeLayer("Back Scene");
            PlayerController.instance.UnGetInput();
            PlayerController.instance.transform.SetParent(platfromTrans);

            GetComponent<SoundEffectPlayer>().PlaySoundEffect(0);
            UIManager.instance.allowPause = false;
            CameraController.instance.CC.m_BoundingShape2D = null;
            CameraController.instance.CVC.Follow = platfromTrans;
            particle.Stop();
            CameraController.instance.exitCIS.GenerateImpulse();

            for (int i = 1; i < MusicController.instance.tracks.Length; i++)
                MusicController.instance.DescreaseMusic(i);

            platfromTrans.DOMove(GameManager.instance.currentLevelData.exitPosAfterLeaveScene, 5f).SetEase(Ease.InSine).OnComplete(() =>
            {
                if (GameManager.instance.currentLevelCount > 13)
                {
                    LoadManager.instance.SceneLoadAction(19);
                    GameManager.instance.consumpteAttribute = GameManager.instance.leftOffsetAttribute = GameManager.instance.rightOffsetAttribute = null;
                }

                else
                {
                    GameManager.instance.RandomAttribute();
                    GameManager.instance.InitializeAttributes();
                    GameManager.instance.RandomLevel();
                    LoadManager.instance.LevelLoadAction(GameManager.instance.currentLevelData);
                }
                StartCoroutine(EnterNextLevel());
            });
        }
    }

    IEnumerator EnterNextLevel()
    {
        if (GameManager.instance.currentLevelData.waveGrindEnemyDatas.Length != 0)
            yield return UIManager.instance.ExtractAttributeAni();

        PlayerAnimation.instance.ChangeLayer("Fore Scene");
        ChangeLayer("Fore Scene");

        platfromTrans.DOMove(Vector2.zero, 5f).SetEase(Ease.InSine).OnComplete(() =>
        {
            PlayerController.instance.bodyArea.enabled = PlayerController.instance.hurtArea.enabled = true;
            PlayerController.instance.allowInput = true;
            PlayerController.instance.transform.SetParent(null);
            ChangeLayer("Back Scene");
            PlayerAnimation.instance.ChangeLayer("Player");
            UIManager.instance.allowPause = true;

            anim.SetBool("Open", false);
            CameraController.instance.exitCIS.GenerateImpulse();
            CameraController.instance.CVC.Follow = CameraController.instance.followTargetTrans;
            Confiner.instance.SetConfiner();

            if (GameManager.instance.currentLevelData != LoadManager.instance.firstSceneData)
                UIManager.instance.LevelCountText("LEVEL " + GameManager.instance.currentLevelCount.ToString());
            else
            {
                UIManager.instance.LevelCountText("LEVEL 0");
                UIManager.instance.RecoverText();
                Goal.instance.GetComponent<Collider2D>().enabled = true;
            }

            GetComponent<SoundEffectPlayer>().PlaySoundEffect(0);
            Cursor.visible = true;
            Cursor.SetCursor(UIManager.instance.aimCursur, new Vector2(32, 32), CursorMode.Auto);
        });

        yield return new WaitForSeconds(7.5f);
        GameManager.instance.isAllowGrindEnemy = true;
    }

    void ExitOpen()
    {
        exitArea.enabled = true;
        anim.SetBool("Open", true);
        particle.Play();
        GetComponent<SoundEffectPlayer>().PlaySoundEffect(1);
    }

    void ChangeLayer(string layerName)
    {
        platfromTrans.GetComponent<SpriteRenderer>().sortingLayerName = layerName;
    }
}

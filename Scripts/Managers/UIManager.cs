using DG.Tweening;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public bool allowPause;

    [Header("UI物体")]
    public GameObject pausePanelObject;
    public GameObject mainMenuPanelObject, HTPPanelObject;
    public RectTransform offsetBarRectTrans;
    public Image leftConsumptionImage, rightConsumptionImage;
    public Texture2D simpleCursur, aimCursur;
    public Text consumptionAtrributeText, leftAtrributeText, rightAtrributeText, levelCountText, titleText;
    public CanvasGroup gamFinishedPanelCG;
    public Button pauseButton, HTPButton;


    [Header("动画器")]
    public Animator gamePanelAnim;
    public Animator pausePanelAnim;
    public Animator mainMenuPanelAnim;
    protected override void Awake()
    {
        base.Awake();
        //mainMenuPanelObject.SetActive(true);//HACK:测试用，记得改
        //gamePanelAnim.gameObject.SetActive(false);//HACK:测试用，记得改
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Start()
    {
        //StartCoroutine(ExtractAttributeAni());
        Cursor.SetCursor(simpleCursur, new Vector2(20, 4), CursorMode.Auto);
        RecoverText();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && mainMenuPanelObject.activeSelf && !mainMenuPanelAnim.GetBool("StartGame"))
            StartCoroutine(StartGame());

        if ((Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape)) && allowPause)
            PausePanel();

#if UNITY_STANDALONE
        if (pausePanelObject.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
#endif

        if (gamePanelAnim.gameObject.activeInHierarchy && !gamePanelAnim.GetBool("Extract"))
        {
            if (GameManager.instance.leftOffsetAttribute && GameManager.instance.rightOffsetAttribute)
                OffsetBar();
            if (GameManager.instance.consumpteAttribute)
                ConsumptionBar();
        }
    }

    public void TimeStop(GameObject UIObject)
    {
        if (UIObject.activeSelf)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    public void UISwitch(GameObject UIObject)
    {
        bool UIObjectActive = UIObject.activeSelf;
        UIObjectActive = !UIObjectActive;
        UIObject.SetActive(UIObjectActive);
        MusicController.instance.mixer.SetFloat("MasterVolume", UIObject.activeSelf ? -10 : 0);
    }

    public void PausePanel()
    {
        UISwitch(pausePanelObject);
        pausePanelAnim.SetBool("Open", pausePanelAnim.gameObject.activeSelf);

        TimeStop(pausePanelObject);

        if (pausePanelObject.activeSelf)
        {
            PlayerController.instance.UnGetInput();
            Cursor.SetCursor(simpleCursur, new Vector2(20, 4), CursorMode.Auto);

        }
        else
        {
            PlayerController.instance.allowInput = true;
            Cursor.SetCursor(aimCursur, new Vector2(32, 32), CursorMode.Auto);
        }
    }

    public void HTPPanel()
    {
        UISwitch(HTPPanelObject);
        TimeStop(HTPPanelObject);
        allowPause = !HTPPanelObject.activeSelf;
    }

    void OffsetBar()
    {
        float attributeValue = Mathf.Sign(GameManager.instance.leftOffsetAttribute.consumptionValue) > 0 ? GameManager.instance.leftOffsetAttribute._currentValue : GameManager.instance.leftOffsetAttribute.minValue + GameManager.instance.leftOffsetAttribute.maxValue - GameManager.instance.leftOffsetAttribute._currentValue;
        Vector3 pos = Vector2.right * Mathf.Lerp(-offsetBarRectTrans.rect.width / 2, offsetBarRectTrans.rect.width / 2, Mathf.InverseLerp(GameManager.instance.leftOffsetAttribute.minValue, GameManager.instance.leftOffsetAttribute.maxValue, attributeValue));
        offsetBarRectTrans.GetChild(0).localPosition = pos;
        offsetBarRectTrans.GetComponent<Image>().color = Color.Lerp(GameManager.instance.leftOffsetAttribute.displayColor, GameManager.instance.rightOffsetAttribute.displayColor, Mathf.InverseLerp(GameManager.instance.leftOffsetAttribute.minValue, GameManager.instance.leftOffsetAttribute.maxValue, attributeValue));
    }

    void ConsumptionBar()
    {
        float attributeValue = Mathf.Sign(GameManager.instance.consumpteAttribute.consumptionValue) > 0 ? GameManager.instance.consumpteAttribute._currentValue : GameManager.instance.consumpteAttribute.minValue + GameManager.instance.consumpteAttribute.maxValue - GameManager.instance.consumpteAttribute._currentValue;
        rightConsumptionImage.fillAmount = leftConsumptionImage.fillAmount = Mathf.InverseLerp(GameManager.instance.consumpteAttribute.minValue, GameManager.instance.consumpteAttribute.maxValue, attributeValue);
        rightConsumptionImage.color = leftConsumptionImage.color = Color.Lerp(Color.red, GameManager.instance.consumpteAttribute.displayColor, leftConsumptionImage.fillAmount);
    }

    public IEnumerator FadeScene()
    {
        allowPause = false;
        MusicController.instance.DescreaseAllMumic();
        DOTween.To(() => CameraController.instance.CVC.m_Lens.FieldOfView, f => CameraController.instance.CVC.m_Lens.FieldOfView = f, 20, 2);
        DOTween.To(() => VolumeController.instance.chromaticAberration.intensity.value, tv => VolumeController.instance.chromaticAberration.intensity.value = tv, 1, 2);
        GetComponent<SoundEffectPlayer>().PlaySoundEffect(1, 0.6f);
        yield return new WaitForSeconds(2.5f);
        GetComponent<SoundEffectPlayer>().PlaySoundEffect(2, 0.4f);
        yield return new WaitForSeconds(0.5f);

        if (GameManager.instance.extractedLevelData.Count != 0)
        {
            yield return LoadManager.instance.UnloadScene(GameManager.instance.extractedLevelData.Last().sceneIndex);
            GameManager.instance.currentLevelData = GameManager.instance.extractedLevelData.Last();
        }
        else
        {
            yield return LoadManager.instance.UnloadScene(19);
            GameManager.instance.currentLevelData = LoadManager.instance.firstSceneData;
        }

        RecoverText();
        MusicController.instance.AddAllMusic();
        PlayerController.instance.transform.position = Vector2.up * 1.5f;
        CameraController.instance.CVC.m_Lens.FieldOfView = 50;
        VolumeController.instance.chromaticAberration.intensity.value = 0;
        PlayerController.instance.Refresh();
        EventsHandler.CallLevelFinished();
        LevelCountText("A MONMENT AGO");
    }

    public IEnumerator ExtractAttributeAni()
    {
        gamePanelAnim.SetBool("Extract", true);
        Cursor.visible = false;
        leftConsumptionImage.DOFillAmount(0, 1);
        rightConsumptionImage.DOFillAmount(0, 1);
        offsetBarRectTrans.GetChild(0).localPosition = Vector2.zero;
        offsetBarRectTrans.GetComponent<Image>().DOColor(Color.white, 1);
        GetComponent<SoundEffectPlayer>().PlaySoundEffect(3);

        leftAtrributeText.fontSize = GameManager.instance.leftOffsetAttribute.textSize;
        rightAtrributeText.fontSize = GameManager.instance.rightOffsetAttribute.textSize;
        leftAtrributeText.DOColor(Color.white, 1);
        rightAtrributeText.DOColor(Color.white, 1);
        leftAtrributeText.DOText(GameManager.instance.leftOffsetAttribute.attributeName, 2, false, ScrambleMode.Uppercase).SetEase(Ease.InQuint).OnComplete(() => leftAtrributeText.transform.DOShakeRotation(1, Vector3.forward * 20f, 45).SetEase(Ease.OutExpo));
        rightAtrributeText.DOText(GameManager.instance.rightOffsetAttribute.attributeName, 3, false, ScrambleMode.Uppercase).SetEase(Ease.InQuint).OnComplete(() => rightAtrributeText.transform.DOShakeRotation(1, Vector3.forward * 20f, 45).SetEase(Ease.OutExpo));

        consumptionAtrributeText.DOText(GameManager.instance.consumpteAttribute.attributeName, 5, false, ScrambleMode.Uppercase).SetEase(Ease.InQuint).OnComplete(() => consumptionAtrributeText.transform.DOShakeRotation(1, Vector3.forward * 20f, 45).SetEase(Ease.OutExpo));

        yield return new WaitForSeconds(3);
        offsetBarRectTrans.GetComponent<Image>().DOColor(Color.Lerp(GameManager.instance.leftOffsetAttribute.displayColor, GameManager.instance.rightOffsetAttribute.displayColor, 0.5f), 1);
        CameraController.instance.UIExtractAttributeCIS.GenerateImpulse();
        VolumeController.instance.ChromaticAberrationShake(0.6f, 1);

        yield return new WaitForSeconds(2);
        leftConsumptionImage.DOFillAmount(1, 0.5f);
        rightConsumptionImage.DOFillAmount(1, 0.5f);
        leftConsumptionImage.DOColor(GameManager.instance.consumpteAttribute.displayColor, 1);
        rightConsumptionImage.DOColor(GameManager.instance.consumpteAttribute.displayColor, 1);
        CameraController.instance.UIExtractAttributeCIS.GenerateImpulse();
        VolumeController.instance.ChromaticAberrationShake(0.6f, 1);
        GetComponent<SoundEffectPlayer>().PlaySoundEffect(5);

        yield return new WaitForSeconds(1);
        gamePanelAnim.SetBool("Extract", false);
        leftAtrributeText.DOColor(Color.black, 1);
        rightAtrributeText.DOColor(Color.black, 1);
        yield return new WaitForSeconds(1);

        for (int i = 1; i < MusicController.instance.tracks.Length; i++)
            MusicController.instance.AddMusic(i);

        CameraController.instance.UIGamePanelObjectCIS.GenerateImpulse();
        VolumeController.instance.ChromaticAberrationShake(1f, 2);
        GetComponent<SoundEffectPlayer>().PlaySoundEffect(4);
    }

    public IEnumerator StartGame()
    {
        mainMenuPanelAnim.SetBool("StartGame", true);
        GetComponent<SoundEffectPlayer>().PlaySoundEffect(0);
        yield return new WaitForSeconds(1);

        MusicController.instance.AddMusic(0);
        Confiner.instance.SetConfiner();
        VolumeController.instance.ChromaticAberrationShake(1, 1);
        EventsHandler.CallLevelFinished();
        mainMenuPanelObject.SetActive(false);
        gamePanelAnim.gameObject.SetActive(true);
        pauseButton.gameObject.SetActive(pausePanelObject.activeSelf);
        HTPButton.gameObject.SetActive(pausePanelObject.activeSelf);
        PlayerController.instance.Refresh();
        Cursor.SetCursor(aimCursur, new Vector2(32, 32), CursorMode.Auto);
        allowPause = true;
    }

    public void RecoverText()
    {
        leftAtrributeText.text = rightAtrributeText.text = "-----";
        consumptionAtrributeText.text = "---------";
        leftAtrributeText.fontSize = rightAtrributeText.fontSize = 55;

        leftConsumptionImage.fillAmount = rightConsumptionImage.fillAmount = 0;
        offsetBarRectTrans.GetComponent<Image>().color = new Color32(165, 165, 165, 255);
    }

    public void LevelCountText(string text)
    {
        levelCountText.text = text;
        levelCountText.color = Color.black;
        levelCountText.DOColor(Color.clear, 3).SetEase(Ease.InCirc);
    }

}

using Cinemachine;
using System;
using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    public GameObject mainCamera;
    public CinemachineVirtualCamera CVC;
    public CinemachineConfiner CC;
    public CinemachineImpulseSource playerShootCIS;
    public CinemachineImpulseSource playerHurtCIS;
    public CinemachineImpulseSource enemyDeadCIS;
    public CinemachineImpulseSource enemyDestructionCIS;
    public CinemachineImpulseSource exitCIS;
    public CinemachineImpulseSource attributeAreaCIS;
    public CinemachineImpulseSource UIExtractAttributeCIS;
    public CinemachineImpulseSource UIGamePanelObjectCIS;
    public Transform followTargetTrans;

    protected override void Awake()
    {
        base.Awake();
        CVC.m_Lens.FieldOfView = 50f;
    }

    private void OnEnable()
    {
        EventsHandler.BeforeSceneLoad += OnBeforeSceneLoad;
        EventsHandler.AfterSceneLoad += OnAfterSceneLoad;
    }

    private void OnDisable()
    {
        EventsHandler.BeforeSceneLoad -= OnBeforeSceneLoad;
        EventsHandler.AfterSceneLoad -= OnAfterSceneLoad;
    }

    private void OnBeforeSceneLoad()
    {
        CVC.transform.position = new Vector3(PlayerController.instance.transform.position.x, PlayerController.instance.transform.position.y, -10);
    }

    private void OnAfterSceneLoad() => mainCamera.SetActive(true);
}

using System;
using UnityEngine;

public static class EventsHandler
{
    public static event Action AfterSceneLoad;
    public static void CallAfterSceneLoad()
    {
        AfterSceneLoad?.Invoke();
    }

    public static event Action LevelFinished;
    public static void CallLevelFinished()
    {
        LevelFinished?.Invoke();
    }

    public static event Action BeforeSceneLoad;
    public static void CallBeforeSceneLoad()
    {
        BeforeSceneLoad?.Invoke();
    }
}

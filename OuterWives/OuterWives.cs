using System.Reflection;
using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using UnityEngine;

namespace OuterWives;

public class OuterWives : ModBehaviour
{
    public static IModHelper Helper { get; private set; }
    public static AssetBundle Assets { get; private set; }
    public static OuterWives Instance { get; internal set; }

    public static void Log(string text)
    {
        Helper.Console.WriteLine(text, MessageType.Info);
    }

    public static void Error(string text)
    {
        Helper.Console.WriteLine(text, MessageType.Error);
    }

    protected void Awake()
    {
        Instance = this;
    }

    protected void Start()
    {
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

        Helper = ModHelper;

        Assets = ModHelper.Assets.LoadBundle("Assets/outer-wives");

        LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
        {
            if (loadScene != OWScene.SolarSystem) return;

            ModHelper.Events.Unity.FireInNUpdates(() =>
            {
                ThingFinder.Create();
                PhotoManager.Create();
                WifeManager.Create();
            }, 100); // TODO ewww
        };
    }

    public static void Notify(string text)
    {
        if (NotificationManager.SharedInstance == null)
        {
            Error("Tried to send notification, but notification manager instance is null.");
            return;
        }

        NotificationManager.SharedInstance.PostNotification(new NotificationData(text));
    }
}

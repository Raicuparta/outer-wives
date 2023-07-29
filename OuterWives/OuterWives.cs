using System;
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

    private PopupMenu _languageWarningPopup;

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

        TextTranslation.Get().OnLanguageChanged += OnLanguageChanged;
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

    private void OnLanguageChanged()
    {
        ModHelper.Events.Unity.FireOnNextUpdate(() =>
        {
            if (TranslationManager.Instance.ModSupportsCurrentLanguage || _languageWarningPopup != null) return;

            var menuApi = ModHelper.Interaction.TryGetModApi<IMenuAPI>("_nebula.MenuFramework");

            _languageWarningPopup = menuApi.MakeInfoPopup($"Outer Wives hasn't been translated for your current language ({TranslationManager.Instance.GetCurrentLanguage()}). This will cause the character dialogue to be a mix of English and your selected language. It is recommended that you change the Outer Wilds language to English, or wait for someone to translate Outer Wives to your prefered language.", "OK");
            _languageWarningPopup._rootCanvas = Locator.GetPromptManager().GetComponent<Canvas>();
            _languageWarningPopup.EnableMenu(true);
            _languageWarningPopup.OnPopupConfirm += () =>
            {
                Destroy(_languageWarningPopup);
                _languageWarningPopup = null;
            };
        });
    }
}

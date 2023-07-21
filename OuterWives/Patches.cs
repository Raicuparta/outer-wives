using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OuterWives;

[HarmonyPatch]
public static class TranslationPatches
{

    private static readonly Dictionary<string, string> translations = new()
    {
        { "MARRY_ME", "Shut up, will you marry me?" },

        { "PROPOSE_PHOTO", "What if I bring you a nice picture?" },

        { "PROPOSE_STONE", "What if I bring you a nice stone with a drawing on it?" },

        { "PROPOSE_MUSIC", "What if I play some nice music for you?" },

        { "REJECTION_1", "No, I would really rather not marry you." },

        { "REJECTION_2", "Unless you can woo me somehow." },

        { "REQUEST_PHOTO_1", $"Yes, that could work. I would love to see a picture of my beloved {Constants.Tokens.PhotoPreference}" },

        { "REQUEST_STONE_1", $"Yes, that could work. I would love to have a stone with a painting of {Constants.Tokens.StonePreference}" },
            
        { "REQUEST_MUSIC_1", $"Yes, that could work. I would love to hear some {Constants.Tokens.MusicPreference} music" },

        { "ACCEPT_REQUEST", $"Ugh alright fine!" },
    };

    private static void SetPreferenceText(ref string original, string token, string value)
    {
        original = original.Replace(token, $"<color=orange>{value}</color>");
    }

    [HarmonyPrefix, HarmonyPatch(typeof(TextTranslation), nameof(TextTranslation.Translate))]
    public static bool TextTranslation_Translate(string key, ref string __result)
    {
        if (!key.StartsWith(Constants.Global.Prefix)) return true;

        OuterWives.Helper.Console.WriteLine($"Translating key {key}");

        var keyParts = key.Split('/');
        var hasCharacterName = keyParts.Length > 2;

        var dictionaryKey = keyParts[hasCharacterName ? 2 : 1];

        var hasTranslation = translations.TryGetValue(dictionaryKey, out __result);
        if (!hasTranslation) return true;

        if (hasCharacterName)
        {
           var characterName = keyParts[1];
            OuterWives.Helper.Console.WriteLine($"Character name in patch: {characterName} ({dictionaryKey})");
            var wife = WifeManager.Instance.Wives.First(w => w.Name == characterName);

            SetPreferenceText(ref __result, Constants.Tokens.PhotoPreference, wife.PhotoPreference);
            SetPreferenceText(ref __result, Constants.Tokens.StonePreference, wife.StonePreference);
            SetPreferenceText(ref __result, Constants.Tokens.MusicPreference, wife.MusicPreference);
        }

        return false;
    }

    private static bool _startingConversation = false;


    [HarmonyPrefix, HarmonyPatch(typeof(CharacterDialogueTree), nameof(CharacterDialogueTree.StartConversation))]
    public static void BeforeConversationStart(CharacterDialogueTree __instance)
    {
        _startingConversation = true;

        var wife = WifeManager.Instance.GetWifeByCharacter(__instance);
        if (wife == null) return;

        wife.PresentDesires();
    }

    [HarmonyPostfix, HarmonyPatch(typeof(CharacterDialogueTree), nameof(CharacterDialogueTree.EndConversation))]
    public static void AfterConversationStart()
    {
        _startingConversation = false;
    }


    [HarmonyPrefix, HarmonyPatch(typeof(ToolModeSwapper), nameof(ToolModeSwapper.UnequipTool))]
    public static bool PreventUnequip()
    {
        return !(Locator.GetToolModeSwapper().IsInToolMode(ToolMode.Probe) && _startingConversation);
    }
}

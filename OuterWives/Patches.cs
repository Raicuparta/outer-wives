using HarmonyLib;
using System.Linq;

namespace OuterWives;

[HarmonyPatch]
public static class TranslationPatches
{
    private static void SetPreferenceText(ref string original, string token, string value)
    {
        original = original.Replace(token, $"<color=orange>{value}</color>");
    }

    [HarmonyPrefix, HarmonyPatch(typeof(TextTranslation), nameof(TextTranslation.Translate))]
    public static bool TextTranslation_Translate(string key, ref string __result)
    {
        if (!key.StartsWith(TextIds.Prefix)) return true;

        var keyParts = key.Split('/');
        var hasCharacterName = keyParts.Length > 2;

        var dictionaryKey = keyParts[hasCharacterName ? 2 : 1];

        var hasTranslation = TranslationManager.Instance.Translation.TryGetValue(dictionaryKey, out __result);
        if (!hasTranslation) return true;

        if (hasCharacterName)
        {
           var characterName = keyParts[1];
            OuterWives.Helper.Console.WriteLine($"Character name in patch: {characterName} ({dictionaryKey})");
            var wife = WifeManager.Instance.Wives.First(w => w.Name == characterName);

            SetPreferenceText(ref __result, TextIds.Tokens.PhotoPreference, wife.PhotoDesire.DisplayName);
            SetPreferenceText(ref __result, TextIds.Tokens.StonePreference, wife.StoneDesire.DisplayName);
            SetPreferenceText(ref __result, TextIds.Tokens.MusicPreference, wife.MusicDesire.DisplayName);
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

using HarmonyLib;
using OuterWives.Desires;
using System.Linq;

namespace OuterWives;

[HarmonyPatch]
public static class Patches
{
    private static void SetPreferenceText(ref string original, IDesire desire)
    {
        original = original.Replace(TextIds.Tokens.Preference(desire), $"<color=orange>{desire.DisplayName}</color>");
    }

    [HarmonyPrefix, HarmonyPatch(typeof(TextTranslation), nameof(TextTranslation.Translate))]
    public static bool TranslateText(string key, ref string __result)
    {
        if (!key.StartsWith(TextIds.Prefix)) return true;

        var keyParts = key.Split('/');
        var hasCharacterName = keyParts.Length > 2;

        var textKey = keyParts[hasCharacterName ? 2 : 1];

        var text = TranslationManager.Instance.GetText(textKey);
        if (text == default) return true;
        __result = text;

        if (hasCharacterName)
        {
           var characterId = keyParts[1];
            OuterWives.Helper.Console.WriteLine($"Character ID in patch: {characterId} ({textKey})");
            var wife = WifeManager.Instance.Wives.First(w => w.Id == characterId);

            foreach (var desire in wife.Desires)
            {
                SetPreferenceText(ref __result, desire);
            }
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

    [HarmonyPostfix, HarmonyPatch(typeof(NomaiConversationManager), nameof(NomaiConversationManager.OnFinishDialogue))]
    public static void PreventDisablingSolanumDialogue(NomaiConversationManager __instance)
    {
        __instance._characterDialogueTree.GetInteractVolume().EnableInteraction();
    }
}

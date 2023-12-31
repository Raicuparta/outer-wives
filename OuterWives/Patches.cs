﻿using HarmonyLib;
using OuterWives.Desires;
using System.Collections.Generic;

namespace OuterWives;

[HarmonyPatch]
public static class Patches
{
    private static string ReplaceDesireTokens(string text, List<IDesire> desires)
    {
        var tokenToValue = new Dictionary<string, string>();
        foreach (var desire in desires)
        {
            tokenToValue[TextIds.Tokens.Desire(desire)] = $"<color=orange>{desire.DisplayName}</color>";
        }

        return TranslationManager.ReplaceTokens(text, tokenToValue);
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
            var wife = WifeManager.Instance.GetWife(characterId);

            if (wife == null)
            {
                OuterWives.Error($"Failed to find wife with character ID \"{characterId}\"");
                return true;
            }

            __result = ReplaceDesireTokens(__result, wife.Desires);
        }

        return false;
    }

    private static bool _startingConversation = false;


    [HarmonyPrefix, HarmonyPatch(typeof(CharacterDialogueTree), nameof(CharacterDialogueTree.StartConversation))]
    public static void SetUpUnequipPrevention()
    {
        _startingConversation = true;
    }

    [HarmonyPostfix, HarmonyPatch(typeof(CharacterDialogueTree), nameof(CharacterDialogueTree.EndConversation))]
    public static void ResetUnequipPrevention()
    {
        _startingConversation = false;
    }


    [HarmonyPrefix, HarmonyPatch(typeof(ToolModeSwapper), nameof(ToolModeSwapper.UnequipTool))]
    public static bool PreventUnequip()
    {
        // Unequiping the probe launcher would mean forever losing the picture the player took.
        // The game usually automatically unequips any tool when starting a dialogue.
        // So we prevent that from happening, so the wife can have a chance to look at the picture.
        return !(Locator.GetToolModeSwapper().IsInToolMode(ToolMode.Probe) && _startingConversation);
    }

    [HarmonyPostfix, HarmonyPatch(typeof(NomaiConversationManager), nameof(NomaiConversationManager.OnFinishDialogue))]
    public static void PreventDisablingSolanumDialogue(NomaiConversationManager __instance)
    {
        __instance._characterDialogueTree.GetInteractVolume().EnableInteraction();
    }

    [HarmonyPostfix, HarmonyPatch(typeof(PrisonerChoiceDialogue), nameof(PrisonerChoiceDialogue.OnEndConversation))]
    public static void PreventDisablingPrisonerDialogue_Choice(PrisonerChoiceDialogue __instance)
    {
        __instance._interactReceiver.EnableInteraction();
    }

    [HarmonyPostfix, HarmonyPatch(typeof(PrisonerDirector), nameof(PrisonerDirector.OnFinishDialogue))]
    public static void PreventDisablingPrisonerDialogue_Director(PrisonerDirector __instance)
    {
        __instance._characterDialogueTree.GetInteractVolume().EnableInteraction();
    }

    [HarmonyPrefix, HarmonyPatch(typeof(QuantumMoon), nameof(QuantumMoon.IsProbeInside))]
    public static bool ForceDetectProbeWithPlayerInQM(ref bool __result, QuantumMoon __instance)
    {
        if (!__instance.IsPlayerInside()) return true;

        // Make the game think the probe is inside QM, if the player is also inside QM.
        // This will prevent interference, and allow taking pictures of photo desires while in QM.
        __result = true;
        return false;
    }
}

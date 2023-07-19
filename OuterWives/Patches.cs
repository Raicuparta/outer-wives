using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OuterWives
{
    [HarmonyPatch]
    public static class TranslationPatches
    {

        private static Dictionary<string, string> translations = new Dictionary<string, string>
        {
            { "WIFE_X_REJECTION_PART_1_VARIANT_1", "No, I would really rather not marry you." },

            { "WIFE_X_REJECTION_PART_2_VARIANT_1", "Unless you can woo me by fulfilling my deepest desires." },

            { "WIFE_X_REJECTION_PART_3_VARIANT_1", "For instance, I would love to see a picture of my beloved $$SECRET_LOVE$$. I've loved them for a very long time, but they do not wish to marry me." },
        };

        [HarmonyPrefix, HarmonyPatch(typeof(TextTranslation), nameof(TextTranslation.Translate))]
        public static bool TextTranslation_Translate(string key, ref string __result)
        {
            if (!key.StartsWith("WIFE_")) return true;

            var keyParts = key.Split('_');
            var characterName = keyParts[1];
            OuterWives.Helper.Console.WriteLine($"Character name in patch: {characterName} ({key})");
            var wife = OuterWives.Wives.First(w => w.name == characterName);

            __result = translations[key.Replace(characterName, "X")];

            __result = __result.Replace("$$SECRET_LOVE$$", wife.secretLove.name);
            return false;
        }
    }
}

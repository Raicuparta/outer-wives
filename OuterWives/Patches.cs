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
            {  "MARRIAGE_REJECTION_1", "No, I would really rather not marry you." }
        };

        [HarmonyPrefix, HarmonyPatch(typeof(TextTranslation), nameof(TextTranslation.Translate))]
        public static bool TextTranslation_Translate(string key, ref string __result)
        {
            if (!key.StartsWith("MARRIAGE_")) return true;

            __result = translations[key];
            return false;
        }
    }
}

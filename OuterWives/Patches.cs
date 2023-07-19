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

        [HarmonyPrefix, HarmonyPatch(typeof(TextTranslation), nameof(TextTranslation.Translate))]
        public static bool TextTranslation_Translate(string key, ref string __result)
        {
            OuterWives.Helper.Console.WriteLine("translate");
            return false;
        }
    }
}

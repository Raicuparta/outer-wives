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
        private const string KEY_PREFIX = "WIFE";
        private const string PHOTO_PREFERENCE = "$PHOTO_PREFERENCE$";
        private const string STONE_PREFERENCE = "$STONE_PREFERENCE$";
        private const string MUSIC_PREFERENCE = "$MUSIC_PREFERENCE$";

        private static Dictionary<string, string> translations = new Dictionary<string, string>
        {
            { "MARRY_ME", "Shut up, will you marry me?" },

            { "PROPOSE_PHOTO", "What if I bring you a nice picture?" },

            { "PROPOSE_STONE", "What if I bring you a nice stone with a drawing on it?" },

            { "PROPOSE_MUSIC", "What if I play some nice music for you?" },

            { "REJECTION_PART_1", "No, I would really rather not marry you." },

            { "REJECTION_PART_2", "Unless you can woo me somehow." },

            { "REQUEST_PHOTO_PART_1", $"Yes, that could work. I would love to see a picture of my beloved {PHOTO_PREFERENCE}" },

            { "REQUEST_STONE_PART_1", $"Yes, that could work. I would love to have a stone with a painting of {STONE_PREFERENCE}" },
                
            { "REQUEST_MUSIC_PART_1", $"Yes, that could work. I would love to hear some {MUSIC_PREFERENCE} music" },

            { "ACCEPT_REQUEST", $"Ugh alright fine!" },
        };

        [HarmonyPrefix, HarmonyPatch(typeof(TextTranslation), nameof(TextTranslation.Translate))]
        public static bool TextTranslation_Translate(string key, ref string __result)
        {
            if (!key.StartsWith(KEY_PREFIX)) return true;

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
                var wife = OuterWives.Wives.First(w => w.name == characterName);

                __result = __result.Replace(PHOTO_PREFERENCE, wife.PhotoPreference.name);
                __result = __result.Replace(STONE_PREFERENCE, wife.StonePreference);
                __result = __result.Replace(MUSIC_PREFERENCE, wife.MusicPreference);
            }

            return false;
        }
    }
}

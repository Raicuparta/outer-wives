using OWML.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static TextTranslation;

namespace OuterWives;

public class TranslationManager: MonoBehaviour
{
    private static TranslationManager _instance;
    public static TranslationManager Instance => _instance == null ? _instance = Create() : _instance;

    private Dictionary<string, string> _translation;
    private Dictionary<string, string> _defaultTranslation;
    private const string DefaultLanguage = "english";

    private static TranslationManager Create()
    {
        return new GameObject(nameof(TranslationManager)).AddComponent<TranslationManager>();
    }

    private void InitializeTranslation()
    {
        _defaultTranslation = LoadTranslation(DefaultLanguage);
        var language = TextTranslation.Get().GetLanguage().GetName().ToLower();
        _translation = LoadTranslation(language) ?? _defaultTranslation;
    }

    private Dictionary<string, string> LoadTranslation(string language)
    {
        return OuterWives.Helper.Storage.Load<Dictionary<string, string>>($"Localization/{language}.json");
    }

    public string GetText(string key)
    {
        if (_translation == null) InitializeTranslation();

        _translation.TryGetValue(key, out var text);

        if (text == null)
        _defaultTranslation.TryGetValue(key, out text);

        return text;
    }
}

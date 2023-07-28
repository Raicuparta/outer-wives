using OWML.Utils;
using System.Collections.Generic;
using UnityEngine;

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

    public string GetText(string key, Dictionary<string, string> tokenToValue)
    {
        return ReplaceTokens(GetText(key), tokenToValue);
    }

    public string GetText(string key)
    {
        if (_translation == null) InitializeTranslation();

        _translation.TryGetValue(key, out var text);

        if (text == null) _defaultTranslation.TryGetValue(key, out text);

        return text;
    }

    public static string ReplaceTokens(string text, Dictionary<string, string> tokenToValue)
    {
        var result = text;
        foreach (var entry in tokenToValue)
        {
            result = result.Replace(entry.Key, entry.Value);
        }
        return result;
    }
}

using OWML.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace OuterWives;

public class TranslationManager: MonoBehaviour
{
    public bool ModSupportsCurrentLanguage => _preferedTranslation != null;

    private static TranslationManager _instance;
    public static TranslationManager Instance => _instance == null ? _instance = Create() : _instance;

    private Dictionary<string, string> _preferedTranslation;
    private Dictionary<string, string> _defaultTranslation;
    private const string DefaultLanguage = "english";
    private bool _isInitialized;

    private static TranslationManager Create()
    {
        return new GameObject(nameof(TranslationManager)).AddComponent<TranslationManager>();
    }

    private void Awake()
    {
        InitializeTranslation();
        TextTranslation.Get().OnLanguageChanged += InitializeTranslation;
    }

    private void OnDestroy()
    {
        TextTranslation.Get().OnLanguageChanged -= InitializeTranslation;
    }

    private void InitializeTranslation()
    {
        _defaultTranslation = LoadTranslation(DefaultLanguage);
        _preferedTranslation = LoadTranslation(GetCurrentLanguage());
        _isInitialized = true;
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
        if (!_isInitialized) InitializeTranslation();

        var translation = _preferedTranslation ?? _defaultTranslation;

        translation.TryGetValue(key, out var text);

        if (text == null) _defaultTranslation.TryGetValue(key, out text);

        if (text == null) return key;

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

    public string GetCurrentLanguage()
    {
        return TextTranslation.Get().GetLanguage().GetName().ToLower();
    }
}

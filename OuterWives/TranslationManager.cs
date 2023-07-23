using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OuterWives;

public class TranslationManager: MonoBehaviour
{
    private static TranslationManager _instance;
    public static TranslationManager Instance => _instance == null ? _instance = Create() : _instance;

    public Dictionary<string, string> Translation { get; private set; }

    private static TranslationManager Create()
    {
        return new GameObject(nameof(TranslationManager)).AddComponent<TranslationManager>();
    }

    private void Awake ()
    {
        Translation = OuterWives.Helper.Storage.Load<Dictionary<string, string>>("Localization/english.json");
        OuterWives.Log("Loading translation...");
        foreach (var entry in Translation)
        {
            OuterWives.Log($"{entry.Key}: {entry.Value}");
        }
    }
}

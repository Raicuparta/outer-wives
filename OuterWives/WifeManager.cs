using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OuterWives;

public class WifeManager: MonoBehaviour
{
    public static WifeManager Instance;

    private readonly Dictionary<string, Wifey> _wives = new();
    private readonly string[] _characterBlockList = new[]
{
        "Tephra",
        "Arkose",
        "Galena",
        "Moraine",
        "Mica",
    };

    public static void Create()
    {
        Instance = new GameObject(nameof(WifeManager)).AddComponent<WifeManager>();
    }

    private void Start()
    {
        var characters = ThingFinder.Instance.GetCharacters();
        for (var characterIndex = 0; characterIndex < characters.Length; characterIndex++)
        {
            var character = characters[characterIndex];
            if (_characterBlockList.Contains(character._characterName)) continue;

            CreateWife(character, characterIndex);
        }
    }

    private void CreateWife(CharacterDialogueTree character, int characterIndex)
    {
        var wife = Wifey.Create(character, characterIndex);
        _wives[wife.Id] = wife;
    }

    public Wifey GetWife(string characterId)
    {
        _wives.TryGetValue(characterId, out var wife);
        return wife;
    }
}

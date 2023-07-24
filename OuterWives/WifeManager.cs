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
        var characters = ThingFinder.Instance.GetCharacters().Where(character => !_characterBlockList.Contains(character._characterName));
        foreach (var character in characters)
        {
            CreateWife(character);
        }
    }

    private void CreateWife(CharacterDialogueTree character)
    {
        var wife = Wifey.Create(character);
        _wives[wife.Id] = wife;
    }

    public Wifey GetWife(string characterId)
    {
        _wives.TryGetValue(characterId, out var wife);
        return wife;
    }
}

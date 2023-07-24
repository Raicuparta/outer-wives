using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OuterWives;

public class WifeManager: MonoBehaviour
{
    public static WifeManager Instance;

    public readonly List<Wifey> Wives = new();
    private readonly Dictionary<CharacterDialogueTree, Wifey> _characterWifeMap = new();
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
        Wives.Add(wife);
        _characterWifeMap[wife.Character] = wife;
    }
}

using OWML.ModHelper;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace OuterWives;

public class WifeManager: MonoBehaviour
{
    public static WifeManager Instance;

    public readonly List<Wifey> Wives = new();
    private readonly Dictionary<CharacterDialogueTree, Wifey> _characterWifeMap = new();

    public static void Create()
    {
        Instance = new GameObject(nameof(WifeManager)).AddComponent<WifeManager>();
    }

    private void Start()
    {
        CreateWife("Feldspar");
        CreateWife("Hal");
        CreateWife("Chert");
        CreateWife("Hornfels");
        CreateWife("Slate");
        CreateWife("Rutile");
        CreateWife("Gneiss");
        CreateWife("Marl");
        CreateWife("Tuff");
        CreateWife("Esker");
        CreateWife("Porphy");
        CreateWife("the Prisoner"); // TODO you can't get out of there, so we shouldn't ask for pictures.
        CreateWife("Tektite");
        CreateWife("Gossan");
        CreateWife("Spinel");
        CreateWife("Gabbro");
        CreateWife("Riebeck");
        CreateWife("Self");
        CreateWife("Solanum");
    }

    private void CreateWife(string name)
    {
        var wife = Wifey.Create(name);
        Wives.Add(wife);
        _characterWifeMap[wife.Character] = wife;
    }

    public Wifey GetWifeByCharacter(CharacterDialogueTree character)
    {
        _characterWifeMap.TryGetValue(character, out var wife);
        return wife;
    }
}

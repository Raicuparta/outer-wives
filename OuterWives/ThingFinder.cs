using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

namespace OuterWives;

public class ThingFinder: MonoBehaviour
{
    public static ThingFinder Instance { get; private set; }

    private readonly string[] _stoneBlocklist = new string[] {
        "None",
        "Module",
    };

    private readonly string[] _characterBlocklist = new string[] {
        null,
        "",
        CharacterDialogueTree.RECORDING_NAME,
        CharacterDialogueTree.SIGN_NAME,
    };

    private SharedStone[] _stones;
    private TravelerController[] _travelers;
    private CharacterDialogueTree[] _characters;

    public static void Create()
    {
        Instance = new GameObject(nameof(ThingFinder)).AddComponent<ThingFinder>();
    }

    private void InitializeStones()
    {
        if (_stones != null) return;

        _stones = Resources.FindObjectsOfTypeAll<SharedStone>()
            .Where(stone => !_stoneBlocklist.Any(blockedWord => stone.GetDisplayName().Contains(blockedWord)))
            .GroupBy(stone => stone.GetDisplayName())
            .Select(group => group.First())
            .ToArray();
    }

    public ReadOnlyArray<SharedStone> GetStones()
    {
        InitializeStones();
        return _stones;
    }

    public SharedStone GetRandomStone()
    {
        return GetRandomObject(GetStones());
    }

    private void InitializeTravelers()
    {
        if (_travelers != null) return;

        _travelers = Resources.FindObjectsOfTypeAll<TravelerController>()
            .Where(traveler => traveler._audioSource != null)
            .ToArray();
    }

    public ReadOnlyArray<TravelerController> GetTravelers()
    {
        InitializeTravelers();
        return _travelers;
    }

    public TravelerController GetRandomTraveler()
    {
        InitializeTravelers();
        return GetRandomObject(GetTravelers());
    }

    private void InitializeCharacters ()
    {
        if (_characters != null) return;

        _characters = Resources.FindObjectsOfTypeAll<CharacterDialogueTree>()
            .Where(character => !_characterBlocklist.Contains(character._characterName))
            .ToArray();
    }

    public ReadOnlyArray<CharacterDialogueTree> GetCharacters()
    {
        InitializeCharacters();
        return _characters;
    }

    public CharacterDialogueTree GetCharacter(string name)
    {
        return GetCharacters().First(character => character._characterName == name);
    }

    public CharacterDialogueTree GetRandomCharacter()
    {
        InitializeCharacters();
        return GetRandomObject(GetCharacters());
    }

    private TComponent GetRandomObject<TComponent>(ReadOnlyArray<TComponent> array)
    {
        return array[Random.Range(0, array.Count)];
    }
}

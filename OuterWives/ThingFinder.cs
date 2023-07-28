using System;
using System.Linq;
using OuterWives.Extensions;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

namespace OuterWives;

public class ThingFinder: MonoBehaviour
{
    public static ThingFinder Instance { get; private set; }

    private readonly NomaiRemoteCameraPlatform.ID[] _stoneBlocklist = new NomaiRemoteCameraPlatform.ID[] {
        NomaiRemoteCameraPlatform.ID.None,
        NomaiRemoteCameraPlatform.ID.GD_ProbeCannonDamagedModule,
        NomaiRemoteCameraPlatform.ID.GD_ProbeCannonIntactModule,
        NomaiRemoteCameraPlatform.ID.GD_ProbeCannonSunkenModule,
    };

    private readonly string[] _characterBlocklist = new string[] {
        null,
        "",
        CharacterDialogueTree.RECORDING_NAME,
        CharacterDialogueTree.SIGN_NAME,
    };

    private readonly ItemType[] _allowedItemTypes = new[]
    {
        ItemType.WarpCore,
        ItemType.SlideReel,
        ItemType.Scroll,
        ItemType.Lantern,
    };

    private ShuffledArray<SharedStone> _stones;
    private ShuffledArray<AudioSignal> _musicSignals;
    private ShuffledArray<CharacterDialogueTree> _characters;
    private ShuffledArray<OWItem> _items;

    public static void Create()
    {
        Instance = new GameObject(nameof(ThingFinder)).AddComponent<ThingFinder>();
    }

    private void InitializeStones()
    {
        if (_stones != null) return;

        _stones = Resources.FindObjectsOfTypeAll<SharedStone>()
            .Where(stone => !_stoneBlocklist.Contains(stone.GetRemoteCameraID()))
            .GroupBy(stone => stone.GetDisplayName())
            .Select(group => group.First())
            .ToShuffledArray();
    }

    public ShuffledArray<SharedStone> GetStones()
    {
        InitializeStones();
        return _stones;
    }

    private void InitializeTravelers()
    {
        if (_musicSignals != null) return;

        _musicSignals = Resources.FindObjectsOfTypeAll<TravelerController>()
            .Where(traveler => traveler._audioSource != null)
            .Select(traveler => traveler._audioSource.GetComponent<AudioSignal>())
            .ToShuffledArray();
    }

    public ShuffledArray<AudioSignal> GetMusicSignals()
    {
        InitializeTravelers();
        return _musicSignals;
    }

    private void InitializeCharacters ()
    {
        if (_characters != null) return;

        _characters = Resources.FindObjectsOfTypeAll<CharacterDialogueTree>()
            .Where(character => !_characterBlocklist.Contains(character._characterName))
            .ToShuffledArray();
    }

    public ShuffledArray<CharacterDialogueTree> GetCharacters()
    {
        InitializeCharacters();
        return _characters;
    }

    private void InitializeItems()
    {
        if (_items != null) return;

        _items = Resources.FindObjectsOfTypeAll<OWItem>()
            .Where(item => _allowedItemTypes.Any(allowedType => allowedType == item.GetItemType()))
            .GroupBy(item => item.GetItemType().ToString())
            .Select(group => group.First())
            .ToShuffledArray();
    }

    public ShuffledArray<OWItem> GetItems()
    {
        InitializeItems();
        return _items;
    }
}

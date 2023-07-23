﻿using System.Linq;
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

    private SharedStone[] _stones;
    private AudioSignal[] _musicSignals;
    private CharacterDialogueTree[] _characters;

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
        if (_musicSignals != null) return;

        _musicSignals = Resources.FindObjectsOfTypeAll<TravelerController>()
            .Where(traveler => traveler._audioSource != null)
            .Select(traveler => traveler._audioSource.GetComponent<AudioSignal>())
            .ToArray();
    }

    public ReadOnlyArray<AudioSignal> GetMusicSignals()
    {
        InitializeTravelers();
        return _musicSignals;
    }

    public AudioSignal GetRandomMusicSignal()
    {
        InitializeTravelers();
        return GetRandomObject(GetMusicSignals());
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

    public CharacterDialogueTree GetRandomCharacter()
    {
        InitializeCharacters();
        return GetRandomObject(GetCharacters());
    }

    public TComponent GetRandomObject<TComponent>(ReadOnlyArray<TComponent> array)
    {
        return array[Random.Range(0, array.Count)];
    }
}

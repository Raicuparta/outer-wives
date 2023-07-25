﻿using System.Collections.Generic;
using System.Linq;
using OuterWives.Extensions;
using UnityEngine;

namespace OuterWives;

public class PhotoManager : MonoBehaviour
{
    public static PhotoManager Instance { get; private set; }

    private readonly List<PhotogenicCharacter> _charactersInShot = new();
    private readonly string[] _characterBlockList = new[]
    {
        "the Prisoner"
    };
    private ShuffledArray<PhotogenicCharacter> _characters;

    public static void Create()
    {
        Instance = new GameObject(nameof(PhotoManager)).AddComponent<PhotoManager>();
    }

    private void Start()
    {
        _characters = ThingFinder.Instance.GetCharacters().Array
            .Where(character => !_characterBlockList.Contains(character._characterName))
            .Select(PhotogenicCharacter.Create) // Create PhotoGenicCharacter components for all including clones,
            .GroupBy(character => character.Id) // but merge clones into one in the stored array.
            .Select(group => group.First())
            .ToShuffledArray();
    }

    private void OnEnable()
    {
        GlobalMessenger.AddListener("Probe Snapshot Removed", OnProbeSnapshotRemoved);
        GlobalMessenger<ProbeCamera>.AddListener("ProbeSnapshot", OnProbeSnapshot);
    }

    private void OnDisable()
    {
        GlobalMessenger.RemoveListener("Probe Snapshot Removed", OnProbeSnapshotRemoved);
        GlobalMessenger<ProbeCamera>.RemoveListener("ProbeSnapshot", OnProbeSnapshot);
    }

    private void OnDestroy()
    {
        Reset();
    }

    private void OnProbeSnapshotRemoved()
    {
        Reset();
    }

    private void OnProbeSnapshot(ProbeCamera camera)
    {
        Reset();
        string notificationText = null;
        foreach (var character in _characters.Array)
        {
            if (character.IsInShot(camera))
            {
                _charactersInShot.Add(character);
            }
        }

        if (_charactersInShot.Count == 0) return;

        // TODO: Localize "photographed".
        notificationText = $"Photographed {string.Join(", ", _charactersInShot.Select(character => character.DisplayName))}";
        NotificationManager.SharedInstance.PostNotification(new NotificationData(notificationText), false);
    }

    private void Reset()
    {
        _charactersInShot.Clear();
    }

    public PhotogenicCharacter GetRandomCharacter(Wifey wife)
    {
        return _characters.Get(wife.Index);
    }

    public bool IsCharacterInShot(string characterId)
    {
        return _charactersInShot.Any(character => character.Id == characterId);
    }
}

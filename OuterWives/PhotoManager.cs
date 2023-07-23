using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OuterWives;

public class PhotoManager : MonoBehaviour
{
    public static PhotoManager Instance { get; private set; }

    private readonly List<PhotogenicCharacter> _charactersInShot = new();
    private readonly List<PhotogenicCharacter> _characters = new();
    private readonly string[] _characterBlockList = new[]
    {
        "the Prisoner"
    };

    public static void Create()
    {
        Instance = new GameObject(nameof(PhotoManager)).AddComponent<PhotoManager>();
    }

    private void Start()
    {
        var characters = ThingFinder.Instance.GetCharacters()
            .Where(character => !_characterBlockList.Contains(character._characterName));

        foreach (var character in characters)
        {
            _characters.Add(PhotogenicCharacter.Create(character));
        }
    }

    private void OnEnable()
    {
        GlobalMessenger.AddListener("Probe Snapshot Removed", OnProbeSnapshotRemoved);
        GlobalMessenger<ProbeCamera>.AddListener("ProbeSnapshot", new Callback<ProbeCamera>(OnProbeSnapshot));
    }

    private void OnDisable()
    {
        GlobalMessenger.RemoveListener("Probe Snapshot Removed", OnProbeSnapshotRemoved);
        GlobalMessenger<ProbeCamera>.RemoveListener("ProbeSnapshot", new Callback<ProbeCamera>(OnProbeSnapshot));
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
        foreach (var character in _characters)
        {
            if (character.IsInShot(camera))
            {
                _charactersInShot.Add(character);
            }
        }

        if (_charactersInShot.Count == 0) return;

        notificationText = $"Photographed {string.Join(", ", _charactersInShot.Select(character => character.Name))}";
        NotificationManager.SharedInstance.PostNotification(new NotificationData(notificationText), false);
    }

    private void Reset()
    {
        _charactersInShot.Clear();
    }

    public PhotogenicCharacter GetRandomCharacter()
    {
        return _characters[Random.Range(0, _characters.Count)];
    }

    public bool IsCharacterInShot(string characterName)
    {
        return _charactersInShot.Any(character => character.Name == characterName);
    }
}

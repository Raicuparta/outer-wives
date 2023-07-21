using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OuterWives;

public class PhotoManager : MonoBehaviour
{
    public static PhotoManager Instance { get; private set; }

    public PhotogenicCharacter PhotographedCharacter { get; private set; }

    private readonly List<PhotogenicCharacter> _characters = new();

    public static void Create()
    {
        Instance = new GameObject(nameof(PhotoManager)).AddComponent<PhotoManager>();
    }

    private void Start()
    {
        var characters = ThingFinder.Instance.GetCharacters().Where(character => character._characterName != "the Prisoner");
        foreach (var character in characters)
        {
            _characters.Add(character.gameObject.AddComponent<PhotogenicCharacter>());
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
                if (PhotographedCharacter != null)
                {
                    notificationText = "Multiple people in shot";
                    break;
                }
                PhotographedCharacter = character;
                notificationText = $"Photographed {PhotographedCharacter.Name}";
            }
        }

        if (notificationText != null)
        {
            NotificationManager.SharedInstance.PostNotification(new NotificationData(notificationText), false);
        }
    }

    private void Reset()
    {
        PhotographedCharacter = null;
    }

    public PhotogenicCharacter GetRandomCharacter()
    {
        return _characters[Random.Range(0, _characters.Count)];
    }
}

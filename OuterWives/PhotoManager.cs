using System.Collections.Generic;
using UnityEngine;

namespace OuterWives
{
    public class PhotoManager : MonoBehaviour
    {
        public PhotogenicCharacter PhotographedCharacter { get; private set; }
        public readonly List<PhotogenicCharacter> Characters = new();

        public static PhotoManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
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
            foreach (var character in Characters)
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
    }
}

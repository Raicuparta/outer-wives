using OWML.ModHelper;
using Steamworks;
using System;
using System.Linq;
using UnityEngine;

namespace OuterWives;

public class PhotogenicCharacter : MonoBehaviour
{
    public static CharacterDialogueTree PhotographedCharacter { get; private set; }

    private Collider[] _colliders;
    private CharacterDialogueTree _character;
    private float _maxPhotoDistance = 20f;

    private void Awake()
    {
        _colliders = transform.parent.GetComponentsInChildren<Collider>();
        _character = GetComponent<CharacterDialogueTree>();
    }

    private void OnEnable()
    {
        GlobalMessenger<ProbeCamera>.AddListener("ProbeSnapshot", new Callback<ProbeCamera>(OnProbeSnapshot));
    }

    private void OnDestroy()
    {
        PhotographedCharacter = null;
    }

    private void OnDisable()
    {
        GlobalMessenger<ProbeCamera>.RemoveListener("ProbeSnapshot", new Callback<ProbeCamera>(OnProbeSnapshot));
    }

    public bool IsVisible(OWCamera camera)
    {
        foreach (var collider in _colliders)
        {
            if (GeometryUtility.TestPlanesAABB(camera.GetFrustumPlanes(), collider.bounds))
                return true;
        }
        return false;
    }

    private bool IsOccludedFromPosition(Vector3 worldPos)
    {
        var hit = Physics.Linecast(worldPos, GetTargetPosition(), out RaycastHit ray, OWLayerMask.physicalMask);

        if (hit)
        {
            OuterWives.Helper.Console.WriteLine($"{_character._characterName} collides {ray.collider.name}");
        }

        return hit && !_colliders.Contains(ray.collider);
    }

    private Vector3 GetTargetPosition()
    {
        return _character._attentionPoint.position;
    }

    private void OnProbeSnapshot(ProbeCamera camera)
    {
        if (PhotographedCharacter == _character)
        {
            PhotographedCharacter = null;
        }

        Vector3 vector = GetTargetPosition() - camera.transform.position;
        float magnitude = vector.magnitude;
        if (magnitude > this._maxPhotoDistance) return;

        if (!IsVisible(camera.GetOWCamera())) return;

        if (IsOccludedFromPosition(camera.transform.position)) return;

        NotificationManager.SharedInstance.PostNotification(new NotificationData($"Photographed {_character._characterName}"), false);

        PhotographedCharacter = _character;
    }
}

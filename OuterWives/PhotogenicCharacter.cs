using OWML.ModHelper;
using Steamworks;
using System;
using System.Linq;
using UnityEngine;

namespace OuterWives;

public class PhotogenicCharacter : MonoBehaviour
{
    public string Name => _character._characterName;

    private Collider[] _colliders;
    private CharacterDialogueTree _character;
    private float _maxPhotoDistance = 20f;

    private void Awake()
    {
        _colliders = transform.parent.GetComponentsInChildren<Collider>();
        _character = GetComponent<CharacterDialogueTree>();
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
        try
        {

        return _character._attentionPoint.position;
        } catch (Exception e)
        {
            OuterWives.Log($"Failed on {name} ({_character?._characterName})");
            return Vector3.zero;
        }
    }

    public bool IsInShot(ProbeCamera camera)
    {
        if (!_character) return false;

        Vector3 vector = GetTargetPosition() - camera.transform.position;
        float magnitude = vector.magnitude;
        if (magnitude > this._maxPhotoDistance) return false;

        if (!IsVisible(camera.GetOWCamera())) return false;

        if (IsOccludedFromPosition(camera.transform.position)) return false;

        return true;
    }
}

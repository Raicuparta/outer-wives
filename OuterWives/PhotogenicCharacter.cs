using System;
using System.Linq;
using UnityEngine;

namespace OuterWives;

public class PhotogenicCharacter : MonoBehaviour
{
    public string Name => _character._characterName;

    private Collider _collider;
    private Collider[] _collidersToIgnoreForOcclusion;
    private CharacterDialogueTree _character;
    private readonly float _maxPhotoDistance = 20f;

    public static PhotogenicCharacter Create(CharacterDialogueTree character)
    {
        var instance = character.gameObject.AddComponent<PhotogenicCharacter>();
        instance._character = character;
        return instance;
    }

    private void Awake()
    {
        _collidersToIgnoreForOcclusion = transform.parent.GetComponentsInChildren<Collider>();
        _collider = gameObject.GetComponent<Collider>();
    }

    public bool IsVisible(OWCamera camera)
    {
        return GeometryUtility.TestPlanesAABB(camera.GetFrustumPlanes(), _collider.bounds);
    }

    private bool IsOccludedFromPosition(Vector3 worldPos)
    {
        var hit = Physics.Linecast(worldPos, GetTargetPosition(), out RaycastHit ray, OWLayerMask.physicalMask);

        if (hit)
        {
            OuterWives.Helper.Console.WriteLine($"{_character._characterName} collides {ray.collider.name}");
        }

        return hit && !_collidersToIgnoreForOcclusion.Contains(ray.collider);
    }

    private Vector3 GetTargetPosition()
    {
        try
        {
            return _character._attentionPoint.position;
        }
        catch (Exception e)
        {
            OuterWives.Log($"Failed on {name} ({_character?._characterName}): {e}");
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

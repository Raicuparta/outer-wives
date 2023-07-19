using System;
using UnityEngine;

namespace OuterWives;

public class ColliderVisibilityTracker : MonoBehaviour
{
  private Collider _collider;
  private CharacterDialogueTree _character;
  private Transform _transform;
  private bool _visible;
  private bool _visibleToProbe;
  private float _raycastOffset = 10f;
  private float _maxPhotoDistance = 50f;

  private void Awake()
  {
    _collider = GetComponent<Collider>();
    _character = GetComponent<CharacterDialogueTree>();
    _transform = transform;
  }

  private void Start()
  {
    var sphere2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    sphere2.GetComponent<Collider>().enabled = false;
    sphere2.transform.parent = transform;
    sphere2.transform.position = GetTargetPosition();
    sphere2.name = $"THING {_character._characterName} attention position";
  }

  private void OnEnable()
  {
    GlobalMessenger<ProbeCamera>.AddListener("ProbeSnapshot", new Callback<ProbeCamera>(OnProbeSnapshot));
  }

  private void OnDisable()
  {
    GlobalMessenger<ProbeCamera>.RemoveListener("ProbeSnapshot", new Callback<ProbeCamera>(OnProbeSnapshot));
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

    return hit && ray.collider != _collider;
  }

  private Vector3 GetTargetPosition() {
    return _character._attentionPoint.position;
  }

  private void OnProbeSnapshot(ProbeCamera camera)
  {
    Vector3 vector = GetTargetPosition() - camera.transform.position;
    float magnitude = vector.magnitude;
    if (magnitude > this._maxPhotoDistance) return;
    
    if (!IsVisible(camera.GetOWCamera())) return;

    if (IsOccludedFromPosition(camera.transform.position)) return;

    NotificationManager.SharedInstance.PostNotification(new NotificationData($"Photographed {_character._characterName}"), false);
  }
}

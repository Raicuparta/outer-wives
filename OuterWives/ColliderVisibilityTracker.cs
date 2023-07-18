using System;
using UnityEngine;

public class ColliderVisibilityTracker : VisibilityTracker
{
  private Collider _collider;
  private Transform _transform;
  private bool _visible;
  private bool _visibleToProbe;

  private void Awake()
  {
    _collider = GetComponent<Collider>();
    _transform = transform;
  }

  private void Update()
  {
    this._visible = !this.IsOccludedFromPosition(Locator.GetPlayerCamera().transform.position);

    if (ProbeCamera.GetLastSnapshotCamera() != null)
    {
      var probeCamera = ProbeCamera.GetLastSnapshotCamera();
      this._visibleToProbe = GeometryUtility.TestPlanesAABB(probeCamera.GetFrustumPlanes(), this._collider.bounds) && !this.IsOccludedFromPosition(probeCamera.transform.position);
    }
  }

  public override bool IsVisibleUsingCameraFrustum()
  {
    return GeometryUtility.TestPlanesAABB(Locator.GetActiveCamera().GetFrustumPlanes(), this._collider.bounds) && !this.IsOccludedFromPosition(Locator.GetActiveCamera().transform.position);

  }

  public override bool IsVisible()
  {
    return _visible;
  }

  public override bool IsVisibleToProbe(OWCamera camera)
  {
    return _visibleToProbe;
  }

  public override bool IsPointInside(Vector3 worldPos)
  {
    return false;
  }

  private bool IsOccludedFromPosition(Vector3 worldPos)
  {
    return false;
    // return Physics.Linecast(worldPos, _transform.position, out RaycastHit _, OWLayerMask.quantumOcclusionMask);
  }
}

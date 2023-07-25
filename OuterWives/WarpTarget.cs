using UnityEngine;

namespace OuterWives;

public class WarpTarget: MonoBehaviour
{
    private SpawnPoint _spawnPoint;

    protected void Awake()
    {
        _spawnPoint = gameObject.GetAddComponent<SpawnPoint>();
    }

    public void WarpPlayerHere()
    {
        Locator.GetPlayerController().GetComponent<PlayerSpawner>().DebugWarp(_spawnPoint);
    }
}

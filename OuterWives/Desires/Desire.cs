using UnityEngine;

namespace OuterWives.Desires;

public abstract class Desire<TBehaviour> : MonoBehaviour where TBehaviour : MonoBehaviour
{
    public string Id => GetId(ObjectBehaviour);
    public abstract string DisplayName { get; }

    protected TBehaviour ObjectBehaviour;
    protected Wifey Wife;

    protected abstract TBehaviour GetObjectBehaviour();
    protected abstract string GetId(TBehaviour behaviour);

    public static TDesire Create<TDesire>(Wifey wife) where TDesire : Desire<TBehaviour>
    {
        var instance = wife.gameObject.AddComponent<TDesire>();
        instance.Wife = wife;
        return instance;
    }

    public bool IsMatch(TBehaviour otherObject)
    {
        return GetId(otherObject) == GetId(ObjectBehaviour);
    }

    private void Start()
    {
        ObjectBehaviour = GetObjectBehaviour();
    }
}

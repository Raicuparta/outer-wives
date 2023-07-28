using UnityEngine;

namespace OuterWives.Desires;

public abstract class Desire<TBehaviour> : MonoBehaviour, IDesire where TBehaviour : MonoBehaviour
{
    public string ObjectId => GetId(ObjectBehaviour);
    public bool IsFulfilled => WifeConditions.Get(TextIds.Conditions.Accepted(this), Wife);

    protected TBehaviour ObjectBehaviour;
    protected Wifey Wife;

    public abstract string TextId { get; }
    public abstract string DisplayName { get; }

    protected abstract TBehaviour GetObjectBehaviour();
    protected abstract string GetId(TBehaviour behaviour);

    public abstract void Present();

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

    protected virtual void Start()
    {
        ObjectBehaviour = GetObjectBehaviour();
        ObjectBehaviour.gameObject.GetAddComponent<WarpTarget>();
    }
}

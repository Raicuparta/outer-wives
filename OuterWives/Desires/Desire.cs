using System;
using UnityEngine;

namespace OuterWives.Desires;

public abstract class Desire<TBehaviour> : MonoBehaviour, IDesire where TBehaviour : MonoBehaviour
{
    public string ObjectId => GetId(ObjectBehaviour);
    public bool IsAccepted => GetCondition(TextIds.Conditions.Accepted);
    public bool IsPresented => GetCondition(TextIds.Conditions.Presented);
    public bool IsConsumed => GetCondition(TextIds.Conditions.Consumed);
    public bool IsSkipped => GetCondition(TextIds.Conditions.Skipped);

    protected TBehaviour ObjectBehaviour;
    protected Wifey Wife;

    public abstract string TextId { get; }
    public abstract string DisplayName { get; }

    protected abstract TBehaviour GetObjectBehaviour();
    protected abstract string GetId(TBehaviour behaviour);
    protected abstract void Consume();

    public abstract void Present();

    public static TDesire Create<TDesire>(Wifey wife) where TDesire : Desire<TBehaviour>
    {
        var instance = wife.gameObject.AddComponent<TDesire>();
        instance.Wife = wife;
        return instance;
    }

    protected bool IsMatch(TBehaviour otherObject)
    {
        if (otherObject == null) return false;

        return GetId(otherObject) == GetId(ObjectBehaviour);
    }

    protected virtual void Start()
    {
        ObjectBehaviour = GetObjectBehaviour();
        ObjectBehaviour.gameObject.GetAddComponent<WarpTarget>();
        Wife.Character.OnEndConversation += OnEndConversation;
    }

    protected virtual void OnDestroy()
    {
        Wife.Character.OnEndConversation -= OnEndConversation;
    }

    protected void SetPresented(bool presented)
    {
        SetCondition(TextIds.Conditions.Presented, presented);
    }

    protected void SetAccepted(bool accepted)
    {
        SetCondition(TextIds.Conditions.Accepted, accepted);
    }

    protected void SetConsumed(bool consumed)
    {
        SetCondition(TextIds.Conditions.Consumed, consumed);
    }

    protected void SetSkipped(bool skipped)
    {
        if (skipped) SetAccepted(true);
        SetCondition(TextIds.Conditions.Skipped, skipped);
    }

    private void SetCondition(Func<IDesire, string> condition, bool conditionValue)
    {
        WifeConditions.Set(condition(this), conditionValue, Wife);
    }

    private bool GetCondition(Func<IDesire, string> condition)
    {
        return WifeConditions.Get(condition(this), Wife);
    }

    private void OnEndConversation()
    {
        if (!IsAccepted || IsConsumed) return;

        Consume();
    }
}

namespace OuterWives.Desires;

public class ItemDesire : Desire<OWItem>
{
    public override string TextId => TextIds.Desires.Item;

    public override string DisplayName => ObjectBehaviour.GetDisplayName();

    protected override void Start()
    {
        base.Start();
        Wife.Character.OnEndConversation += ConsumeItem;
    }

    protected void OnDestroy()
    {
        Wife.Character.OnEndConversation -= ConsumeItem;
    }

    public override void Present()
    {
        var heldItem = GetHeldItem();
        if (heldItem == null) return;
        WifeConditions.Set(TextIds.Conditions.Presented(this), IsMatch(heldItem), Wife);
        Destroy(heldItem);
    }

    private OWItem GetHeldItem()
    {
        return Locator.GetToolModeSwapper().GetItemCarryTool().GetHeldItem();
    }

    protected override string GetId(OWItem behaviour)
    {
        return behaviour.GetItemType().ToString();
    }

    protected override OWItem GetObjectBehaviour()
    {
        return ThingFinder.Instance.GetItems().Get(Wife.Index);
    }

    private void ConsumeItem() // yummy
    {
        if (!WifeConditions.Get(TextIds.Conditions.Accepted(this), Wife)) return;

        Destroy(GetHeldItem().gameObject);
    }
}

namespace OuterWives.Desires;

public class ItemDesire : Desire<OWItem>
{
    public override string TextId => TextIds.Desires.Item;

    public override string DisplayName => ObjectBehaviour.GetDisplayName();

    public override void Present()
    {
        SetPresented(GetHeldItem() != null);
    }

    private OWItem GetHeldItem()
    {
        var item = Locator.GetToolModeSwapper().GetItemCarryTool().GetHeldItem();
        return IsMatch(item) ? item : null;
    }

    protected override string GetId(OWItem behaviour)
    {
        return behaviour.GetItemType().ToString();
    }

    protected override OWItem GetObjectBehaviour()
    {
        return ThingFinder.Instance.GetItems().Get(Wife.Index);
    }

    protected override void Consume()
    {
        Destroy(GetHeldItem()?.gameObject);
    }
}

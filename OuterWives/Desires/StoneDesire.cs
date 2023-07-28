namespace OuterWives.Desires;

public class StoneDesire : Desire<SharedStone>
{
    public override string TextId => TextIds.Desires.Stone;
    public override string DisplayName => NomaiRemoteCameraPlatform.IDToPlanetString(ObjectBehaviour._connectedPlatform);

    protected override void Start()
    {
        base.Start();
        Wife.Character.OnEndConversation += ConsumeStone;
    }

    protected void OnDestroy()
    {
        Wife.Character.OnEndConversation -= ConsumeStone;
    }

    protected override SharedStone GetObjectBehaviour()
    {
        return ThingFinder.Instance.GetStones().Get(Wife.Index);
    }

    protected override string GetId(SharedStone stone)
    {
        // Using translated display name as the ID,
        // which should be fine since you need to go back to the main menu to change the language I think.
        return DisplayName;
    }

    public override void Present()
    {
        WifeConditions.Set(TextIds.Conditions.Presented(this), GetHeldStone() != null, Wife);
    }

    private SharedStone GetHeldStone()
    {
        var heldItem = Locator.GetToolModeSwapper().GetItemCarryTool().GetHeldItem();
        if (heldItem is SharedStone stone && IsMatch(stone))
        {
            return stone;
        }
        return null;
    }

    private void ConsumeStone() // yummy
    {
        if (!WifeConditions.Get(TextIds.Conditions.Accepted(this), Wife)) return;

        Destroy(GetHeldStone().gameObject);
    }
}

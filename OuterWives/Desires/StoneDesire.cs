namespace OuterWives.Desires;

public class StoneDesire : Desire<SharedStone>
{
    public override string TextId => TextIds.Desires.Stone;
    public override string DisplayName => NomaiRemoteCameraPlatform.IDToPlanetString(ObjectBehaviour._connectedPlatform);

    protected override SharedStone GetObjectBehaviour()
    {
        return ThingFinder.Instance.GetRandomStone();
    }

    protected override string GetId(SharedStone stone)
    {
        // TODO: this is localized, not good for an ID.
        // Need to get an ID that's not localized, but is the same for every stone with the same drawing.
        return NomaiRemoteCameraPlatform.IDToPlanetString(stone._connectedPlatform);
    }

    public override void Present()
    {
        var condition = TextIds.Conditions.Presented(this);

        var heldItem = Locator.GetToolModeSwapper().GetItemCarryTool().GetHeldItem();
        var playerHasCorrectStone = heldItem is SharedStone stone && IsMatch(stone);
        WifeConditions.Set(condition, playerHasCorrectStone, Wife);
    }
}

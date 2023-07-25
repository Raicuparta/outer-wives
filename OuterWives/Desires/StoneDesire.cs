namespace OuterWives.Desires;

public class StoneDesire : Desire<SharedStone>
{
    public override string TextId => TextIds.Desires.Stone;
    public override string DisplayName => NomaiRemoteCameraPlatform.IDToPlanetString(ObjectBehaviour._connectedPlatform);

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
        var condition = TextIds.Conditions.Presented(this);

        var heldItem = Locator.GetToolModeSwapper().GetItemCarryTool().GetHeldItem();
        var playerHasCorrectStone = heldItem is SharedStone stone && IsMatch(stone);
        WifeConditions.Set(condition, playerHasCorrectStone, Wife);
    }
}

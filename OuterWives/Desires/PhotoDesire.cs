namespace OuterWives.Desires;

public class PhotoDesire : Desire<PhotogenicCharacter>
{
    public override string TextId => TextIds.Desires.Photo;
    public override string DisplayName => ObjectBehaviour.DisplayName;

    protected override PhotogenicCharacter GetObjectBehaviour()
    {
        return PhotoManager.Instance.GetRandomCharacter(Wife);
    }

    protected override string GetId(PhotogenicCharacter character)
    {
        return ObjectBehaviour.Id;
    }

    public override void Present()
    {
        SetPresented(PhotoManager.Instance.IsCharacterInShot(ObjectId));
    }

    protected override void Consume()
    {
        Locator.GetToolModeSwapper().UnequipTool();
    }
}

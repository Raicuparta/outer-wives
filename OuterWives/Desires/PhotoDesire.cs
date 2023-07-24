namespace OuterWives.Desires;

public class PhotoDesire : Desire<PhotogenicCharacter>
{
    public override string TextId => TextIds.Desires.Photo;
    public override string DisplayName => ObjectBehaviour.DisplayName;

    protected override PhotogenicCharacter GetObjectBehaviour()
    {
        return PhotoManager.Instance.GetRandomCharacter();
    }

    protected override string GetId(PhotogenicCharacter character)
    {
        return ObjectBehaviour.Id;
    }

    public override void Present()
    {
        var condition = TextIds.Conditions.Presented(this);

        WifeConditions.Set(condition, PhotoManager.Instance.IsCharacterInShot(ObjectId), Wife);
    }
}

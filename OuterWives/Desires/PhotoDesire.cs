namespace OuterWives.Desires;

public class PhotoDesire : Desire<PhotogenicCharacter>
{
    public override string DisplayName => ObjectBehaviour.DisplayName;

    protected override PhotogenicCharacter GetObjectBehaviour()
    {
        return PhotoManager.Instance.GetRandomCharacter();
    }

    protected override string GetId(PhotogenicCharacter character)
    {
        return ObjectBehaviour.Id;
    }
}

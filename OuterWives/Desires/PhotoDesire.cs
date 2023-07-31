using System.Collections.Generic;

namespace OuterWives.Desires;

public class PhotoDesire : Desire<PhotogenicCharacter>
{
    public override string TextId => TextIds.Desires.Photo;
    public override string DisplayName => ObjectBehaviour.DisplayName;

    private readonly Dictionary<string, string> _overrides = new()
    {
        { "Solanum", "Esker" }
    };

    protected override PhotogenicCharacter GetObjectBehaviour()
    {
        var hasOverride = _overrides.TryGetValue(Wife.Id, out var overrideId);
        var overrideCharacter = hasOverride ? PhotoManager.Instance.GetCharacter(overrideId) : null;
        return overrideCharacter ?? PhotoManager.Instance.GetRandomCharacter(Wife);
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

namespace OuterWives;

public class WifeMaterial
{
    private readonly CharacterDialogueTree _photoPreference;
    public string PhotoPreference => _photoPreference._characterName;
    //public string PhotoPreference => "Slate";

    private readonly SharedStone _stonePreference;
    public string StonePreference => NomaiRemoteCameraPlatform.IDToPlanetString(_stonePreference._connectedPlatform);

    private readonly TravelerController _musicPreference;
    public string MusicPreference => _musicPreference._audioSource.name.Replace("Signal_", "");

    public readonly CharacterDialogueTree Character;
    public string Name => Character._characterName;

    public WifeMaterial(string name, ThingFinder thingFinder, PhotoManager photoManager)
    {
        Character = thingFinder.GetCharacter(name);
        _photoPreference = thingFinder.GetRandomCharacter();
        _stonePreference = thingFinder.GetRandomStone();
        _musicPreference = thingFinder.GetRandomTraveler();

        photoManager.Characters.Add(Character.gameObject.AddComponent<PhotogenicCharacter>());

        Character.LoadXml();


        var rejectionNode = Character.AddNode(Constants.Nodes.Rejection, 2);
        foreach (var node in Character._mapDialogueNodes.Values)
        {
            if (node == rejectionNode) continue;

            node._listDialogueOptions.Clear();
            node.AddOption(Constants.Options.MarryMe, rejectionNode);
            node.AddOption(Constants.Options.Accept);
        }

        var requestPhotoNode = Character.AddNode(Constants.Nodes.RequestPhoto);
        var requestStoneNode = Character.AddNode(Constants.Nodes.RequestStone);
        var requestMusicNode = Character.AddNode(Constants.Nodes.RequestMusic);
        rejectionNode.AddOption(Constants.Options.ProposePhoto, requestPhotoNode);
        rejectionNode.AddOption(Constants.Options.ProposeStone, requestStoneNode);
        rejectionNode.AddOption(Constants.Options.ProposeMusic, requestMusicNode);
        rejectionNode.AddOption(Constants.Options.Accept);

        requestPhotoNode.AddOption(Constants.Options.ProposeStone, requestStoneNode);
        requestPhotoNode.AddOption(Constants.Options.ProposeMusic, requestMusicNode);
        requestPhotoNode.AddOption(Constants.Options.Accept);

        requestStoneNode.AddOption(Constants.Options.ProposePhoto, requestPhotoNode);
        requestStoneNode.AddOption(Constants.Options.ProposeMusic, requestMusicNode);
        requestStoneNode.AddOption(Constants.Options.Accept);

        requestMusicNode.AddOption(Constants.Options.ProposePhoto, requestPhotoNode);
        requestMusicNode.AddOption(Constants.Options.ProposeStone, requestStoneNode);
        requestMusicNode.AddOption(Constants.Options.Accept);

        var acceptPhotoNode = Character.AddNode(Constants.Nodes.AcceptPhoto, 1);
        foreach (var node in Character._mapDialogueNodes.Values)
        {
            if (node == acceptPhotoNode) continue;
            node.AddOption(Constants.Options.GivePhoto, acceptPhotoNode)
                .AddCondition(Constants.Conditions.GavePhoto, Character);
        }
    }

    public void GivePhoto()
    {
        var playerHasCorrectPhoto = PhotoPreference == PhotoManager.Instance.PhotographedCharacter?.Name;
        DialogueConditionManager.SharedInstance.SetWifeCondition(Constants.Conditions.GavePhoto, playerHasCorrectPhoto, Character);
    }
}

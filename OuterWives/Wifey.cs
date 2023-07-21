using System.Security.Policy;
using UnityEngine;

namespace OuterWives;

public class Wifey: MonoBehaviour
{
    private CharacterDialogueTree _photoPreference;
    public string PhotoPreference => _photoPreference._characterName;
    //public string PhotoPreference => "Slate";

    private SharedStone _stonePreference;
    public string StonePreference => NomaiRemoteCameraPlatform.IDToPlanetString(_stonePreference._connectedPlatform);

    private TravelerController _musicPreference;
    public string MusicPreference => _musicPreference._audioSource.name.Replace("Signal_", "");

    public CharacterDialogueTree Character;
    public string Name => Character._characterName;

    private Animator _animator;
    public bool Active => _animator.enabled;

    public static Wifey Create(string name)
    {
        var character = ThingFinder.Instance.GetCharacter(name);
        if (character == null)
        {
            OuterWives.Log($"Failed to find character for wife {name}");
            return null;
        }

        var wifey = character.gameObject.AddComponent<Wifey>();
        wifey.Character = character;

        return wifey;
    }

    private void Start()
    {
        _photoPreference = ThingFinder.Instance.GetRandomCharacter();
        _stonePreference = ThingFinder.Instance.GetRandomStone();
        _musicPreference = ThingFinder.Instance.GetRandomTraveler();
        _animator = Character.transform.parent.GetComponentInChildren<Animator>();

        // TODO: some characters that aren't wives can be photographed.
        PhotoManager.Instance.Characters.Add(Character.gameObject.AddComponent<PhotogenicCharacter>());

        Character.LoadXml();

        var rejectionNode = Character.AddNode(Constants.Nodes.Rejection, 2);
        var acceptOption = rejectionNode.AddOption(Constants.Options.Accept);

        foreach (var node in Character._mapDialogueNodes.Values)
        {
            if (node == rejectionNode) continue;

            node._listDialogueOptions.Clear();
            node.AddOption(Constants.Options.MarryMe, rejectionNode);
            node.AddOption(acceptOption);
        }

        var requestPhotoNode = Character.AddNode(Constants.Nodes.RequestPhoto);
        var requestStoneNode = Character.AddNode(Constants.Nodes.RequestStone);
        var requestMusicNode = Character.AddNode(Constants.Nodes.RequestMusic);

        var proposePhotoOption = rejectionNode.AddOption(Constants.Options.ProposePhoto, requestPhotoNode)
            .RejectCondition(Constants.Conditions.WifeAcceptedPhoto, Character);
        var proposeStoneOption = rejectionNode.AddOption(Constants.Options.ProposeStone, requestStoneNode);
        var proposeMusicOption = rejectionNode.AddOption(Constants.Options.ProposeMusic, requestMusicNode);

        requestPhotoNode.AddOption(proposeStoneOption);
        requestPhotoNode.AddOption(proposeMusicOption);
        requestPhotoNode.AddOption(acceptOption);

        requestStoneNode.AddOption(proposePhotoOption);
        requestStoneNode.AddOption(proposeMusicOption);
        requestStoneNode.AddOption(acceptOption);

        requestMusicNode.AddOption(proposePhotoOption);
        requestMusicNode.AddOption(proposeStoneOption);
        requestMusicNode.AddOption(acceptOption);

        var acceptPhotoNode = Character.AddNode(Constants.Nodes.AcceptPhoto, 1);
        foreach (var node in Character._mapDialogueNodes.Values)
        {
            if (node == acceptPhotoNode) continue;
            node.AddOption(Constants.Options.GivePhoto, acceptPhotoNode)
                .RequireCondition(Constants.Conditions.PlayerBroughtPhoto, Character)
                .GiveCondition(Constants.Conditions.WifeAcceptedPhoto, Character)
                .RejectCondition(Constants.Conditions.WifeAcceptedPhoto, Character);
        }
    }

    public void GivePhoto()
    {
        var playerHasCorrectPhoto = PhotoPreference == PhotoManager.Instance.PhotographedCharacter?.Name;
        DialogueConditionManager.SharedInstance.SetWifeCondition(Constants.Conditions.PlayerBroughtPhoto, playerHasCorrectPhoto, Character);
    }
}

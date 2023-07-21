using System;
using System.Collections.Generic;
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

    private float _playerNearbyDistance = 5f;

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

        SetUpDialogue();
    }

    private void Update()
    {
        if (Active && IsMusicPreferencePlaying() && IsNearPlayer())
        {
            var condition = DesireString.Conditions.Presented(DesireString.Desires.Music);
            if (DialogueConditionManager.SharedInstance.GetWifeCondition(condition, Character)) return;
            DialogueConditionManager.SharedInstance.SetWifeCondition(condition, true, Character);
        }
    }

    private void SetUpDialogue()
    {
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

        var desireNodes = new Dictionary<string, DialogueNode>();
        var desireOptions = new Dictionary<string, DialogueOption>();

        foreach (var desireId in DesireString.Desires.All)
        {
            var dialogueNode = desireNodes[desireId] = CreateDesire(desireId);

            desireOptions[desireId] = rejectionNode.AddOption(DesireString.Actions.Propose(desireId), dialogueNode)
                .RejectCondition(DesireString.Actions.Accept(desireId), Character);

            dialogueNode.AddOption(acceptOption);
        }

        foreach (var nodeDesireId in DesireString.Desires.All)
        {
            foreach (var optionDesireId in DesireString.Desires.All)
            {
                if (nodeDesireId == optionDesireId) continue;
                desireNodes[nodeDesireId].AddOption(desireOptions[optionDesireId]);
            }
        }
    }

    private DialogueNode CreateDesire(string desireId)
    {
        var requestNode = Character.AddNode(DesireString.Actions.Request(desireId));

        var acceptNode = Character.AddNode(DesireString.Actions.Accept(desireId), 1);

        foreach (var node in Character._mapDialogueNodes.Values)
        {
            if (node == acceptNode) continue;

            node.AddOption(DesireString.Actions.Present(desireId), acceptNode)
                .RequireCondition(DesireString.Conditions.Presented(desireId), Character)
                .GiveCondition(DesireString.Conditions.Accepted(desireId), Character)
                .RejectCondition(DesireString.Conditions.Accepted(desireId), Character);
        }

        return requestNode;
    }

    private bool IsNearPlayer()
    {
        return Vector3.Distance(Locator.GetPlayerTransform().position, transform.position) < _playerNearbyDistance;
    }

    private bool IsMusicPreferencePlaying()
    {
        var signalScope = Locator.GetToolModeSwapper().GetSignalScope();
        var signalStrength = signalScope.GetStrongestSignalStrength(AudioSignal.FrequencyToIndex(SignalFrequency.Traveler));

        return signalStrength == 1f && signalScope.GetStrongestSignal().name == _musicPreference._audioSource.name;
    }

    public void GivePhoto()
    {
        var condition = DesireString.Conditions.Presented(DesireString.Desires.Photo);

        var playerHasCorrectPhoto = PhotoPreference == PhotoManager.Instance.PhotographedCharacter?.Name;
        DialogueConditionManager.SharedInstance.SetWifeCondition(condition, playerHasCorrectPhoto, Character);
    }
}

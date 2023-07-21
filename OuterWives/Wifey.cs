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
    public string StonePreference => GetStoneName(_stonePreference);

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
            var condition = WifeDesires.Conditions.Presented(WifeDesires.Desires.Music);
            if (WifeConditions.Get(condition, this)) return;
            WifeConditions.Set(condition, true, this);
        }
    }

    private void SetUpDialogue()
    {
        Character.LoadXml();

        var rejectionNode = Character.AddNode(Constants.Nodes.RejectMarriage, 2);
        var acceptMarriageNode = Character.AddNode(Constants.Nodes.AcceptMarriage, 1);
        var acceptOption = rejectionNode.AddOption(Constants.Options.Ok);

        foreach (var node in Character._mapDialogueNodes.Values)
        {
            if (node == rejectionNode || node == acceptMarriageNode) continue;

            node._listDialogueOptions.Clear();
            node.AddOption(acceptOption);
            
            var proposeMarriageOption = node.AddOption(Constants.Options.ProposeMarriage, rejectionNode);
            proposeMarriageOption.RejectCondition(Constants.Conditions.ReadyToMarry, this);

            var confirmMarriageOption = node.AddOption(Constants.Options.ConfirmMarriage, acceptMarriageNode);
            confirmMarriageOption.RequireCondition(Constants.Conditions.ReadyToMarry, this);
        }

        var desireNodes = new Dictionary<string, DialogueNode>();
        var desireOptions = new Dictionary<string, DialogueOption>();

        foreach (var desireId in WifeDesires.Desires.All)
        {
            var dialogueNode = desireNodes[desireId] = CreateDesire(desireId);

            desireOptions[desireId] = rejectionNode.AddOption(WifeDesires.Actions.Propose(desireId), dialogueNode)
                .RejectCondition(WifeDesires.Conditions.Accepted(desireId), this);

            dialogueNode.AddOption(acceptOption);
        }

        foreach (var nodeDesireId in WifeDesires.Desires.All)
        {
            foreach (var optionDesireId in WifeDesires.Desires.All)
            {
                if (nodeDesireId == optionDesireId) continue;
                desireNodes[nodeDesireId].AddOption(desireOptions[optionDesireId]);
            }
        }
    }

    private DialogueNode CreateDesire(string desireId)
    {
        var requestNode = Character.AddNode(WifeDesires.Actions.Request(desireId));

        var acceptNode = Character.AddNode(WifeDesires.Actions.Accept(desireId), 1);

        foreach (var node in Character._mapDialogueNodes.Values)
        {
            if (node == acceptNode) continue;

            node.AddOption(WifeDesires.Actions.Present(desireId), acceptNode)
                .RequireCondition(WifeDesires.Conditions.Presented(desireId), this)
                .GiveCondition(WifeDesires.Conditions.Accepted(desireId), this)
                .RejectCondition(WifeDesires.Conditions.Accepted(desireId), this);
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

    public void PresentDesires()
    {
        PresentPhoto();
        PresentStone();
    }

    private void PresentPhoto()
    {
        var condition = WifeDesires.Conditions.Presented(WifeDesires.Desires.Photo);

        var playerHasCorrectPhoto = PhotoPreference == PhotoManager.Instance.PhotographedCharacter?.Name;
        WifeConditions.Set(condition, playerHasCorrectPhoto, this);
    }

    private string GetStoneName(SharedStone stone)
    {
        return NomaiRemoteCameraPlatform.IDToPlanetString(stone._connectedPlatform);
    }

    private void PresentStone()
    {
        var condition = WifeDesires.Conditions.Presented(WifeDesires.Desires.Stone);

        var heldItem = Locator.GetToolModeSwapper().GetItemCarryTool().GetHeldItem();
        var playerHasCorrectStone = heldItem is SharedStone stone && GetStoneName(stone) == StonePreference;
        WifeConditions.Set(condition, playerHasCorrectStone, this);
    }

    public bool HasFulfilledAllDesires()
    {
        foreach (var desireId in WifeDesires.Desires.All)
        {
            if (!WifeConditions.Get(WifeDesires.Conditions.Accepted(desireId), this)) return false;
        }
        return true;
    }
}

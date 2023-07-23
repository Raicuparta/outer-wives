using OuterWives.Desires;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

namespace OuterWives;

public class Wifey: MonoBehaviour
{
    public PhotoDesire PhotoDesire { get; private set; }
    public StoneDesire StoneDesire { get; private set; }
    public MusicDesire MusicDesire { get; private set; }

    public CharacterDialogueTree Character;
    public string Name => Character._characterName;

    private Animator _animator;
    public bool Active => _animator.enabled;

    private float _playerNearbyDistance = 5f;

    public static Wifey Create(CharacterDialogueTree character)
    {
        var wifey = character.gameObject.AddComponent<Wifey>();
        wifey.Character = character;

        return wifey;
    }

    private void Start()
    {
        PhotoDesire = PhotoDesire.Create<PhotoDesire>(this);
        StoneDesire = StoneDesire.Create<StoneDesire>(this);
        MusicDesire = MusicDesire.Create<MusicDesire>(this);

        _animator = Character.transform.parent.GetComponentInChildren<Animator>();

        SetUpDialogue();
    }

    private void Update()
    {
        if (Active && IsMusicPreferencePlaying() && IsNearPlayer())
        {
            var condition = TextIds.Conditions.Presented(TextIds.Desires.Music);
            if (WifeConditions.Get(condition, this)) return;
            WifeConditions.Set(condition, true, this);
        }
    }

    private void SetUpDialogue()
    {
        Character.LoadXml();

        var rejectionNode = Character.AddNode(TextIds.Nodes.RejectMarriage, 2);
        var acceptMarriageNode = Character.AddNode(TextIds.Nodes.AcceptMarriage, 1);
        var acceptOption = rejectionNode.AddOption(TextIds.Options.Ok);

        foreach (var node in Character._mapDialogueNodes.Values)
        {
            if (node == rejectionNode || node == acceptMarriageNode) continue;

            node._listDialogueOptions.Clear();
            node.AddOption(acceptOption);
            
            var proposeMarriageOption = node.AddOption(TextIds.Options.ProposeMarriage, rejectionNode);
            proposeMarriageOption.RejectCondition(TextIds.Conditions.ReadyToMarry, this);

            var confirmMarriageOption = node.AddOption(TextIds.Options.ConfirmMarriage, acceptMarriageNode);
            confirmMarriageOption.RequireCondition(TextIds.Conditions.ReadyToMarry, this);
        }

        var desireNodes = new Dictionary<string, DialogueNode>();
        var desireOptions = new Dictionary<string, DialogueOption>();

        foreach (var desireId in TextIds.Desires.All)
        {
            var dialogueNode = desireNodes[desireId] = CreateDesireDialogue(desireId);

            desireOptions[desireId] = rejectionNode.AddOption(TextIds.Actions.Propose(desireId), dialogueNode)
                .RejectCondition(TextIds.Conditions.Accepted(desireId), this);

            dialogueNode.AddOption(acceptOption);
        }

        foreach (var nodeDesireId in TextIds.Desires.All)
        {
            foreach (var optionDesireId in TextIds.Desires.All)
            {
                if (nodeDesireId == optionDesireId) continue;
                desireNodes[nodeDesireId].AddOption(desireOptions[optionDesireId]);
            }
        }
    }

    private DialogueNode CreateDesireDialogue(string desireId)
    {
        var requestNode = Character.AddNode(TextIds.Actions.Request(desireId));

        var acceptNode = Character.AddNode(TextIds.Actions.Accept(desireId), 1);

        foreach (var node in Character._mapDialogueNodes.Values)
        {
            if (node == acceptNode) continue;

            node.AddOption(TextIds.Actions.Present(desireId), acceptNode)
                .RequireCondition(TextIds.Conditions.Presented(desireId), this)
                .GiveCondition(TextIds.Conditions.Accepted(desireId), this)
                .RejectCondition(TextIds.Conditions.Accepted(desireId), this);
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

        return signalStrength == 1f && MusicDesire.IsMatch(signalScope.GetStrongestSignal());
    }

    public void PresentDesires()
    {
        PresentPhoto();
        PresentStone();
    }

    private void PresentPhoto()
    {
        var condition = TextIds.Conditions.Presented(TextIds.Desires.Photo);

        WifeConditions.Set(condition, PhotoManager.Instance.IsCharacterInShot(PhotoDesire.Id), this);
    }

    private void PresentStone()
    {
        var condition = TextIds.Conditions.Presented(TextIds.Desires.Stone);

        var heldItem = Locator.GetToolModeSwapper().GetItemCarryTool().GetHeldItem();
        var playerHasCorrectStone = heldItem is SharedStone stone && StoneDesire.IsMatch(stone);
        WifeConditions.Set(condition, playerHasCorrectStone, this);
    }

    public bool HasFulfilledAllDesires()
    {
        foreach (var desireId in TextIds.Desires.All)
        {
            if (!WifeConditions.Get(TextIds.Conditions.Accepted(desireId), this)) return false;
        }
        return true;
    }
}

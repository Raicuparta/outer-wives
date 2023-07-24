using OuterWives.Desires;
using System.Collections.Generic;
using UnityEngine;

namespace OuterWives;

public class Wifey: MonoBehaviour
{
    public List<IDesire> Desires { get; private set; } = new();

    public CharacterDialogueTree Character;
    public string Id => Character._characterName;

    private Animator _animator;
    public bool Active => _animator.enabled;

    public static Wifey Create(CharacterDialogueTree character)
    {
        var wifey = character.gameObject.AddComponent<Wifey>();
        wifey.Character = character;

        return wifey;
    }

    private void Start()
    {
        CreateDesires();

        _animator = Character.transform.parent.GetComponentInChildren<Animator>();

        SetUpDialogue();
    }

    private void OnEnable()
    {
        GlobalMessenger.AddListener("ExitConversation", OnExitConversation);
    }

    private void OnExitConversation()
    {
        if (!WifeConditions.Get(TextIds.Conditions.GettingMarried, this)) return;

        GetMarried();
    }

    private void GetMarried()
    {
        NotificationManager.SharedInstance.PostNotification(new NotificationData($"You got married to {Id}"), false);
    }

    private void CreateDesires()
    {
        Desires.Add(PhotoDesire.Create<PhotoDesire>(this));
        Desires.Add(StoneDesire.Create<StoneDesire>(this));
        Desires.Add(MusicDesire.Create<MusicDesire>(this));
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
            confirmMarriageOption.GiveCondition(TextIds.Conditions.GettingMarried, this);
        }

        var desireNodes = new Dictionary<string, DialogueNode>();
        var desireOptions = new Dictionary<string, DialogueOption>();

        foreach (var desire in Desires)
        {
            var dialogueNode = desireNodes[desire.TextId] = CreateDesireDialogue(desire);

            desireOptions[desire.TextId] = rejectionNode.AddOption(TextIds.Actions.Propose(desire), dialogueNode)
                .RejectCondition(TextIds.Conditions.Accepted(desire), this);

            dialogueNode.AddOption(acceptOption);
        }

        foreach (var nodeDesire in Desires)
        {
            foreach (var optionDesire in Desires)
            {
                if (Desires == Desires) continue;
                desireNodes[nodeDesire.TextId].AddOption(desireOptions[optionDesire.TextId]);
            }
        }
    }

    private DialogueNode CreateDesireDialogue(IDesire desire)
    {
        var requestNode = Character.AddNode(TextIds.Actions.Request(desire));

        var acceptNode = Character.AddNode(TextIds.Actions.Accept(desire), 1);

        foreach (var node in Character._mapDialogueNodes.Values)
        {
            if (node == acceptNode) continue;

            node.AddOption(TextIds.Actions.Present(desire), acceptNode)
                .RequireCondition(TextIds.Conditions.Presented(desire), this)
                .GiveCondition(TextIds.Conditions.Accepted(desire), this)
                .RejectCondition(TextIds.Conditions.Accepted(desire), this);
        }

        return requestNode;
    }

    public void PresentDesires()
    {
        foreach (var desire in Desires)
        {
            desire.Present();
        }
    }

    public bool HasFulfilledAllDesires()
    {
        foreach (var desire in Desires)
        {
            if (!desire.IsFulfilled) return false;
        }
        return true;
    }
}

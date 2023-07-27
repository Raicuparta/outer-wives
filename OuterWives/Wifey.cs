using OuterWives.Desires;
using OuterWives.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace OuterWives;

public class Wifey: MonoBehaviour
{
    public List<IDesire> Desires { get; private set; } = new();
    public int Index { get; private set; }

    public CharacterDialogueTree Character { get; private set; }
    public string Id => Character._characterName;
    public string DisplayName => TextTranslation.Translate(Character._characterName);
    public bool Active => !_animator || _animator.enabled;

    private Animator _animator;

    public static Wifey Create(CharacterDialogueTree character, int index)
    {
        var wifey = character.gameObject.AddComponent<Wifey>();
        wifey.Character = character;
        wifey.Index = index;

        return wifey;
    }

    protected void Start()
    {
        CreateDesires();
        SetUpAnimator();
        SetUpDialogue();

        gameObject.AddComponent<WarpTarget>();

        Character.OnStartConversation += OnStartConversation;
        Character.OnEndConversation += OnExitConversation;
    }

    protected void OnDestroy()
    {
        Character.OnStartConversation -= OnStartConversation;
        Character.OnEndConversation -= OnExitConversation;
    }

    private void OnStartConversation()
    {
        PresentDesires();
    }

    private void OnExitConversation()
    {
        if (!WifeConditions.Get(TextIds.Conditions.GettingMarried, this)) return;

        StartGetMarriedCoroutine();
    }

    private void CreateDesires()
    {
        Desires.Add(PhotoDesire.Create<PhotoDesire>(this));
        Desires.Add(StoneDesire.Create<StoneDesire>(this));
        Desires.Add(MusicDesire.Create<MusicDesire>(this));
    }

    private void SetUpAnimator()
    {
        _animator = Character.transform.parent.GetComponentInChildren<Animator>();
        if (!_animator)
        {
            OuterWives.Error($"Failed to find animator for Wife {Id}");
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

    private void PresentDesires()
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

    public override string ToString()
    {
        return $"{Id} ({DisplayName}): {string.Join(", ", Desires.Select(desire => $"{desire.TextId}={desire.DisplayName}"))}";
    }

    private void StartGetMarriedCoroutine()
    {
        StartCoroutine(GetMarried());
    }

    private IEnumerator GetMarried()
    {
        var effectController = Locator.GetPlayerCamera().GetComponent<PlayerCameraEffectController>();
        effectController.CloseEyes(1f);
        yield return new WaitForSeconds(1f);
        WifeManager.Instance.GetMarried(this);
        effectController.OpenEyes(1f);
    }
}

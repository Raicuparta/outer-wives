using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace OuterWives;

public static class DialogueExtensions
{
    public static DialogueNode AddNode(this CharacterDialogueTree character, string nodeTextId, int pageCount = 1)
    {
        var nodeName = $"{Constants.Global.Prefix}/{character._characterName}/{nodeTextId}";

        return character._mapDialogueNodes[nodeName] = new DialogueNode()
        {
            _name = nodeName,
            _listDialogueOptions = new(),
            _displayTextData = new DialogueText(new XElement[] { }, false)
            {
                _listTextBlocks = new List<DialogueText.TextBlock> {
                    new DialogueText.TextBlock
                    {
                        condition = "",
                        listPageText = Enumerable
                            .Range(1, pageCount)
                            .Select(index => $"_{index}")
                            .ToList()
                    }
                },
            },
        };
    }

    public static DialogueOption AddOption(this DialogueNode node, string optionTextId, DialogueNode target = null)
    {
        var option = new DialogueOption()
        {
            _textID = $"{Constants.Global.Prefix}/{optionTextId}",
            _targetName = target?.Name ?? "",
        };

        node.AddOption(option);

        return option;
    }

    public static DialogueOption AddOption(this DialogueNode node, DialogueOption option)
    {
        node._listDialogueOptions.Add(option);
        return option;
    }

    public static DialogueOption RequireCondition(this DialogueOption option, string conditionId, Wifey character)
    {
        option.ConditionRequirement = $"{Constants.Global.Prefix}/{character.Name}_{conditionId}";
        return option;
    }

    public static DialogueOption RejectCondition(this DialogueOption option, string conditionId, Wifey character)
    {
        option.CancelledRequirement = $"{Constants.Global.Prefix}/{character.Name}_{conditionId}";
        return option;
    }

    public static DialogueOption GiveCondition(this DialogueOption option, string conditionId, Wifey character)
    {
        option.ConditionToSet = $"{Constants.Global.Prefix}/{character.Name}_{conditionId}";
        return option;
    }
}

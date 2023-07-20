using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace OuterWives
{
    public static class DialogueExtensions
    {
        public static DialogueNode AddNode(this CharacterDialogueTree character, string nodeTextId, int pageCount = 1)
        {
            var nodeName = $"WIFE/{character._characterName}/{nodeTextId}";

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
                                .Select(index => $"_PART_{index}")
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
                _textID = $"WIFE/{optionTextId}",
                _targetName = target?.Name ?? ""
            };

            node._listDialogueOptions.Add(option);

            return option;
        }
    }
}

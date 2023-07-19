using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace OuterWives.Dialogue
{
    public class DialogueNodeData
    {
        [Required]
        public string name;
        public List<string> entryConditions = new();
        public List<string> setConditions = new();
        public string setPersistentCondition;
        public string cancelPersistentCondition;
        public List<string> revealFacts = new();
        public bool randomize;
        public List<DialogueTextData> dialogues = new();
        public List<string> targetShipLogConditions = new();
        public string target;
        public List<DialogueOptionData> options = new();
    }
}

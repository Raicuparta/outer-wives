using OWML.ModHelper;
using System.Linq;
using UnityEngine;

namespace OuterWives
{
    public class WifeManager: MonoBehaviour
    {
        public static WifeManager Instance;

        public WifeMaterial[] Wives { get; private set; }

        private void Awake()
        {
            OuterWives.Log("Wife Manager Awake");

            Instance = this;
            Wives = new[] {
                new WifeMaterial("Feldspar"),
                new WifeMaterial("Hal"),
                new WifeMaterial("Chert"),
                new WifeMaterial("Hornfels"),
                new WifeMaterial("Slate"),
                new WifeMaterial("Rutile"),
                new WifeMaterial("Gneiss"),
                new WifeMaterial("Marl"),
                new WifeMaterial("Tuff"),
                new WifeMaterial("Esker"),
                new WifeMaterial("Porphy"),
                new WifeMaterial("the Prisoner"), // TODO you can't get out of there, so we shouldn't ask for pictures.
                new WifeMaterial("Tektite"),
                new WifeMaterial("Gossan"),
                new WifeMaterial("Spinel"),
                new WifeMaterial("Gabbro"),
                new WifeMaterial("Riebeck"),
                new WifeMaterial("Self"),
                new WifeMaterial("Solanum"),
            };
        }

        private void Start()
        {
            foreach (var wife in Wives)
            {
                wife.Initialize();
            }

            var photoManager = new GameObject("PhotoManager").AddComponent<PhotoManager>();

            var characters = Resources.FindObjectsOfTypeAll<CharacterDialogueTree>()
                .Where(character => Wives.Any(wife => wife.name == character._characterName));

            foreach (var character in characters)
            {
                OuterWives.Log($"Character: {character._characterName} ({character.gameObject.activeInHierarchy})");
                photoManager.Characters.Add(character.gameObject.AddComponent<PhotogenicCharacter>());

                character.LoadXml();


                var rejectionNode = character.AddNode("REJECTION", 2);
                foreach (var node in character._mapDialogueNodes.Values)
                {
                    if (node == rejectionNode) continue;

                    OuterWives.Log($"{character._characterName}: Adding marry me option to node {node.Name}");
                    node._listDialogueOptions.Clear();
                    node.AddOption("MARRY_ME", rejectionNode);
                }

                var requestPhotoNode = character.AddNode("REQUEST_PHOTO");
                var requestStoneNode = character.AddNode("REQUEST_STONE");
                var requestMusicNode = character.AddNode("REQUEST_MUSIC");
                rejectionNode.AddOption("PROPOSE_PHOTO", requestPhotoNode);
                rejectionNode.AddOption("PROPOSE_STONE", requestStoneNode);
                rejectionNode.AddOption("PROPOSE_MUSIC", requestMusicNode);

                requestPhotoNode.AddOption("PROPOSE_STONE", requestStoneNode);
                requestPhotoNode.AddOption("PROPOSE_MUSIC", requestMusicNode);
                requestPhotoNode.AddOption("ACCEPT_REQUEST");

                requestStoneNode.AddOption("PROPOSE_PHOTO", requestPhotoNode);
                requestStoneNode.AddOption("PROPOSE_MUSIC", requestMusicNode);
                requestStoneNode.AddOption("ACCEPT_REQUEST");

                requestMusicNode.AddOption("PROPOSE_PHOTO", requestPhotoNode);
                requestMusicNode.AddOption("PROPOSE_STONE", requestStoneNode);
                requestMusicNode.AddOption("ACCEPT_REQUEST");
            }
        }
    }
}

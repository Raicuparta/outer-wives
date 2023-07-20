using UnityEngine;

namespace OuterWives
{
    public class WifeMaterial
    {
        private readonly CharacterDialogueTree _photoPreference;
        public string PhotoPreference => _photoPreference._characterName;

        private readonly SharedStone _stonePreference;
        public string StonePreference => NomaiRemoteCameraPlatform.IDToPlanetString(_stonePreference._connectedPlatform);

        private readonly TravelerController _musicPreference;
        public string MusicPreference => _musicPreference._audioSource.name.Replace("Signal_", "");

        public readonly CharacterDialogueTree Character;
        public string Name => Character._characterName;

        public WifeMaterial(string name, ThingFinder thingFinder, PhotoManager photoManager)
        {
            Character = thingFinder.GetCharacter(name);
            _photoPreference = thingFinder.GetRandomCharacter();
            _stonePreference = thingFinder.GetRandomStone();
            _musicPreference = thingFinder.GetRandomTraveler();

            photoManager.Characters.Add(Character.gameObject.AddComponent<PhotogenicCharacter>());

            Character.LoadXml();


            var rejectionNode = Character.AddNode("REJECTION", 2);
            foreach (var node in Character._mapDialogueNodes.Values)
            {
                if (node == rejectionNode) continue;

                OuterWives.Log($"{Character._characterName}: Adding marry me option to node {node.Name}");
                node._listDialogueOptions.Clear();
                node.AddOption("MARRY_ME", rejectionNode);
            }

            var requestPhotoNode = Character.AddNode("REQUEST_PHOTO");
            var requestStoneNode = Character.AddNode("REQUEST_STONE");
            var requestMusicNode = Character.AddNode("REQUEST_MUSIC");
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

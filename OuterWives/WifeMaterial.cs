using Delaunay;
using System;
using Unity.Collections;
using UnityEngine;

namespace OuterWives
{
    public class WifeMaterial
    {
        private readonly CharacterDialogueTree _photoPreference;
        //public string PhotoPreference => _photoPreference._characterName;
        public string PhotoPreference => "Slate";

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

                node._listDialogueOptions.Clear();
                node.AddOption("MARRY_ME", rejectionNode);
                node.AddOption("ACCEPT_REQUEST");
            }

            var requestPhotoNode = Character.AddNode("REQUEST_PHOTO");
            var requestStoneNode = Character.AddNode("REQUEST_STONE");
            var requestMusicNode = Character.AddNode("REQUEST_MUSIC");
            rejectionNode.AddOption("PROPOSE_PHOTO", requestPhotoNode);
            rejectionNode.AddOption("PROPOSE_STONE", requestStoneNode);
            rejectionNode.AddOption("PROPOSE_MUSIC", requestMusicNode);
            rejectionNode.AddOption("ACCEPT_REQUEST");

            requestPhotoNode.AddOption("PROPOSE_STONE", requestStoneNode);
            requestPhotoNode.AddOption("PROPOSE_MUSIC", requestMusicNode);
            requestPhotoNode.AddOption("ACCEPT_REQUEST");

            requestStoneNode.AddOption("PROPOSE_PHOTO", requestPhotoNode);
            requestStoneNode.AddOption("PROPOSE_MUSIC", requestMusicNode);
            requestStoneNode.AddOption("ACCEPT_REQUEST");

            requestMusicNode.AddOption("PROPOSE_PHOTO", requestPhotoNode);
            requestMusicNode.AddOption("PROPOSE_STONE", requestStoneNode);
            requestMusicNode.AddOption("ACCEPT_REQUEST");

            var acceptPhotoNode = Character.AddNode("ACCEPT_PHOTO", 1);
            foreach (var node in Character._mapDialogueNodes.Values)
            {
                if (node == acceptPhotoNode) continue;
                var givePhotoOption = node.AddOption("GIVE_PHOTO", acceptPhotoNode);
                givePhotoOption.ConditionRequirement = $"WIFE_{Name}_GAVE_PHOTO";
            }
        }

        public void GivePhoto()
        {
            var playerHasCorrectPhoto = PhotoPreference == PhotoManager.Instance.PhotographedCharacter?.Name;
            DialogueConditionManager.SharedInstance.SetConditionState($"WIFE_{Name}_GAVE_PHOTO", playerHasCorrectPhoto);
        }
    }
}

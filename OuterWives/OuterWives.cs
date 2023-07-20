using System.Linq;
using System.Reflection;
using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using UnityEngine;

namespace OuterWives;

public class OuterWives : ModBehaviour
{

    public static IModHelper Helper;

    public static readonly WifeMaterial[] Wives = new[] {
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
        new WifeMaterial("the Prisoner"),
        new WifeMaterial("Tektite"),
        new WifeMaterial("Gossan"),
        new WifeMaterial("Spinel"),
        new WifeMaterial("Gabbro"),
        new WifeMaterial("Riebeck"),
        new WifeMaterial("Self"),
        new WifeMaterial("Solanum"),
    };

    private void Awake()
    {
        // You won't be able to access OWML's mod helper in Awake.
        // So you probably don't want to do anything here.
        // Use Start() instead.
    }

    private void Start()
    {

        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

        Helper = ModHelper;

        // Starting here, you'll have access to OWML's mod helper.
        ModHelper.Console.WriteLine($"My mod {nameof(OuterWives)} is loaded!", MessageType.Success);


        // Example of accessing game code.
        LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
        {
            if (loadScene != OWScene.SolarSystem) return;

            ModHelper.Events.Unity.FireInNUpdates(() =>
            {
                foreach (var wife in Wives)
                {
                    wife.Initialize();
                }

                var characters = Resources.FindObjectsOfTypeAll<CharacterDialogueTree>()
                    .Where(character => Wives.Any(wife => wife.name == character._characterName));

                ModHelper.Console.WriteLine($"Found {characters.Count()} wife-material characters:", MessageType.Success);

                foreach (var character in characters)
                {
                    ModHelper.Console.WriteLine($"Character: {character._characterName} ({character.gameObject.activeInHierarchy})");
                    var visibilityTracker = character.gameObject.AddComponent<PhotogenicCharacter>();

                    character.LoadXml();


                    var rejectionNode = character.AddNode("REJECTION", 2);
                    foreach (var node in character._mapDialogueNodes.Values)
                    {
                        if (node == rejectionNode) continue;

                        ModHelper.Console.WriteLine($"{character._characterName}: Adding marry me option to node {node.Name}");
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

                var items = Resources.FindObjectsOfTypeAll<OWItem>();
                ModHelper.Console.WriteLine($"Found {items.Count()} items:", MessageType.Success);
                foreach (var item in items)
                {
                    ModHelper.Console.WriteLine($"Item: {item.GetDisplayName()} (interactable: {item._interactable})");
                }

            }, 100);

        };
    }
}


/*

Items

- Slide Reel (specify by slide?).
- Scroll (specify by text?).
- Projection Stone (specify by picture?).
- Solanum stones: (PROBLEM: they unload once you leave QM?)
	- Me (self's helmet)
	- Identify (nomai without mask)
	- Explain (nomai with mask)
	- You (nomai mask)
	- Eye of the Universe
	- Quantum Moon
- Owlk artifact (normal vs broken?).
- Owlk lantern.
- Owlk vision torch (PROBLEM: can't get it out in normal game)
- Warp core (small ones? big ones? what's the difference).

Picture

Characters could have a favorite thing/location/person, and you need to take a picture of it with the scout and show them.

Music

Characters could have a favorite instrument, and you have to use the signal scope to make them hear it.

*/

﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using UnityEngine;

namespace OuterWives;

public class WifeMaterial
{
    public string name;
    public WifeMaterial secretLove;

    internal void Initialize()
    {
        secretLove = OuterWives.Wives[Random.Range(0, OuterWives.Wives.Length)];
    }
}

public class OuterWives : ModBehaviour
{

    public static IModHelper Helper;

    public static readonly WifeMaterial[] Wives = new[] {
        new WifeMaterial() {
            name = "Feldspar"
        },
        new WifeMaterial() {
            name = "Hal"
        },
        new WifeMaterial() {
            name = "Chert"
        },
        new WifeMaterial() {
            name = "Hornfels"
        },
        new WifeMaterial() {
            name = "Slate"
        },
        new WifeMaterial() {
            name = "Rutile"
        },
        new WifeMaterial() {
            name = "Gneiss"
        },
        new WifeMaterial() {
            name = "Marl"
        },
        new WifeMaterial() {
            name = "Tuff"
        },
        new WifeMaterial() {
            name = "Esker"
        },
        new WifeMaterial() {
            name = "Porphy"
        },
        new WifeMaterial() {
            name = "the Prisoner"
        },
        new WifeMaterial() {
            name = "Tektite"
        },
        new WifeMaterial() {
            name = "Gossan"
        },
        new WifeMaterial() {
            name = "Spinel"
        },
        new WifeMaterial() {
            name = "Gabbro"
        },
        new WifeMaterial() {
            name = "Riebeck"
        },
        new WifeMaterial() {
            name = "Self"
        },
        new WifeMaterial() {
            name = "Solanum"
        }
    };

    private void Awake()
    {
        // You won't be able to access OWML's mod helper in Awake.
        // So you probably don't want to do anything here.
        // Use Start() instead.

        foreach (var wife in Wives)
        {
            wife.Initialize();
        }
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
                var characters = Resources.FindObjectsOfTypeAll<CharacterDialogueTree>()
                    .Where(character => Wives.Any(wife => wife.name == character._characterName));

                ModHelper.Console.WriteLine($"Found {characters.Count()} wife-material characters:", MessageType.Success);

                foreach (var character in characters)
                {
                    ModHelper.Console.WriteLine($"Character: {character._characterName} ({character.gameObject.activeInHierarchy})");
                    var visibilityTracker = character.gameObject.AddComponent<PhotogenicCharacter>();

                    character.LoadXml();


                    var rejectionNode = character.AddNode("REJECTION", 3);
                    foreach (var node in character._mapDialogueNodes.Values)
                    {
                        if (node == rejectionNode) return;

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
                    requestPhotoNode.AddOption("ACCEPT_REQUEST");
                    requestStoneNode.AddOption("ACCEPT_REQUEST");
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

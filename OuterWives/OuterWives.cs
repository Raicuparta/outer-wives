using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using OuterWives.Dialogue;
using OWML.Common;
using OWML.ModHelper;
using UnityEngine;

namespace OuterWives;

public class WifeMaterial
{
    public string name;
}

public class OuterWives : ModBehaviour
{

    public static IModHelper Helper;

    private readonly WifeMaterial[] wives = new[] {
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
    }

    string xml = @"
<DialogueTree xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:noNamespaceSchemaLocation=""https://raw.githubusercontent.com/xen-42/outer-wilds-new-horizons/main/NewHorizons/Schemas/dialogue_schema.xsd"">
	<NameField>Scoria</NameField>
	<DialogueNode>
		<Name>INTRO</Name>
		<EntryCondition>DEFAULT</EntryCondition>
		<Dialogue>
			<Page>Howdy there, hatchling! You remember me, right? Tektite's twin, Scoria? Used to work in the mines, before my back gave out.</Page>
			<Page>Anyways, I've been trying to find a purpose for all of these soft, worthless metals that Tuff and I dug up over the years.</Page>
			<Page>I figured it'd be nice to turn it into something useful, y'know, so... drum roll, please!...</Page>
			<Page>Presenting Scoria's golden marshmallows! Shiny metals molded into an easy, convenient to carry shape! Not that tasty though.</Page>
			<Page>You might be wondering, ""How's inedible confectionary useful, Scoria?"" Well, I reckon we could use 'em as a representation of value for tradin' stuff.</Page>
			<Page>So instead of saying ""I'll give ya three fish for your axe,"" you could exchange each fish for two marshmallows, and then six marshmallows for an axe. You followin', hatchling?</Page>
			<Page>Well, since you generously listened to my whole pitch, how about I give ya some to start? Maybe if an up-and-comer like yourself starts trading them, it'll finally catch on!</Page>
			<Page>I'll also exchange any odds and ends you find for marshmallows, and marshmallows for whatever I've got on hand. Come see me any time!</Page>
		</Dialogue>
		<SetPersistentCondition>ScoriaIntro</SetPersistentCondition>
		<DialogueTarget>MAIN</DialogueTarget>
	</DialogueNode>
	<DialogueNode>
		<Name>MAIN</Name>
		<EntryCondition>ScoriaIntro</EntryCondition>
		<Dialogue>
			<Page>Howdy there, hatchling! Got something to trade?</Page>
		</Dialogue>
		<DialogueOptionsList>
			<DialogueOption>
				<Text>What've you got?</Text>
				<DialogueTarget>SHOP_BUY</DialogueTarget>
			</DialogueOption>
			<DialogueOption>
				<Text>I have some items to exchange.</Text>
				<DialogueTarget>SHOP_SELL</DialogueTarget>
			</DialogueOption>
			<DialogueOption>
				<Text>I'd like to trade you back for an item I gave you.</Text>
				<DialogueTarget>SHOP_BUYBACK</DialogueTarget>
			</DialogueOption>
			<DialogueOption>
				<Text>Nothing today.</Text>
				<DialogueTarget>END</DialogueTarget>
			</DialogueOption>
		</DialogueOptionsList>
	</DialogueNode>
	<DialogueNode>
		<Name>END</Name>
		<Dialogue>
			<Page>Any time!</Page>
		</Dialogue>
	</DialogueNode>
</DialogueTree>
";

    private void Start()
    {
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
                    .Where(character => wives.Any(wife => wife.name == character._characterName));

                ModHelper.Console.WriteLine($"Found {characters.Count()} wife-material characters:", MessageType.Success);

                foreach (var character in characters)
                {
                    ModHelper.Console.WriteLine($"Character: {character._characterName} ({character.gameObject.activeInHierarchy})");
                    var visibilityTracker = character.gameObject.AddComponent<PhotogenicCharacter>();

                    character.LoadXml();

                    foreach (var node in character._mapDialogueNodes.Values)
                    {
                        node._listDialogueOptions.Clear();
                        node._listDialogueOptions.Add(new DialogueOption()
                        {
                            _textID = "Never mind that, will you marry me???",
                            _text = "bautiful text",
                            _targetName = "No. "
                        });
                    }

                    character._mapDialogueNodes["No. "] = new DialogueNode()
                    {
                        _name = "No. ",
                        _displayTextData = new DialogueText(new XElement[]{ }, false)
                        {
                            _listTextBlocks = new List<DialogueText.TextBlock> {
                                new DialogueText.TextBlock
                                {
                                    condition= "DEFAULT",
                                    listPageText = new List<string>
                                    {
                                        "I don't want to marry you",
                                    }
                                }
                            }
                        },
                    };
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

using Epic.OnlineServices;
using NewHorizons;
using NewHorizons.Builder.Props;
using NewHorizons.External.Modules;
using NewHorizons.External.Modules.Props;
using NewHorizons.External.SerializableData;
using NewHorizons.Utility.OuterWilds;
using NewHorizons.Utility.OWML;
using NewHorizons.Utility;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NewHorizons.Handlers;
using OuterWives.Extensions;

namespace OuterWives;

public class WifeManager: MonoBehaviour
{
    public static WifeManager Instance;

    private readonly Dictionary<string, Wifey> _wives = new();
    private readonly string[] _characterBlockList = new[]
{
        "Tephra",
        "Arkose",
        "Galena",
        "Moraine",
        "Mica",
    };

    public static void Create()
    {
        Instance = new GameObject(nameof(WifeManager)).AddComponent<WifeManager>();
    }

    private void Start()
    {
        var characters = ThingFinder.Instance.GetCharacters();
        for (var characterIndex = 0; characterIndex < characters.Array.Count; characterIndex++)
        {
            var character = characters.Array[characterIndex];
            if (_characterBlockList.Contains(character._characterName)) continue;

            CreateWife(character, characterIndex);
        }
        OuterWives.Helper.Events.Unity.FireInNUpdates(() =>
        {
            LogWives();
        }, 100);
    }

    public void GetMarried(Wifey wife)
    {
        var prefab = OuterWives.Assets.LoadAsset<GameObject>("OuterWives");
        var timberHearth = Locator.GetAstroObject(AstroObject.Name.TimberHearth);

        var instance = Instantiate(prefab, timberHearth.transform);
        var characterSlots = instance.transform.Find("CharacterSlots");
        var stage = instance.transform.Find("Stage/Cylinder");
        var guests = characterSlots.Find("Guests");
        var characters = ThingFinder.Instance.GetCharacters().Array
            .GroupBy(character => character._characterName)
            .Select(group => group.Last()).ToArray();

        for (var guestIndex = 0; guestIndex < guests.childCount; guestIndex++)
        {
            if (guestIndex >= _wives.Count) break;

            var character = characters[guestIndex];

            if (character == wife.Character) continue;

            var guestSpot = guests.GetChild(guestIndex);

            var clone = CloneCharacter(character, timberHearth, guestSpot);
            if (!clone) continue;
        }

        CloneCharacter(wife.Character, timberHearth, characterSlots.Find("WifeA"));
    }

    private GameObject CloneCharacter(CharacterDialogueTree character, AstroObject astroObject, Transform spot)
    {
        spot.DestroyAllChildren();

        var parent = character.transform.parent;
        var animator = parent.GetComponentInChildren<Animator>();
        if (!animator)
        {
            OuterWives.Error($"No Animator for {character._characterName}");
            return null;
        }

        var hit = Physics.Raycast(spot.position, -spot.up, out var hitInfo);
        if (!hit)
        {
            OuterWives.Error($"Failed to find floor for {spot.name}");
            return null;
        }

        var detailInfo = new DetailInfo()
        {
            position = astroObject.transform.InverseTransformPoint(hitInfo.point),
            rotation = spot.localEulerAngles,
            scale = animator.transform.lossyScale.x,
        };

        var clone = DetailBuilder.Make(astroObject.gameObject, astroObject.GetRootSector(), animator.gameObject, detailInfo);
        astroObject.GetRootSector().OnOccupantEnterSector.Invoke(Locator.GetPlayerSectorDetector());
        clone.transform.SetParent(spot);

        parent.gameObject.SetActive(false);

        return clone;
    }

    private void CreateWife(CharacterDialogueTree character, int characterIndex)
    {
        var wife = Wifey.Create(character, characterIndex);
        _wives[wife.Id] = wife;
    }

    public Wifey GetWife(string characterId)
    {
        _wives.TryGetValue(characterId, out var wife);
        return wife;
    }

    private void LogWives()
    {
        OuterWives.Log("## Start Logging Wives ##");
        foreach (var wife in _wives.Values)
        {
            OuterWives.Log($"- {wife}");
        }
        OuterWives.Log("## End Logging Wives ##");
    }
}

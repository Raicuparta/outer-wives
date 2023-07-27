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
            SetUpStage();
        }, 100);
    }

    private void SetUpStage()
    {
        var nh = new NewHorizonsApi();
//        nh.CreatePlanet(@$"{{
//    ""name"": ""TimberHearth"",
//    ""Props"": {{
        
//    }}
//}}", OuterWives.Instance);

        var prefab = OuterWives.Assets.LoadAsset<GameObject>("OuterWives");
        var timberHearth = Locator.GetAstroObject(AstroObject.Name.TimberHearth);

        var instance = Instantiate(prefab, timberHearth.transform);
        var characterSlots = instance.transform.Find("CharacterSlots");
        var stage = instance.transform.Find("Stage/Cylinder");
        var guests = characterSlots.Find("Guests");
        var wives = _wives.Values.ToArray();
        for (var guestIndex = 0; guestIndex < guests.childCount; guestIndex++)
        {
            if (guestIndex >= _wives.Count) break;

            var wife = wives[guestIndex].transform.parent;
            var guestSpot = guests.GetChild(guestIndex);

            var animator = wife.GetComponentInChildren<Animator>();
            if (!animator)
            {
                OuterWives.Error($"No Animator for {wives[guestIndex].DisplayName}");
                continue;
            }

            var hit = Physics.Raycast(guestSpot.position, -guestSpot.up, out var hitInfo);
            if (!hit)
            {
                OuterWives.Error($"Failed to find floor for {guestSpot.name}");
                continue;
            }

            var detailInfo = new DetailInfo()
            {
                position = timberHearth.transform.InverseTransformPoint(hitInfo.point),
                rotation = guestSpot.localEulerAngles,
                scale = animator.transform.lossyScale.x,
            };

            //wife.gameObject.SetActive(false);

            //var prop = GeneralPropBuilder.MakeFromExisting(wife.gameObject, timberHearth.gameObject, timberHearth.GetRootSector(), detailInfo);
            //StreamingHandler.SetUpStreaming(prop, timberHearth.GetRootSector());

            var clone = DetailBuilder.Make(timberHearth.gameObject, timberHearth.GetRootSector(), animator.gameObject, detailInfo);
            timberHearth.GetRootSector().OnOccupantEnterSector.Invoke(Locator.GetPlayerSectorDetector());



            //Destroy(clone.GetComponentInChildren<Wifey>());
            //clone.SetActive(true);

            wife.gameObject.SetActive(true);

            //wife.SetParent(guests.GetChild(guestIndex));
            //wife.localPosition = Vector3.zero;
            //wife.localRotation = Quaternion.identity;

            //wife.LookAt(stage, -stage.up);
        }

        //foreach (var streamingGroup in Resources.FindObjectsOfTypeAll<StreamingGroup>())
        //{
        //    streamingGroup.LoadGeneralAssets();
        //}
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

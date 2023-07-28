using NewHorizons.Builder.Props;
using NewHorizons.External.Modules.Props;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

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

    private readonly string _runtimeAnimatorName = "Villager_Arkose";

    private void Start()
    {
        var characters = ThingFinder.Instance.GetCharacters();
        for (var characterIndex = 0; characterIndex < characters.Array.Count; characterIndex++)
        {
            var character = characters.Array[characterIndex];

            if (_characterBlockList.Contains(character._characterName)
                || character.transform.parent.GetComponent<Sector>())
            {
                continue;
            }

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
            .Where(character => !character.transform.parent.GetComponent<Sector>())
            .GroupBy(character => character._characterName)
            .Select(group => group.Last()).ToArray();

        var animatorController = characters
            .Select(character => character.transform.parent.GetComponentInChildren<Animator>()?.runtimeAnimatorController)
            .First(animatorController => animatorController.name == _runtimeAnimatorName);

        var guestIndex = 0;

        var wifeClone = CloneCharacter(wife.Character, timberHearth, characterSlots.Find("WifeA"), animatorController);

        foreach (var character in characters)
        {
            var guestSlot = guests.GetChild(guestIndex);

            if (character == wife.Character) continue;
            var clone = CloneCharacter(character, timberHearth, guestSlot, animatorController, stage);

            guestIndex++;
            if (guestIndex >= guests.childCount)
            {
                break;
            };
        }
    }

    private GameObject CloneCharacter(CharacterDialogueTree character, AstroObject astroObject, Transform slot, RuntimeAnimatorController animatorController, Transform lookAt = null)
    {
        var parent = character.transform.parent;
        var animator = parent.GetComponentInChildren<Animator>();
        if (!animator)
        {
            OuterWives.Error($"No Animator for {character._characterName}");
            return null;
        }

        var hit = Physics.Raycast(slot.position, -slot.up, out var hitInfo);
        if (!hit)
        {
            OuterWives.Error($"Failed to find floor for {slot.name}");
            return null;
        }

        slot.position = hitInfo.point;
        if (lookAt)
        {
            var forward = Vector3.ProjectOnPlane(lookAt.position - slot.position, hitInfo.normal);
            slot.rotation = Quaternion.LookRotation(forward, hitInfo.normal);
        }

        var detailInfo = new DetailInfo()
        {
            position = slot.localPosition,
            rotation = slot.localEulerAngles,
            scale = animator.transform.lossyScale.x,
        };

        var collider = slot.GetComponentInChildren<Collider>();
        if (collider)
        {
            collider.enabled = true;
        }

        animator.gameObject.SetActive(false);
        var clone = DetailBuilder.Make(astroObject.gameObject, astroObject.GetRootSector(), animator.gameObject, detailInfo);
        DestroyComponents(clone);
        foreach (Transform child in clone.transform)
        {
            DestroyComponents(child.gameObject);
        }
        clone.SetActive(true);

        clone.GetComponent<Animator>().runtimeAnimatorController = animatorController;
        astroObject.GetRootSector().OnOccupantEnterSector.Invoke(Locator.GetPlayerSectorDetector());
        clone.transform.SetParent(slot);

        parent.gameObject.SetActive(false);

        return clone;
    }

    private readonly Type[] _allowedTypes = new[]
    {
        typeof(StreamingMeshHandle),
        typeof(OWRenderer),
        typeof(CullGroup),
    };

    private void DestroyComponents(GameObject obj)
    {
        foreach (var childComponent in obj.GetComponentsInChildren<MonoBehaviour>())
        {
            if (_allowedTypes.Any(type => type.IsAssignableFrom(childComponent.GetType()))) continue;
            Destroy(childComponent);
        }
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

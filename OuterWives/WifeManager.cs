﻿using NewHorizons.Builder.Props;
using NewHorizons.External.Modules.Props;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
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
    private readonly string[] _guestAnimatorControllers = new[]
    {
        "Villager_Tektite",
        "Villager_Arkose",
        "Villager_Marl",
        "Villager_Galena",
        "Villager_Hal_Museum",
        "Villager_Mica",
        "Villager_Spinel",
    };
    private GameObject _weddingAssetInstance;

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

            if (_characterBlockList.Contains(character._characterName)
                || character.transform.parent.GetComponent<Sector>())
            {
                continue;
            }

            CreateWife(character, characterIndex);
        }
        //OuterWives.Helper.Events.Unity.FireInNUpdates(() =>
        //{
        //    LogWives();
        //    SetUpAltar(_wives.First().Value);
        //}, 100);
    }

    public void SetUpAltar(Wifey wife)
    {
        var prefab = OuterWives.Assets.LoadAsset<GameObject>("OuterWives");
        var timberHearth = Locator.GetAstroObject(AstroObject.Name.TimberHearth);

        _weddingAssetInstance = Instantiate(prefab, timberHearth.transform);
        var characterSlots = _weddingAssetInstance.transform.Find("CharacterSlots");
        var stage = _weddingAssetInstance.transform.Find("Stage/Cylinder");
        var guests = characterSlots.Find("Guests");
        var characters = ThingFinder.Instance.GetCharacters().Array
            .Where(character => !character.transform.parent.GetComponent<Sector>())
            .GroupBy(character => character._characterName)
            .Select(group => group.Last()).ToArray();

        var animatorControllers = characters
            .Select(character => character.transform.parent.GetComponentInChildren<Animator>()?.runtimeAnimatorController)
            .Where(animatorController => _guestAnimatorControllers.Contains(animatorController.name)).ToShuffledArray();

        var guestIndex = 0;

        for (var characterIndex = 0; characterIndex < characters.Length; characterIndex++)
        {
            var character = characters[characterIndex];
            var animatorController = animatorControllers.Get(characterIndex);
            if (character == wife.Character)
            {
                var wifeSlot = characterSlots.Find("WifeA");
                var wifeClone = CloneCharacter(character, timberHearth, characterSlots.Find("WifeA"), animatorController, stage);
                wifeClone.layer = LayerMask.NameToLayer("Interactible");
                var interactReceiver = wifeSlot.GetComponentInChildren<Collider>().gameObject.GetAddComponent<InteractReceiver>();
                interactReceiver.OnPressInteract += () =>
                {
                    StartCoroutine(wife.Marry());
                };
                interactReceiver.SetPromptText(UITextType.TalkPrompt);
                continue;
            }
            var guestSlot = guests.GetChild(guestIndex);
            CloneCharacter(character, timberHearth, guestSlot, animatorController, stage);

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
            // Remove character chairs, stand up you lazy bastards.
            if (child.name.EndsWith("_Rocker"))
            {
                child.gameObject.SetActive(false);
            }
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

    private void DebugAnimations(string runtimeAnimatorName)
    {
        var characters = ThingFinder.Instance.GetCharacters();

        var animatorController = characters.Array
            .Select(character => character.transform.parent.GetComponentInChildren<Animator>(true)?.runtimeAnimatorController)
            .First(animatorController => animatorController.name == runtimeAnimatorName);

        foreach (var animator in _weddingAssetInstance.GetComponentsInChildren<Animator>())
        {
            animator.runtimeAnimatorController = animatorController;
        }
    }
}

/*
 * Villager_Tektite 
 * Villager_Arkose
 * Villager_Marl
 * Villager_Galena
 * Villager_Gossan
 * Villager_Hal_Museum
 * Villager_Mica
 * Villager_Spinel
 */
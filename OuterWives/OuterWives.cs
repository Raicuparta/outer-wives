using System.Reflection;
using HarmonyLib;
using NewHorizons.Utility;
using OWML.Common;
using OWML.ModHelper;
using UnityEngine;

namespace OuterWives;

public class OuterWives : ModBehaviour
{
    public static IModHelper Helper { get; private set; }
    public static AssetBundle Assets { get; private set; }
    public static OuterWives Instance { get; internal set; }

    public static void Log(string text)
    {
        Helper.Console.WriteLine(text, MessageType.Info);
    }

    public static void Error(string text)
    {
        Helper.Console.WriteLine(text, MessageType.Error);
    }

    protected void Awake()
    {
        Instance = this;
    }

    protected void Start()
    {
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

        Helper = ModHelper;

        Assets = ModHelper.Assets.LoadBundle("Assets/outer-wives");

        LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
        {
            if (loadScene != OWScene.SolarSystem) return;

            ModHelper.Events.Unity.FireInNUpdates(() =>
            {
                ThingFinder.Create();
                PhotoManager.Create();
                WifeManager.Create();
            }, 100); // TODO ewww
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

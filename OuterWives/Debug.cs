using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OuterWives;

public static class Debug
{

    public static void GoToDreamWorld()
    {
        var relativeLocation = new RelativeLocationData(Vector3.up * 2 + Vector3.forward * 2, Quaternion.identity, Vector3.zero);

        var location = Keyboard.current[Key.LeftShift].isPressed ? DreamArrivalPoint.Location.Zone4 : DreamArrivalPoint.Location.Zone3;
        var arrivalPoint = Locator.GetDreamArrivalPoint(location);
        var dreamCampfire = Locator.GetDreamCampfire(location);
        if (Locator.GetToolModeSwapper().GetItemCarryTool().GetHeldItemType() != ItemType.DreamLantern)
        {
            var dreamLanternItem = Resources.FindObjectsOfTypeAll<DreamLanternItem>().Where(lantern => lantern.GetLanternType() == DreamLanternType.Functioning).FirstOrDefault();
            Locator.GetToolModeSwapper().GetItemCarryTool().PickUpItemInstantly(dreamLanternItem);
        }

        Locator.GetDreamWorldController().EnterDreamWorld(dreamCampfire, arrivalPoint, relativeLocation);
    }
}

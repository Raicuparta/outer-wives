namespace OuterWives;

public static class WifeConditions
{
    private static string GetFullConditionId(string conditionId, Wifey wife)
    {
        return $"{TextIds.Prefix}/{wife.Id}_{conditionId}";
    }

    public static void Set(string conditionId, bool conditionState, Wifey wife)
    {
        DialogueConditionManager.SharedInstance
            .SetConditionState(GetFullConditionId(conditionId, wife), conditionState);

        if (wife.HasFulfilledAllDesires())
        {
            DialogueConditionManager.SharedInstance
            .SetConditionState(GetFullConditionId(TextIds.Conditions.ReadyToMarry, wife), true);
        }
    }

    public static bool Get(string conditionId, Wifey wife)
    {
        return DialogueConditionManager.SharedInstance
            .GetConditionState(GetFullConditionId(conditionId, wife));
    }
}

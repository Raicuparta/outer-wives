using UnityEngine;

namespace OuterWives.Desires;

public class MusicDesire : Desire<AudioSignal>
{
    public override string TextId => TextIds.Desires.Music;
    // TODO this is not localized, but I doubt there's anywhere in the game that translates just the instrument.
    // Might need to add it to the translation files.
    public override string DisplayName => ObjectBehaviour.name.Replace("Signal_", "");

    private readonly float _playerNearbyDistance = 5f;

    protected override string GetId(AudioSignal signal)
    {
        return signal.name;
    }

    protected override AudioSignal GetObjectBehaviour()
    {
        return ThingFinder.Instance.GetRandomMusicSignal();
    }

    public override void Present()
    {
    }

    protected void Update()
    {
        if (Wife.Active && IsMusicPreferencePlaying() && IsNearPlayer())
        {
            var condition = TextIds.Conditions.Presented(this);
            if (WifeConditions.Get(condition, Wife)) return;
            WifeConditions.Set(condition, true, Wife);
        }
    }

    private bool IsMusicPreferencePlaying()
    {
        var signalScope = Locator.GetToolModeSwapper().GetSignalScope();
        var signalStrength = signalScope.GetStrongestSignalStrength(AudioSignal.FrequencyToIndex(SignalFrequency.Traveler));

        return signalStrength == 1f && IsMatch(signalScope.GetStrongestSignal());
    }

    private bool IsNearPlayer()
    {
        return Vector3.Distance(Locator.GetPlayerTransform().position, transform.position) < _playerNearbyDistance;
    }
}

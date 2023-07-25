using UnityEngine;

namespace OuterWives.Desires;

public class MusicDesire : Desire<AudioSignal>
{
    public override string TextId => TextIds.Desires.Music;
    // TODO this is not localized, but I doubt there's anywhere in the game that translates just the instrument.
    // Might need to add it to the translation files.
    public override string DisplayName => ObjectBehaviour.name.Replace("Signal_", "");

    private readonly float _playerNearbySquareDistance = 25f;

    protected override string GetId(AudioSignal signal)
    {
        return signal.name;
    }

    protected override AudioSignal GetObjectBehaviour()
    {
        return ThingFinder.Instance.GetMusicSignals().Get(Wife.Index);
    }

    public override void Present()
    {
        var signalscope = Locator.GetToolModeSwapper().GetSignalScope();
        signalscope.UpdateAudioSignals();
        signalscope.SelectFrequency(SignalFrequency.Traveler);
        var travelerSignal = signalscope.GetStrongestSignal();
        if (travelerSignal == null || !travelerSignal.CanBePickedUpByScope())
        {
            // Can't get signals in some places, so if we're in there, we'll skip this desire.
            WifeConditions.Set(TextIds.Conditions.Accepted(this), true, Wife);
        }
    }

    protected void Update()
    {
        var condition = TextIds.Conditions.Presented(this);
        if (WifeConditions.Get(condition, Wife)) return;
        if (Wife.Active && IsMusicPreferencePlaying() && IsNearPlayer())
        {
            WifeConditions.Set(condition, true, Wife);
            // TODO translate
            NotificationManager.SharedInstance.PostNotification(new NotificationData($"Played {DisplayName} music near {Wife.DisplayName}"));
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
        return Vector3.SqrMagnitude(Locator.GetPlayerTransform().position - transform.position) < _playerNearbySquareDistance;
    }
}

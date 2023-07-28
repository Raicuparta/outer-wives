using UnityEngine;

namespace OuterWives.Desires;

public class MusicDesire : Desire<AudioSignal>
{
    public override string TextId => TextIds.Desires.Music;
    public override string DisplayName => TranslationManager.Instance.GetText(TextIds.Desires.Instrument(this));

    private readonly float _playerNearbySquareDistance = 25f;

    protected override string GetId(AudioSignal signal)
    {
        return signal.name.Replace("Signal_", "");
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
            OuterWives.Notify(TranslationManager.Instance.GetText(TextIds.Information.Music, new()
            {
                { TextIds.Tokens.Preference(this), DisplayName },
                { TextIds.Tokens.CharacterName, Wife.DisplayName },
            }));
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

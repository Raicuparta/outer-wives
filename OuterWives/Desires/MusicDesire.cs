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
        // We don't want travelers to requets their own music because that would be silly and this is a SERIOUS mod.
        var travelerSignal = transform.parent.GetComponent<TravelerController>()?._audioSource.GetComponent<AudioSignal>();

        return ThingFinder.Instance.GetMusicSignals().Get(Wife.Index, travelerSignal);
    }

    public override void Present()
    {
        var signalscope = Locator.GetToolModeSwapper().GetSignalScope();
        signalscope.UpdateAudioSignals();
        signalscope.SelectFrequency(SignalFrequency.Traveler);

        if (!CanPickUpSignal())
        {
            // Can't get signals in some places, so if we're in there, we'll skip this desire.
            WifeConditions.Set(TextIds.Conditions.Accepted(this), true, Wife);
        }
    }

    private bool CanPickUpSignal()
    {
        if (ObjectBehaviour.CanBePickedUpByScope()) return true;

        // There are multiple copies of the same signal, so we need to check them all.
        foreach (var signal in Resources.FindObjectsOfTypeAll<AudioSignal>())
        {
            if (signal.GetName() == ObjectBehaviour.GetName() && signal.CanBePickedUpByScope())
            {
                return true;
            }
        }

        return false;
    }

    protected void Update()
    {
        var condition = TextIds.Conditions.Presented(this);
        if (WifeConditions.Get(condition, Wife)) return;
        if (Wife.Active && IsMusicDesirePlaying() && IsNearPlayer())
        {
            WifeConditions.Set(condition, true, Wife);
            OuterWives.Notify(TranslationManager.Instance.GetText(TextIds.Information.Music, new()
            {
                { TextIds.Tokens.Desire(this), DisplayName },
                { TextIds.Tokens.CharacterName, Wife.DisplayName },
            }));
        }
    }

    private bool IsMusicDesirePlaying()
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

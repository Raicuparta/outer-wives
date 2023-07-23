namespace OuterWives.Desires;

public class MusicDesire : Desire<AudioSignal>
{
    public override string DisplayName => ObjectBehaviour.name.Replace("Signal_", ""); // TODO this is not localized

    protected override string GetId(AudioSignal signal)
    {
        return signal.name;
    }

    protected override AudioSignal GetObjectBehaviour()
    {
        return ThingFinder.Instance.GetRandomMusicSignal();
    }
}

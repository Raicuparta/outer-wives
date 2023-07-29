namespace OuterWives.Desires;

public interface IDesire
{
    string ObjectId{ get; }
    string TextId { get; }
    string DisplayName { get; }
    bool IsAccepted { get; }

    void Present();
}

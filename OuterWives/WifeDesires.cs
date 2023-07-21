namespace OuterWives;

public static class WifeDesires
{
    private static string Id(params string[] ids)
    {
        return string.Join("_", ids);
    }

    public static class Desires
    {
        public const string Photo = "PHOTO";
        public const string Stone = "STONE";
        public const string Music = "MUSIC";
        public static readonly string[] All = new[]
        {
            Photo,
            Stone,
            Music,
        };
    }

    public static class Actions
    {
        public static string Propose(string request) => Id("PROPOSE", request);
        public static string Present(string request) => Id("PRESENT", request);
        public static string Accept(string request) => Id("ACCEPT", request);
        public static string Request(string request) => Id("REQUEST", request);
    }

    public static class Conditions
    {
        public static string Presented(string request) => Id("PRESENTED", request);
        public static string Accepted(string request) => Id("ACCEPTED", request);
    }
}

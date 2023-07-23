namespace OuterWives;

public static class TextIds
{
    public const string Prefix = "WIFE";

    public static class Nodes
    {
        public const string RejectMarriage = "REJECT_MARRIAGE";
        public static string AcceptMarriage = "ACCEPT_MARRIAGE";
    }

    public static class Options
    {
        public const string ProposeMarriage = "PROPOSE_MARRIAGE";
        public const string Ok = "OK";
        public static string ConfirmMarriage = "CONFIRM_MARRIAGE";
    }

    public static class Tokens
    {
        public const string PhotoPreference = "$PHOTO_PREFERENCE$";
        public const string StonePreference = "$STONE_PREFERENCE$";
        public const string MusicPreference = "$MUSIC_PREFERENCE$";
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
        public static string Propose(string request) => Join("PROPOSE", request);
        public static string Request(string request) => Join("REQUEST", request);
        public static string Present(string request) => Join("PRESENT", request);
        public static string Accept(string request) => Join("ACCEPT", request);
    }

    public static class Conditions
    {
        public static string Presented(string request) => Join("PRESENTED", request);
        public static string Accepted(string request) => Join("ACCEPTED", request);

        public const string ReadyToMarry = "READY_TO_MARRY";
    }

    private static string Join(params string[] ids)
    {
        return string.Join("_", ids);
    }
}

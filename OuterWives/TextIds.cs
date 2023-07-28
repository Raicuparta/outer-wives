using OuterWives.Desires;

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
        public static string Preference(IDesire desire) => $"${Join(desire.TextId, "PREFERENCE")}$";
        public const string CharacterName = "$CHARACTER_NAME$";
    }

    public static class Desires
    {
        public const string Photo = "PHOTO";
        public const string Stone = "STONE";
        public const string Music = "MUSIC";
        public static string Item = "ITEM";

        public static string Instrument(MusicDesire musicDesire) => Join("INSTRUMENT", musicDesire.ObjectId.ToUpperInvariant());
    }

    public static class Actions
    {
        public static string Propose(IDesire desire) => Join("PROPOSE", desire.TextId);
        public static string Request(IDesire desire) => Join("REQUEST", desire.TextId);
        public static string Present(IDesire desire) => Join("PRESENT", desire.TextId);
        public static string Accept(IDesire desire) => Join("ACCEPT", desire.TextId);
    }

    public static class Conditions
    {
        public static string Presented(IDesire desire) => Join("PRESENTED", desire.TextId);
        public static string Accepted(IDesire desire) => Join("ACCEPTED", desire.TextId);

        public const string ReadyToMarry = "READY_TO_MARRY";
        public const string GettingMarried = "GETTING_MARRIED";
    }

    public static class Information
    {
        private const string Base = "INFORMATION";
        public static string Music => Join(Base, Desires.Music);
        public static string Photo => Join(Base, Desires.Photo);
        public static string Married => Join(Base, "MARRIED");
    }

    private static string Join(params string[] ids)
    {
        return string.Join("_", ids);
    }
}

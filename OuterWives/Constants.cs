namespace OuterWives;

public static class Constants
{
    public static class Global
    {
        public const string Prefix = "WIFE";
    }

    public static class Nodes
    {
        public const string Rejection = "REJECTION";
        public const string RequestPhoto = "REQUEST_PHOTO";
        public const string RequestStone = "REQUEST_STONE";
        public const string RequestMusic = "REQUEST_MUSIC";
        public const string AcceptPhoto = "ACCEPT_PHOTO";
        public const string AcceptStone = "ACCEPT_STONE";
        public const string AcceptMusic = "ACCEPT_MUSIC";
    }

    public static class Options
    {
        public const string MarryMe = "MARRY_ME";
        public const string ProposePhoto = "PROPOSE_PHOTO";
        public const string ProposeStone = "PROPOSE_STONE";
        public const string ProposeMusic = "PROPOSE_MUSIC";
        public const string Accept = "ACCEPT";
        public const string PresentPhoto = "PRESENT_PHOTO";
        public const string PresentMusic = "PRESENT_MUSIC";
        public const string PresentStone = "PRESENT_STONE";
    }

    public static class Tokens
    {
        public const string PhotoPreference = "$PHOTO_PREFERENCE$";
        public const string StonePreference = "$STONE_PREFERENCE$";
        public const string MusicPreference = "$MUSIC_PREFERENCE$";
    }

    public static class Conditions
    {
        public const string PlayerPresentedPhoto = "PRESENTED_PHOTO";
        public const string WifeAcceptedPhoto = "ACCEPTED_PHOTO";
        public static string PlayerPresentedMusic = "PRESENTED_MUSIC";
        public static string WifeAcceptedMusic = "ACCEPTED_MUSIC";
        public const string PlayerPresentedStone = "PRESENTED_STONE";
        public const string WifeAcceptedStone = "ACCEPTED_STONE";
    }
}

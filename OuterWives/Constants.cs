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
    }

    public static class Conditions
    {
        public const string GavePhoto = "GAVE_PHOTO";
    }

    public static class Options
    {
        public const string MarryMe = "MARRY_ME";
        public const string ProposePhoto = "PROPOSE_PHOTO";
        public const string ProposeStone = "PROPOSE_STONE";
        public const string ProposeMusic = "PROPOSE_MUSIC";
        public const string Accept = "ACCEPT";
        public const string GivePhoto = "GIVE_PHOTO";
    }

    public static class Tokens
    {
        public const string PhotoPreference = "$PHOTO_PREFERENCE$";
        public const string StonePreference = "$STONE_PREFERENCE$";
        public const string MusicPreference = "$MUSIC_PREFERENCE$";
    }
}

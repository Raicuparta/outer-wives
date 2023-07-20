using UnityEngine;

namespace OuterWives
{
    public class WifeMaterial
    {
        public readonly string name;

        private WifeMaterial _photoPreference;
        public string PhotoPreference => _photoPreference.name;

        private SharedStone _stonePreference;
        public string StonePreference => NomaiRemoteCameraPlatform.IDToPlanetString(_stonePreference._connectedPlatform);

        private TravelerController _musicPreference;
        public string MusicPreference => _musicPreference._audioSource.name.Replace("Signal_", "");

        public WifeMaterial(string name)
        {
            this.name = name;
        }

        public void Initialize()
        {
            _photoPreference = OuterWives.Wives[Random.Range(0, OuterWives.Wives.Length)];
            _stonePreference = ThingFinder.GetRandomStone();
            _musicPreference = ThingFinder.GetRandomTraveler();
        }
    }
}

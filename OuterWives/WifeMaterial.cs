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

        public string MusicPreference { get; private set; }

        public WifeMaterial(string name)
        {
            this.name = name;
        }

        public void Initialize()
        {
            _photoPreference = OuterWives.Wives[Random.Range(0, OuterWives.Wives.Length)];
            _stonePreference = ThingFinder.GetRandomStone();
            MusicPreference = "Poopy metal";
        }
    }
}

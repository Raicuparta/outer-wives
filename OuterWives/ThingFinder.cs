using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OuterWives
{
    public static class ThingFinder
    {
        private static readonly string[] _stoneBlocklist = new string[] {
            "None",
            "Module",
        };
        private static SharedStone[] _stones; // TODO don't persist across reloads.
        public static SharedStone GetRandomStone()
        {
            if (_stones == null)
            {
                _stones = Resources.FindObjectsOfTypeAll<SharedStone>()
                    .Where(stone => !_stoneBlocklist.Any(blockedWord => stone.GetDisplayName().Contains(blockedWord)))
                    .GroupBy(stone => stone.GetDisplayName())
                    .Select(group => group.First())
                    .ToArray();

                foreach (var stone in _stones)
                {
                    OuterWives.Helper.Console.WriteLine($"Found stone: {stone.GetDisplayName()}");
                }
            }

            var index = Random.Range(0, _stones.Length);

            OuterWives.Helper.Console.WriteLine($"Index is {index} ({_stones.Length})");

            return _stones[index];
        }

        private static TravelerController[] _travelers;
        public static TravelerController GetRandomTraveler()
        {
            if (_travelers == null)
            {
                _travelers = Resources.FindObjectsOfTypeAll<TravelerController>()
                    .Where(traveler => traveler._audioSource != null)
                    .ToArray();

                foreach (var traveler in _travelers)
                {
                    OuterWives.Helper.Console.WriteLine($"Found traveler: {traveler.name}");
                }
            }

            var index = Random.Range(0, _travelers.Length);

            OuterWives.Helper.Console.WriteLine($"Index is {index} ({_travelers.Length})");

            return _travelers[index];
        }
    }
}

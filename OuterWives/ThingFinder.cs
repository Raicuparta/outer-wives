using UnityEngine;

namespace OuterWives
{
    public static class ThingFinder
    {
        private static SharedStone[] _stones;
        public static SharedStone GetRandomStone()
        {
            _stones ??= Resources.FindObjectsOfTypeAll<SharedStone>();

            var index = Random.Range(0, _stones.Length);

            OuterWives.Helper.Console.WriteLine($"Index is {index} ({_stones.Length})");

            return _stones[index];
        }
    }
}

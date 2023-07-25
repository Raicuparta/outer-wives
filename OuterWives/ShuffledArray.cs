
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

namespace OuterWives;

public class ShuffledArray<TObject> where TObject: Object
{
    private const int Seed = 643216;

    private static Random.State _randomState;

    public readonly ReadOnlyArray<TObject> Array;
    private readonly Dictionary<int, int> _wifeToIndexMap = new();

    private int _index;

    public ShuffledArray(IEnumerable<TObject> enumerable)
    {
        SetUpRandomness();
        Array = enumerable.OrderBy(character => Random.value).ToArray();
        ResetRandomness();
    }

    private static void SetUpRandomness()
    {
        _randomState = Random.state;
        Random.InitState(Seed);
    }

    private static void ResetRandomness()
    {
        Random.state = _randomState;
    }

    private int GetNextIndex()
    {
        var index = _index;
        _index = (_index + 1) % Array.Count;
        return index;
    }

    public TObject Get(int wifeIndex)
    {
        var hasValue = _wifeToIndexMap.TryGetValue(wifeIndex, out var index);
        if (!hasValue)
        {
            index = _wifeToIndexMap[wifeIndex] = GetNextIndex();
        }
        return Array[index];
    }
}

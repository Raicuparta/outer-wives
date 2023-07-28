
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
    private readonly Dictionary<int, int> _indexToRandom = new();

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

    private int GetNextRandomIndex()
    {
        var index = _index;
        _index = (_index + 1) % Array.Count;
        return index;
    }

    public TObject Get(int index)
    {
        var hasValue = _indexToRandom.TryGetValue(index, out var randomIndex);
        if (!hasValue)
        {
            randomIndex = _indexToRandom[index] = GetNextRandomIndex();
        }
        return Array[randomIndex];
    }
}

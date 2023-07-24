using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OuterWives;

public static class ArrayExtensions
{
    private const int Seed = 643216;

    private static Random.State _randomState;
    private static void PreRandom()
    {
        _randomState = Random.state;
        Random.InitState(Seed);
    }

    private static void PostRandom()
    {
        Random.state = _randomState;
    }

    public static TObject[] ToShuffledArray<TObject>(this IEnumerable<TObject> iterable) where TObject : Object
    {
        PreRandom();
        var array = iterable.OrderBy(character => Random.value).ToArray();
        PostRandom();
        return array;
    }

    public static TObject GetWrapped<TObject>(this TObject[] array, int index, TObject avoid = null) where TObject : Object
    {
        var wrappedIndex = index % array.Length;
        var item = array[wrappedIndex];

        if (item == avoid) return array.GetWrapped(index + 1, avoid);

        return item;
    }
}

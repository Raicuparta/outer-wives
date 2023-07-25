using System.Collections.Generic;
using UnityEngine;

namespace OuterWives.Extensions;

public static class ArrayExtensions
{
    public static ShuffledArray<TObject> ToShuffledArray<TObject>(this IEnumerable<TObject> iterable) where TObject : Object
    {
        return new ShuffledArray<TObject>(iterable);
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class TBArrayExtensions
{
    /// <summary>
    /// Adds an item to an array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="item"></param>
    public static T[] AddToArray<T>(T[] array, T item)
    {
        T[] newArray = new T[array.Length + 1];
        for (int i = 0; i < array.Length; i++)
        {
            newArray[i] = array[i];
        }
        newArray[newArray.Length - 1] = item;
        return newArray;
    }

    /// <summary>
    /// Removes an item from an array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="item"></param>
    public static T[] RemoveFromArray<T>(T[] array, int index)
    {
        var newArray = new List<T>(array);
        newArray.RemoveAt(index);
        return newArray.ToArray();
    }
}
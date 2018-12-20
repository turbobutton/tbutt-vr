using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TButt
{
    /// <summary>
    /// Extension methods for converting enum flags to and from arrays
    /// via PedroCheckos from https://stackoverflow.com/questions/22132995/extension-method-to-convert-flags-to-ienumerableenum-and-conversely-c
    /// </summary>
    static class TBEnumFlags
    {
        public static T ToMask<T>(this IEnumerable<T> values) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type.");

            int builtValue = 0;
            foreach (T value in Enum.GetValues(typeof(T)))
            {
                if (values.Contains(value))
                {
                    builtValue |= Convert.ToInt32(value);
                }
            }
            return (T)Enum.Parse(typeof(T), builtValue.ToString());
        }


        public static IEnumerable<T> ToValues<T>(this T flags) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type.");

            int inputInt = (int)(object)(T)flags;
            foreach (T value in Enum.GetValues(typeof(T)))
            {
                int valueInt = (int)(object)(T)value;
                if (0 != (valueInt & inputInt))
                {
                    yield return value;
                }
            }
        }
    }
}
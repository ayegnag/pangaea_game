using System;
using UnityEngine;
using System.Collections.Generic;

namespace KOI
{
    public static class Utils
    {
        static readonly System.Random _RandomInstance = new System.Random(1);

        public static int RandomRange(int minInclusive, int maxInclusive)
        {
            return _RandomInstance.Next(minInclusive, maxInclusive + 1);
        }

        public static T RandomValueFromList<T>(List<T> list){
            return (T)list[_RandomInstance.Next(list.Count)];
        }

        public static T RandomEnumValue<T>()
        {
            Array valuesArray = Enum.GetValues(typeof(T));
            int randomEnumIndex = _RandomInstance.Next(valuesArray.Length);
            return (T)valuesArray.GetValue(randomEnumIndex);
        }

        public static float LinearConverstion(float inputValue, float originalMin, float originalMax, float targetMin, float targetMax)
        {
            // Input value in the range of 0 to 1
            float result = (inputValue - originalMin) * (targetMax - targetMin) / (originalMax - originalMin) + targetMin; // Convert inputValue to the range of 10 to 20
            return result;
        }

        public static void DumpToConsole(object obj)
        {
            var output = JsonUtility.ToJson(obj, true);
            Debug.Log(output);
        }
    }
}
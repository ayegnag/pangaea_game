using System;
using UnityEngine;

namespace KOI
{
    public static class Utils
    {
        static readonly System.Random _RandomInstance = new System.Random(1);

        public static int RandomRange(int minInclusive, int maxInclusive)
        {
            return _RandomInstance.Next(minInclusive, maxInclusive + 1);
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

        public static EntityId GetEntityTypeIdFromName(string name)
        {
            string[] identity = name.Split("_");
            EntityId result = new EntityId(identity[0], int.Parse(identity[1]));
            return result;
        }


        public static string GenerateRandomName()
        {
            // Set up arrays of consonants and vowels
            char[] consonants = {'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'y', 'z'};
            char[] vowels = {'a', 'e', 'i', 'o', 'u'};

            // Generate a random name
            System.Random rand = new System.Random();
            int nameLength = rand.Next(4, 8); // Name length between 4 and 8 characters
            string name = "";

            // Start with a consonant
            name += consonants[rand.Next(consonants.Length)].ToString().ToUpper();

            // Alternate consonants and vowels until the name is long enough
            for (int i = 0; i < nameLength - 1; i++)
            {
                if (i % 2 == 1) // Consonant
                {
                    name += consonants[rand.Next(consonants.Length)];
                }
                else // Vowel
                {
                    name += vowels[rand.Next(vowels.Length)];
                }
            }
            // [TODO:] For some names a vovel needs to be added Like Fbuh would be better as Fibuh
            // Display the generated name
            Debug.Log("Your random name is: " + name);
            return name;
        }
    }
}
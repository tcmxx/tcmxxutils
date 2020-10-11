using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCUtils
{

    public static class MathUtils
    {
        public enum InterpolateMethod
        {
            Linear,
            Log
        }

        /// <summary>
        /// interpolate between x1 and x2 to ty suing the interpolate method
        /// </summary>
        /// <param name="method"></param>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float Interpolate(float x1, float x2, float t, InterpolateMethod method = InterpolateMethod.Linear)
        {
            if (method == InterpolateMethod.Linear)
            {
                return Mathf.Lerp(x1, x2, t);
            }
            else
            {
                return Mathf.Pow(x1, 1 - t) * Mathf.Pow(x2, t);
            }
        }

        /// <summary>
        /// Return a index randomly. The probability if a index depends on the value in that list
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static int IndexByChance(float[] list)
        {
            float total = 0;

            foreach (var v in list)
            {
                total += v;
            }
            Debug.Assert(total > 0);

            float current = 0;
            float point = UnityEngine.Random.Range(0, total);

            for (int i = 0; i < list.Length; ++i)
            {
                current += list[i];
                if (current >= point)
                {
                    return i;
                }
            }
            return 0;
        }
        /// <summary>
        /// return the index of the max value in the list
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static int IndexMax(float[] list)
        {
            int result = 0;
            for (int i = 1; i < list.Length; ++i)
            {
                if (list[i - 1] < list[i])
                {
                    result = i;
                }
            }
            return result;
        }

        /// <summary>
        /// Shuffle a list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="rnd"></param>
        public static void Shuffle<T>(IList<T> list, System.Random rnd)
        {
            int n = list.Count;
            while (n > 1)
            {

                n--;
                int k = rnd.Next(0, n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        /// <summary>
        /// Shuffle a list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="rnd"></param>
        /// <param name="numberOfElementToShuffle"></param>
        public static void ShuffleFromStart<T>(IList<T> list, System.Random rnd, int numberOfElementToShuffle = -1)
        {
            if (numberOfElementToShuffle <= 0)
                numberOfElementToShuffle = list.Count;

            int n = 0;
            int count = list.Count;

            while (n < count - 1 && n < numberOfElementToShuffle)
            {
                int k = rnd.Next(n, count);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;

                n++;
            }
        }
    }
}
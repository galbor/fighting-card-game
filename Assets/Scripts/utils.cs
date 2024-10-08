﻿using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public static class utils
    {
        public static void ShuffleArrayInPlace(object[] arr)
        {
            //Fisher-Yates shuffle
            for (int i = 0; i < arr.Length-1; i++)
            {
                int j = Random.Range(i, arr.Length);
                (arr[i], arr[j]) = (arr[j], arr[i]);
            }
        }

        /**
         * chooses k different random numbers between 0, n (not including n)
         * @return list of chosen numbers 
         */
        public static List<int> ChooseKRandomNumbersOrdered(int n, int k)
        {
            int[] nums = Enumerable.Repeat(-1, n).ToArray(); //fills array with -1
            k = Math.Min(n, k);
            for (int i = 0; i < k; i++)
            {
                int chosen = Random.Range(i, n);
                if (nums[i] == -1) nums[i] = i;
                if (nums[chosen] == -1) nums[chosen] = chosen;
                (nums[i], nums[chosen]) = (nums[chosen], nums[i]);
            }

            return nums.Take(k).ToList();
        }
    }
}
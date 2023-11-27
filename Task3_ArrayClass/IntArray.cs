using System;
using System.Collections.Generic;

namespace IntArray
{
    public class IntArray
    {
        int[] arr;
        int count;
        public IntArray(int n)
        {
            count = n;
            arr = new int[n];
        }
        public IntArray()
        {
            count = 0;
            arr = null;
        }
        public int[] Arr => arr;
        public int Count => count;
        public void InputData(params int[] mas)
        {
            for (int i = 0; i < Math.Min(mas.Length, count); i++)
                arr[i] = mas[i];
        }
        public void InputDataRandom()
        {
            Random rnd = new Random();
            for (int i = 0; i < count; i++)
                arr[i] = rnd.Next(-100,100);
        }
        public void print(int a, int b)
        {
            if ((a < 0) || (a>=count) || (b<0) || (b>=count)) 
                throw new InvalidOperationException("Индекс за пределами диапазона!");
            int sign = Math.Sign(b-a);
            for (int i = a; i != b+sign; i += sign)
                Console.Write(arr[i] + " ");
        }

        public void print()
        {
            for (int i = 0; i < count; i++)
                Console.Write(arr[i] + " ");
            Console.WriteLine();
        }

        public List<int> FindValue(int num)
        {
            List<int> lst = new List<int>();
            for (int i = 0; i < count; i++)
                if (arr[i] == num) lst.Add(i);
            return lst;
        }
        public void DelValue(int num, out bool changed)
        {
            List<int> newArr = new List<int>(count);
            for (int i = 0; i < count; i++)
                if (arr[i] != num) newArr.Add(arr[i]);
            arr = newArr.ToArray();
            if (count > arr.Length)
            {
                changed = true;
                count = arr.Length;
            }
            else changed = false;
        }
        public int FindMax()
        {
            int max = -int.MaxValue;
            for (int i = 0; i < count; i++)
                if (arr[i] > max) max = arr[i];
            return max;
        }

        public int this[int index]
        {
            get => arr[index];
            set => arr[index] = value;
        }

        public static void Add(in IntArray arr1, in IntArray arr2, ref IntArray arr3)
        {
            if (arr1.Count == arr2.Count)
            { 
                arr3 = new IntArray(arr1.Count);
                for (int i = 0; i < arr1.Count; i++)
                    arr3[i] = arr1[i] + arr2[i];
            }
        }


        void Swap(ref int x, ref int y)
        {
            x = x + y;
            y = x - y;
            x = x - y;
        }

        public void Sort() //шейкерная сортировка
        {
            int left = 0;
            int right = count - 1;
            while (left < right)
            {
                for (int i = left; i < right; i++)
                {
                    if (arr[i] > arr[i + 1])
                        Swap(ref arr[i], ref arr[i + 1]);
                }
                right--;
                for (int i = right; i > left; i--)
                {
                    if (arr[i - 1] > arr[i])
                        Swap(ref arr[i-1], ref arr[i]);
                }
                left++;
            }
        }
    }
}

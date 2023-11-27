using System;
using IntArray;
namespace Task3
{
    class Program
    {
        static void Main(string[] args)
        {
            IntArray.IntArray arr1 = new IntArray.IntArray(10);
            IntArray.IntArray arr2 = new IntArray.IntArray(10);
            arr1.InputDataRandom();
            arr2.InputData(6, 4, 3, 1, 7, 5, 2, -5, 0, -1, 3, 6);
            Console.Write("arr1 = ");
            arr1.print();
            Console.Write("arr2 = ");
            arr2.print();
            IntArray.IntArray arr3 = null;
            IntArray.IntArray.Add(in arr1, in arr2, ref arr3);
            Console.Write("arr3 = arr1 + arr2 = ");
            arr3.print();
            arr1.Sort();
            Console.Write("Sorted arr1 = ");
            arr1.print(2,4);
        }
    }
}

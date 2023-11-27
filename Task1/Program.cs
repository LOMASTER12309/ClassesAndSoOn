using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Task1
{
    class Program
    {
        int n = 5;
        static float[][] ToMatrixFloat(in string[] lines)
        {
            int n = lines.Length;
            List<float[]> norm = new List<float[]>();
            for (int i = 0; i < n; i++)
            {
                string[] divided = lines[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                float[] values = new float[divided.Length];
                for (int j = 0; j < divided.Length; j++) float.TryParse(divided[j], out values[j]);
                if (values.Length > 0) norm.Add(values);
            }
            return norm.ToArray();
        }

        static float[] MinInLines(in string[] lines)
        {
            float[][] matrix = ToMatrixFloat(in lines);
            float[] result = new float[matrix.Length];
            for (int i = 0; i < matrix.Length; i++)
            {
                float min = float.MaxValue;
                for (int j = 0; j < matrix[i].Length; j++)
                    if (matrix[i][j] < min) min = matrix[i][j];
                result[i] = min;
            }
            return result;
        }
        static float[] MaxInLines(in string[] lines)
        {
            float[][] matrix = ToMatrixFloat(in lines);
            float[] result = new float[matrix.Length];
            for (int i = 0; i < matrix.Length; i++)
            {
                float max = -float.MaxValue;
                for (int j = 0; j < matrix[i].Length; j++)
                    if (matrix[i][j] > max) max = matrix[i][j];
                result[i] = max;
            }
            return result;
        }

        static float[] SumInLines(in string[] lines)
        {
            float[][] matrix = ToMatrixFloat(in lines);
            float[] result = new float[matrix.Length];
            for (int i = 0; i < matrix.Length; i++)
            {
                float sum = 0;
                for (int j = 0; j < matrix[i].Length; j++)
                    sum += matrix[i][j];
                result[i] = sum;
            }
            return result;
        }
        /*static void ToFloat(string str, ref float value)
        {
            float.TryParse(str, out value);
        }*/
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines("lines.txt");
            float[][] result = ToMatrixFloat(in lines);
            Console.WriteLine("Преобразовано в матрицу чисел:");
            for (int i = 0; i < result.Length; i++)
            {
                for (int j = 0; j < result[i].Length; j++)
                    Console.Write(result[i][j] + " ");
                Console.WriteLine();
            }
            float[] min = MinInLines(lines);
            float[] max = MaxInLines(lines);
            float[] sum = SumInLines(lines);
            Console.WriteLine("Минимаьное, максимальное и сумма по строкам:");
            for (int i = 0; i < min.Length; i++)
                Console.WriteLine($"{i + 1}) min = {min[i]}; max = {max[i]}; sum = {sum[i]}");
        }
    }
}

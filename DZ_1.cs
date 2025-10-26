namespace Algorithms;
public static class DZ1
{
    public static void Run()
    {
        Console.WriteLine("\n========================= Домашняя работа 1 =========================");
        Console.WriteLine("Задание 1.");
        Console.WriteLine(
            "Разработать блок-схему алгоритма и программу для вычисления площади треугольника S " +
            "по заданным с клавиатуры значениям стороны a и высоты h, " +
            "проведенной к этой стороне. Значение площади вывести на экран.");

        Console.WriteLine("\nБлок-схема алгоритма:\n");

        Console.WriteLine(" ┌────────────┐");
        Console.WriteLine(" │   НАЧАЛО   │");
        Console.WriteLine(" └──────┬─────┘");
        Console.WriteLine("        │");
        Console.WriteLine("        ▼");
        Console.WriteLine(" ┌─────────────────────┐");
        Console.WriteLine(" │ Ввод значений a и h │");
        Console.WriteLine(" └──────┬──────────────┘");
        Console.WriteLine("        │");
        Console.WriteLine("        ▼");
        Console.WriteLine(" ┌────────────────────────────┐");
        Console.WriteLine(" │ Вычисление S = 0,5 * a * h │");
        Console.WriteLine(" └──────┬─────────────────────┘");
        Console.WriteLine("        │");
        Console.WriteLine("        ▼");
        Console.WriteLine(" ┌──────────────────┐");
        Console.WriteLine(" │ Вывод значения S │");
        Console.WriteLine(" └──────┬───────────┘");
        Console.WriteLine("        │");
        Console.WriteLine("        ▼");
        Console.WriteLine(" ┌─────────────┐");
        Console.WriteLine(" │    КОНЕЦ    │");
        Console.WriteLine(" └─────────────┘");

        Console.Write("\nВведите длину стороны a: ");
        double a = Convert.ToDouble(Console.ReadLine());

        Console.Write("\nВведите высоту h: ");
        double h = Convert.ToDouble(Console.ReadLine());

        double S = 0.5 * a * h;

        Console.WriteLine($"\nПлощадь треугольника: {S:F2}.\n");

        Console.WriteLine("\nЗадание 2.");
        Console.WriteLine(
            "Дано два массива A и B, элементами которых являются целые числа. " +
            "Массив A отсортирован в порядке убывания. " +
            "Для каждого элемента массива B найти наиболее близкое число к данному в массиве A. " +
            "Если таких несколько, то вывести оба.");

        int[] A = [65, 43, 23, 11, 7];
        int[] B = [3, 54, 23, 9, 65];

        Console.WriteLine($"\nМассив A: [{string.Join(", ", A)}];");
        Console.WriteLine($"Массив B: [{string.Join(", ", B)}].\n");

        int[] NegA = [.. A.Select(v => -v)];

        Console.WriteLine("Результат:\n");
        foreach (int x in B)
        {
            var closest = FindClosest(A, NegA, x);
            Console.WriteLine($"{x} - {string.Join(' ', closest)}.");
        }
        Console.WriteLine();
    }

    static int[] FindClosest(int[] A, int[] NegA, int x)
    {
        int pos = Array.BinarySearch(NegA, -x);

        if (pos >= 0) return [A[pos]];

        int ins = ~pos;
        int n = A.Length;

        if (ins == 0) return [A[0]];
        if (ins == n) return [A[n - 1]];

        int L = ins - 1;
        int R = ins;

        int dl = Math.Abs(A[L] - x);
        int dr = Math.Abs(A[R] - x);

        if (dl < dr) return [A[L]];
        if (dr < dl) return [A[R]];
        return A[L] == A[R] ? [A[L]] : [A[L], A[R]];
    }
}
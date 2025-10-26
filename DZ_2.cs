namespace Algorithms;
public static class DZ2
{
    public static void Run()
    {
        Console.WriteLine("\n========================= Домашняя работа 2 =========================");

        int[] M = [53, 32, 64, 78, 1, 98, 34, 32, 99];

        Console.WriteLine("Задание 1.");
        Console.WriteLine($"Задан целочисленный массив [{string.Join(", ", M)}]. " +
            "Требуется посчитать число инверсий (пар элементов, расположенных «вне своего естественного порядка»), " +
            "возникших в процессе сортировки элементов массива от большего к меньшему «пузырьком».");

        long swaps = BubbleSortDescAndCount(M, out int passes);

        Console.WriteLine($"\nОтсортированный по убыванию массив: [{string.Join(", ", M)}];");
        Console.WriteLine($"\nКоличество проходов пузырька: {passes};");
        Console.WriteLine($"\nЧисло инверсий (обменов): {swaps}.");
        
        Console.WriteLine();
        Console.WriteLine("\nЗадание 2.");
        Console.WriteLine("Реализовать алгоритм бинарного поиска через рекурсию.");
        Console.WriteLine($"\nВыполним поиск в отсортированном по убыванию массиве [{string.Join(", ", M)}], полученном в предыдущем задании.");

        Console.Write("\nВведите число для поиска: ");
        if (int.TryParse(Console.ReadLine(), out int target))
        {
            int steps = 0;
            int index = RecursiveBinarySearch(M, target, 0, M.Length - 1, ref steps);

            if (index != -1)
                Console.WriteLine($"\nЭлемент {target} найден под индексом {index} на позиции {index + 1}.");
            else
                Console.WriteLine($"\nЭлемент {target} не найден в массиве.");

            Console.WriteLine($"\nВсего шагов поиска: {steps}.");
        }
        else
        {
            Console.WriteLine("Ошибка: введено не число.\n");
        }
        Console.WriteLine();
    }

    static long BubbleSortDescAndCount(int[] arr, out int passes)
    {
        long swaps = 0;
        passes = 0;
        int n = arr.Length;
        bool changed;

        do
        {
            changed = false;
            passes++;

            for (int i = 0; i + 1 < n; i++)
            {
                if (arr[i] < arr[i + 1])
                {
                    (arr[i], arr[i + 1]) = (arr[i + 1], arr[i]);
                    swaps++;
                    changed = true;
                }
            }

            n--;
        } while (changed);

        return swaps;
    }

    static int RecursiveBinarySearch(int[] arr, int target, int left, int right, ref int steps, int depth = 0)
    {
        static void Log(int d, string msg) => Console.WriteLine(new string(' ', d * 2) + msg);

        if (left > right)
        {
            steps++;
            Log(depth, $"[{depth}] Диапазон пуст — элемент не найден.");
            return -1;
        }

        steps++;
        int mid = (left + right) / 2;

        Log(depth, $"[{depth}] Проверяем индекс {mid} (значение {arr[mid]});");

        if (arr[mid] == target)
        {
            Log(depth, $"[{depth}] Найден!");
            return mid;
        }

        if (arr[mid] < target)
        {
            Log(depth, $"[{depth}] Идём влево (большие числа);");
            return RecursiveBinarySearch(arr, target, left, mid - 1, ref steps, depth + 1);
        }
        else
        {
            Log(depth, $"[{depth}] Идём вправо (меньшие числа);");
            return RecursiveBinarySearch(arr, target, mid + 1, right, ref steps, depth + 1);
        }
    }
}
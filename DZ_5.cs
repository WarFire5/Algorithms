namespace Algorithms;
public static class DZ5
{
    public static void Run()
    {
        Console.WriteLine("\n========================= Домашняя работа 5 =========================");
        Console.WriteLine("Вычислить расстояние Дамерау–Левенштейна для:\n");

        CalculateDamerauLevenshteinDistance("1) Аббревиатура и Бабреваитуар", "Аббревиатура", "Бабреваитуар");

        CalculateDamerauLevenshteinDistance("2) Фуникулер и Уфнниуклер", "Фуникулер", "Уфнниуклер");
    }

    private static void CalculateDamerauLevenshteinDistance(string title, string s, string t)
    {
        Console.WriteLine(title);
        Console.WriteLine("\nШаги вычисления:");

        int n = s.Length, m = t.Length;
        var d = new int[n + 1, m + 1];

        for (int i = 0; i <= n; i++) d[i, 0] = i;
        for (int j = 0; j <= m; j++) d[0, j] = j;

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
            {
                int cost = s[i - 1] == t[j - 1] ? 0 : 1;

                int del = d[i - 1, j] + 1;
                int ins = d[i, j - 1] + 1;
                int sub = d[i - 1, j - 1] + cost;

                int best = Math.Min(Math.Min(del, ins), sub);

                if (i > 1 && j > 1 &&
                    s[i - 1] == t[j - 2] &&
                    s[i - 2] == t[j - 1])
                {
                    best = Math.Min(best, d[i - 2, j - 2] + 1);
                    Console.WriteLine($"[{i,2},{j,2}] транспозиция '{s[i - 2]}{s[i - 1]}' ↔ '{t[j - 2]}{t[j - 1]}' → +1");
                }

                d[i, j] = best;

                string act = cost == 0 ? "совпадают" : "разные";
                Console.WriteLine($"[{i,2},{j,2}] '{s[i - 1]}' vs '{t[j - 1]}' → {act}, min({del},{ins},{sub}) = {best}");
            }
        }

        Console.WriteLine("\nМатрица расстояний:");
        PrintMatrix(d, s, t);

        Console.WriteLine($"\nРасстояние Дамерау–Левенштейна равно {d[n, m]}.\n");
        Console.WriteLine(new string('─', 70));
        Console.WriteLine();
    }

    private static void PrintMatrix(int[,] d, string s, string t)
    {
        int n = s.Length, m = t.Length;

        Console.Write("      ");
        for (int j = 0; j < m; j++)
            Console.Write($"{t[j],4}");
        Console.WriteLine();

        for (int i = 0; i <= n; i++)
        {
            if (i == 0) Console.Write("  ");
            else Console.Write($"{s[i - 1],2}");

            for (int j = 0; j <= m; j++)
                Console.Write($"{d[i, j],4}");
            Console.WriteLine();
        }
    }
}
namespace Algorithms;

public static class DZ7
{
    public static void Run()
    {
        Console.WriteLine("\n========================= Домашняя работа 7 =========================");

        var given = new Dictionary<string, Dictionary<string, int>>
        {
            ["A"] = new() { ["B"] = 7 },
            ["B"] = new() { ["A"] = 7 },
            ["C"] = new() { ["B"] = 4, ["D"] = 1 },
            ["D"] = new() { ["B"] = 2, ["C"] = 1 },
            ["E"] = new() { ["B"] = 4, ["F"] = 2, ["G"] = 2 },
            ["F"] = new() { ["E"] = 2, ["G"] = 2, ["A"] = 12 },
            ["G"] = new() { ["E"] = 2, ["F"] = 2, ["H"] = 3 },
            ["H"] = new() { ["G"] = 3, ["I"] = 2, ["J"] = 1 },
            ["I"] = new() { ["H"] = 2, ["J"] = 1 },
            ["J"] = new() { ["H"] = 1, ["J"] = 1 }
        };

        Console.WriteLine("\nПостроить граф по следующей таблице смежности:");
        PrintAdjacencyList(given);

        Console.WriteLine("\nПостроение неориентированного графа:");
        var g = ToUndirectedWithTrace(given);

        Console.WriteLine("\nМатрица весов (итоговый граф):");
        PrintAdjacencyMatrix(g);

        Console.WriteLine("\nASCII-схема построенного графа:");
        PrintGraphScheme();

        Console.WriteLine("\nПошаговое вычисление расстояний алгоритмом Дейкстры (старт = B):");
        var (dist, prev) = DijkstraWithTrace(g, "B");

        Console.WriteLine("\nКратчайшие расстояния и пути от узла B до всех вершин:");
        foreach (var v in dist.Keys.OrderBy(x => x))
            Console.WriteLine($"{v}: {FmtInf(dist[v]),2}  |  {string.Join(" → ", Reconstruct(prev, "B", v))}");
        Console.WriteLine();
    }

    private static void PrintAdjacencyList(Dictionary<string, Dictionary<string, int>> g)
    {
        foreach (var u in g.Keys.OrderBy(x => x))
        {
            var edges = g[u].OrderBy(kv => kv.Key).Select(kv => $"{kv.Key}:{kv.Value}");
            Console.WriteLine($"{u}: {{ {string.Join(", ", edges)} }}");
        }
    }

    private static void PrintAdjacencyMatrix(Dictionary<string, Dictionary<string, int>> g)
    {
        var V = g.Keys.OrderBy(x => x).ToArray();
        const int W = 4;

        Console.Write("     ");
        foreach (var v in V) Console.Write($"{v,W}");
        Console.WriteLine();

        foreach (var u in V)
        {
            Console.Write($"{u,3}  ");
            foreach (var v in V)
            {
                if (u == v) { Console.Write($"{"-",W}"); continue; }
                Console.Write(g[u].TryGetValue(v, out int w) ? $"{w,W}" : $"{'·',W}");
            }
            Console.WriteLine();
        }
    }

    private static void PrintGraphScheme()
    {
        const string ascii = @"
                      A
                      |7
                      B
                    / | \
                  4/  |  \2
                  C   |   D
                   \  |  /
                    \ | /
                      E
                     / \
                    2   2
                   /     \
                  F ---2--- G ---3--- H ---2--- I
                                        \       |
                                         \1     |1
                                          \     |
                                            ----J
";
        Console.WriteLine(ascii);
    }

    private static Dictionary<string, Dictionary<string, int>>
    ToUndirectedWithTrace(Dictionary<string, Dictionary<string, int>> given)
    {
        var g = new Dictionary<string, Dictionary<string, int>>();

        var vertices = new SortedSet<string>(given.Keys);
        foreach (var (_, nbrs) in given)
            foreach (var v in nbrs.Keys) vertices.Add(v);

        foreach (var v in vertices) g[v] = new Dictionary<string, int>();

        int step = 0;

        void UpsertEdge(string from, string to, int w, bool mirror)
        {
            var tag = mirror ? " (симметрия)" : "";
            if (!g[from].TryGetValue(to, out var cur))
            {
                g[from][to] = w;
                Console.WriteLine($"     + добавляем {from}—{to} = {w}{tag}");
            }
            else if (w < cur)
            {
                g[from][to] = w;
                Console.WriteLine($"     * обновляем {from}—{to}: {cur} → {w}{tag}");
            }
            else
            {
                Console.WriteLine($"     = оставляем {from}—{to} = {cur}{tag}");
            }
        }

        foreach (var (u, nbrs) in given.OrderBy(k => k.Key))
        {
            foreach (var (v, w) in nbrs.OrderBy(k => k.Key))
            {
                Console.WriteLine($"[{++step}] рассматриваем направленное ребро {u}→{v} (w={w})");
                UpsertEdge(u, v, w, mirror: false);
                UpsertEdge(v, u, w, mirror: true);
            }
        }

        Console.WriteLine("\nИтоговый список смежности:");
        PrintAdjacencyList(g);
        return g;
    }

    private static (Dictionary<string, int> dist, Dictionary<string, string?> prev)
    DijkstraWithTrace(Dictionary<string, Dictionary<string, int>> g, string start)
    {
        var dist = g.Keys.ToDictionary(v => v, _ => int.MaxValue);
        var prev = g.Keys.ToDictionary(v => v, _ => (string?)null);
        var visited = new HashSet<string>();
        var pq = new PriorityQueue<string, int>();

        dist[start] = 0;
        pq.Enqueue(start, 0);

        int step = 0;
        while (pq.Count > 0)
        {
            pq.TryDequeue(out var u, out var du);
            if (visited.Contains(u) || du != dist[u]) continue;

            visited.Add(u);
            Console.WriteLine($"(D{++step}) извлекаем вершину {u} с d={FmtInf(du)}");

            foreach (var (v, w) in g[u].OrderBy(kv => kv.Key))
            {
                if (visited.Contains(v)) continue;
                var nd = du + w;
                if (nd < dist[v])
                {
                    var old = dist[v];
                    dist[v] = nd;
                    prev[v] = u;
                    pq.Enqueue(v, nd);
                    Console.WriteLine($"     релакс {u}—{v} (w={w}): d[{v}] {FmtInf(old)} → {nd}; предок[{v}] = {u}");
                }
                else
                {
                    Console.WriteLine($"     ребро {u}—{v} (w={w}): улучшения нет (d[{v}]={FmtInf(dist[v])})");
                }
            }
        }

        Console.WriteLine("\nРасстояния и предки:");
        foreach (var v in dist.Keys.OrderBy(x => x))
            Console.WriteLine($"  {v}: d={FmtInf(dist[v])}, pred={(prev[v] ?? "—")}");
        return (dist, prev);
    }

    private static IEnumerable<string> Reconstruct(Dictionary<string, string?> prev, string s, string t)
    {
        var stack = new Stack<string>();
        string? cur = t;
        while (cur != null) { stack.Push(cur); cur = prev[cur]; }
        if (stack.Peek() != s) return new[] { "(недостижимо)" };
        return stack;
    }

    private static string FmtInf(int d) => d == int.MaxValue ? "∞" : d.ToString();
}
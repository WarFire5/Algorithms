namespace Algorithms;

public static class DZ4
{
    public static void Run()
    {
        Console.WriteLine("\n========================= Домашняя работа 4 =========================");

        int[] Add = [31, 38, 21, 24, 14, 9, 19, 15, 18, 16, 20, 44, 37, 41];
        int[] Delete = [37, 19, 18];

        Console.WriteLine("Задание 1.");
        Console.WriteLine($"Для набора чисел [{string.Join(", ", Add)}] построить бинарное дерево, " +
                          "поочередно добавляя элементы в том порядке, в котором они представлены.");

        var avl = new AvlTree();

        Console.WriteLine("\nПостроение дерева (балансировка выполняется после КАЖДОЙ вставки):");
        foreach (var x in Add)
        {
            avl.Insert(x);
            Console.WriteLine($"+ {x,-2}  -> root={avl.RootKeyOrEmpty}, height={avl.Height()}, bf(root)={avl.RootBalance}");
        }

        Console.WriteLine("\nДерево после всех вставок:");
        avl.Print();

        PrintTraversals(avl, "Обходы после вставок:");
        Console.WriteLine();

        Console.WriteLine("\nЗадание 2.");
        Console.WriteLine($"Удалить из дерева элементы [{string.Join(", ", Delete)}] в заданном порядке.");
        Console.WriteLine("\nУдаление элементов (балансировка выполняется после КАЖДОГО удаления):");
        foreach (var x in Delete)
        {
            avl.Delete(x);
            Console.WriteLine($"- {x,-2}  -> root={avl.RootKeyOrEmpty}, height={avl.Height()}, bf(root)={avl.RootBalance}");
        }

        Console.WriteLine("\nДерево после всех удалений:");
        avl.Print();

        PrintTraversals(avl, "Обходы после удалений:");
        Console.WriteLine();
    }

    private static void PrintTraversals(AvlTree avl, string title)
    {
        Console.WriteLine($"\n{title}");
        Console.WriteLine($" InOrder (симметричный обход по возрастанию): [{string.Join(", ", avl.InOrder())}];");
        Console.WriteLine($" PreOrder (прямой обход): [{string.Join(", ", avl.PreOrder())}];");
        Console.WriteLine($" LevelOrder (обход по уровням слева направо): [{string.Join(", ", avl.LevelOrder())}].");
    }
}

public sealed class AvlTree
{
    private sealed class Node(int key)
    {
        public int Key = key;
        public int Height = 1;
        public Node? Left, Right;
    }

    private Node? _root;

    public void Insert(int key) => _root = Insert(_root, key);
    public void Delete(int key) => _root = Delete(_root, key);

    public int Height() => GetH(_root);
    public string RootKeyOrEmpty => _root == null ? "∅" : _root.Key.ToString();
    public int RootBalance => _root == null ? 0 : Balance(_root);

    public IEnumerable<int> InOrder() { var r = new List<int>(); InOrder(_root, r); return r; }
    public IEnumerable<int> PreOrder() { var r = new List<int>(); PreOrder(_root, r); return r; }
    public IEnumerable<int> LevelOrder()
    {
        var res = new List<int>();
        if (_root == null) return res;
        var q = new Queue<Node>(); q.Enqueue(_root);
        while (q.Count > 0)
        {
            var n = q.Dequeue();
            res.Add(n.Key);
            if (n.Left != null) q.Enqueue(n.Left);
            if (n.Right != null) q.Enqueue(n.Right);
        }
        return res;
    }

    public void Print() => Print(_root, "", true);
    private static void Print(Node? n, string prefix, bool tail)
    {
        if (n == null) { Console.WriteLine("(пусто)"); return; }
        if (n.Right != null) Print(n.Right, prefix + (tail ? "│   " : "    "), false);
        Console.WriteLine(prefix + (tail ? "└── " : "┌── ") + $"{n.Key}  [h={n.Height}, bf={Balance(n)}]");
        if (n.Left != null) Print(n.Left, prefix + (tail ? "    " : "│   "), true);
    }

    private static int GetH(Node? n) => n?.Height ?? 0;
    private static int Balance(Node n) => GetH(n.Left) - GetH(n.Right);
    private static void FixH(Node n) => n.Height = 1 + Math.Max(GetH(n.Left), GetH(n.Right));

    private static Node RotateRight(Node p)
    {
        var q = p.Left!;
        p.Left = q.Right;
        q.Right = p;
        FixH(p); FixH(q);
        return q;
    }

    private static Node RotateLeft(Node q)
    {
        var p = q.Right!;
        q.Right = p.Left;
        p.Left = q;
        FixH(q); FixH(p);
        return p;
    }

    private static Node BalanceNode(Node n)
    {
        FixH(n);
        int bf = Balance(n);

        if (bf > 1)
        {
            if (Balance(n.Left!) < 0) n.Left = RotateLeft(n.Left!);
            return RotateRight(n); // LL
        }

        if (bf < -1)
        {
            if (Balance(n.Right!) > 0) n.Right = RotateRight(n.Right!);
            return RotateLeft(n);
        }

        return n;
    }

    private static Node Insert(Node? n, int key)
    {
        if (n == null) return new Node(key);
        if (key < n.Key) n.Left = Insert(n.Left, key);
        else if (key > n.Key) n.Right = Insert(n.Right, key);
        else return n;

        return BalanceNode(n);
    }

    private static Node? Delete(Node? n, int key)
    {
        if (n == null) return null;

        if (key < n.Key) n.Left = Delete(n.Left, key);
        else if (key > n.Key) n.Right = Delete(n.Right, key);
        else
        {
            if (n.Left is null || n.Right is null)
            {
                n = n.Left ?? n.Right;
            }
            else
            {
                var min = MinNode(n.Right);
                n.Key = min.Key;
                n.Right = Delete(n.Right, min.Key);
            }
        }

        return n == null ? null : BalanceNode(n);
    }

    private static Node MinNode(Node n)
    {
        while (n.Left != null) n = n.Left;
        return n;
    }

    private static void InOrder(Node? n, List<int> r)
    {
        if (n == null) return;
        InOrder(n.Left, r);
        r.Add(n.Key);
        InOrder(n.Right, r);
    }

    private static void PreOrder(Node? n, List<int> r)
    {
        if (n == null) return;
        r.Add(n.Key);
        PreOrder(n.Left, r);
        PreOrder(n.Right, r);
    }
}
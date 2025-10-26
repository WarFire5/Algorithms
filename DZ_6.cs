namespace Algorithms;

public static class DZ6
{
    public static void Run()
    {
        Console.WriteLine("\n========================= Домашняя работа 6 =========================");

        int[] Add = [31, 38, 21, 24, 14, 9, 19, 15, 18, 16, 20, 44, 37, 41];
        int[] Delete = [37, 19, 18];

        Console.WriteLine("Задание 1.");
        Console.WriteLine($"Для набора чисел [{string.Join(", ", Add)}] построить красно-черное дерево, " +
                          "поочередно добавляя элементы в том порядке, в котором они представлены. " +
                          "При необходимости выполнить балансировку дерева.");

        var rbt = new RedBlackTree();

        Console.WriteLine("\nПостроение дерева (балансировка выполняется после КАЖДОЙ вставки):");
        foreach (var x in Add)
        {
            rbt.Insert(x);
            Console.WriteLine($"+ {x,-2}  -> root={rbt.RootKeyOrEmpty}, blackHeight≈{rbt.BlackHeight()}");
        }

        Console.WriteLine("\nДерево после всех вставок:");
        rbt.Print();
        Console.WriteLine();

        Console.WriteLine("\nЗадание 2.");
        Console.WriteLine($"Удалить из дерева элементы [{string.Join(", ", Delete)}] в заданном порядке. " +
                          "При необходимости выполнить балансировку дерева.");

        Console.WriteLine("\nУдаление элементов (балансировка выполняется после КАЖДОГО удаления):");
        foreach (var x in Delete)
        {
            rbt.Delete(x);
            Console.WriteLine($"- {x,-2}  -> root={rbt.RootKeyOrEmpty}, blackHeight≈{rbt.BlackHeight()}");
        }

        Console.WriteLine("\nДерево после всех удалений:");
        rbt.Print();
        Console.WriteLine();
    }
}

public sealed class RedBlackTree
{
    private enum Col { Red, Black }

    private sealed class Node(int key, Col color, Node? left, Node? right, Node? parent)
    {
        public int Key = key;
        public Col Color = color;
        public Node? Left = left, Right = right, Parent = parent;
    }

    private readonly Node _nil;
    private Node _root;

    public RedBlackTree()
    {
        _nil = new Node(0, Col.Black, null!, null!, null!);
        _nil.Left = _nil.Right = _nil.Parent = _nil;
        _root = _nil;
    }

    public string RootKeyOrEmpty => _root == _nil ? "∅" : _root.Key.ToString();

    public int BlackHeight()
    {
        int h = 0;
        var x = _root;
        while (x != _nil)
        {
            if (x.Color == Col.Black) h++;
            x = x.Left!;
        }
        return h;
    }

    public void Insert(int key)
    {
        Node z = new Node(key, Col.Red, _nil, _nil, _nil);
        Node y = _nil;
        Node x = _root;

        while (x != _nil)
        {
            y = x;
            if (z.Key < x.Key) x = x.Left!;
            else if (z.Key > x.Key) x = x.Right!;
            else return;
        }
        z.Parent = y;
        if (y == _nil) _root = z;
        else if (z.Key < y.Key) y.Left = z;
        else y.Right = z;

        InsertFixup(z);
    }

    public void Delete(int key)
    {
        Node z = Search(key);
        if (z == _nil) return;

        Node y = z;
        Col yOriginalColor = y.Color;
        Node x;

        if (z.Left == _nil)
        {
            x = z.Right!;
            Transplant(z, z.Right!);
        }
        else if (z.Right == _nil)
        {
            x = z.Left!;
            Transplant(z, z.Left!);
        }
        else
        {
            y = Minimum(z.Right!);
            yOriginalColor = y.Color;
            x = y.Right!;
            if (y.Parent == z)
            {
                x.Parent = y;
            }
            else
            {
                Transplant(y, y.Right!);
                y.Right = z.Right;
                y.Right!.Parent = y;
            }
            Transplant(z, y);
            y.Left = z.Left;
            y.Left!.Parent = y;
            y.Color = z.Color;
        }

        if (yOriginalColor == Col.Black)
            DeleteFixup(x);
    }

    private Node Search(int key)
    {
        var x = _root;
        while (x != _nil)
        {
            if (key < x.Key) x = x.Left!;
            else if (key > x.Key) x = x.Right!;
            else return x;
        }
        return _nil;
    }

    private void RotateLeft(Node x)
    {
        Node y = x.Right!;
        x.Right = y.Left;
        if (y.Left != _nil) y.Left!.Parent = x;
        y.Parent = x.Parent;
        if (x.Parent == _nil) _root = y;
        else if (x == x.Parent!.Left) x.Parent.Left = y;
        else x.Parent!.Right = y;
        y.Left = x;
        x.Parent = y;
    }

    private void RotateRight(Node y)
    {
        Node x = y.Left!;
        y.Left = x.Right;
        if (x.Right != _nil) x.Right!.Parent = y;
        x.Parent = y.Parent;
        if (y.Parent == _nil) _root = x;
        else if (y == y.Parent!.Left) y.Parent.Left = x;
        else y.Parent!.Right = x;
        x.Right = y;
        y.Parent = x;
    }

    private void Transplant(Node u, Node v)
    {
        if (u.Parent == _nil) _root = v;
        else if (u == u.Parent!.Left) u.Parent.Left = v;
        else u.Parent!.Right = v;
        v.Parent = u.Parent;
    }

    private Node Minimum(Node x)
    {
        while (x.Left != _nil) x = x.Left!;
        return x;
    }

    private void InsertFixup(Node z)
    {
        while (z.Parent!.Color == Col.Red)
        {
            if (z.Parent == z.Parent.Parent!.Left)
            {
                Node y = z.Parent.Parent.Right!;
                if (y.Color == Col.Red)
                {
                    z.Parent.Color = Col.Black;
                    y.Color = Col.Black;
                    z.Parent.Parent.Color = Col.Red;
                    z = z.Parent.Parent;
                }
                else
                {
                    if (z == z.Parent.Right)
                    {
                        z = z.Parent;
                        RotateLeft(z);
                    }
                    z.Parent!.Color = Col.Black;
                    z.Parent.Parent!.Color = Col.Red;
                    RotateRight(z.Parent.Parent);
                }
            }
            else
            {
                Node y = z.Parent.Parent!.Left!;
                if (y.Color == Col.Red)
                {
                    z.Parent.Color = Col.Black;
                    y.Color = Col.Black;
                    z.Parent.Parent.Color = Col.Red;
                    z = z.Parent.Parent;
                }
                else
                {
                    if (z == z.Parent.Left)
                    {
                        z = z.Parent;
                        RotateRight(z);
                    }
                    z.Parent!.Color = Col.Black;
                    z.Parent.Parent!.Color = Col.Red;
                    RotateLeft(z.Parent.Parent);
                }
            }
        }
        _root.Color = Col.Black;
    }

    private void DeleteFixup(Node x)
    {
        while (x != _root && x.Color == Col.Black)
        {
            if (x == x.Parent!.Left)
            {
                Node w = x.Parent.Right!;
                if (w.Color == Col.Red)
                {
                    w.Color = Col.Black;
                    x.Parent.Color = Col.Red;
                    RotateLeft(x.Parent);
                    w = x.Parent.Right!;
                }
                if (w.Left!.Color == Col.Black && w.Right!.Color == Col.Black)
                {
                    w.Color = Col.Red;
                    x = x.Parent;
                }
                else
                {
                    if (w.Right!.Color == Col.Black)
                    {
                        w.Left!.Color = Col.Black;
                        w.Color = Col.Red;
                        RotateRight(w);
                        w = x.Parent.Right!;
                    }
                    w.Color = x.Parent.Color;
                    x.Parent.Color = Col.Black;
                    w.Right!.Color = Col.Black;
                    RotateLeft(x.Parent);
                    x = _root;
                }
            }
            else
            {
                Node w = x.Parent!.Left!;
                if (w.Color == Col.Red)
                {
                    w.Color = Col.Black;
                    x.Parent.Color = Col.Red;
                    RotateRight(x.Parent);
                    w = x.Parent.Left!;
                }
                if (w.Right!.Color == Col.Black && w.Left!.Color == Col.Black)
                {
                    w.Color = Col.Red;
                    x = x.Parent;
                }
                else
                {
                    if (w.Left!.Color == Col.Black)
                    {
                        w.Right!.Color = Col.Black;
                        w.Color = Col.Red;
                        RotateLeft(w);
                        w = x.Parent.Left!;
                    }
                    w.Color = x.Parent.Color;
                    x.Parent.Color = Col.Black;
                    w.Left!.Color = Col.Black;
                    RotateRight(x.Parent);
                    x = _root;
                }
            }
        }
        x.Color = Col.Black;
    }

    public void Print()
    {
        if (_root == _nil) { Console.WriteLine("(пусто)"); return; }
        Print(_root, "", true);
    }

    private void Print(Node n, string prefix, bool tail)
    {
        if (n.Right != _nil) Print(n.Right!, prefix + (tail ? "│   " : "    "), false);

        Console.Write(prefix + (tail ? "└── " : "┌── "));

        var old = Console.ForegroundColor;
        Console.ForegroundColor = (n.Color == Col.Red) ? ConsoleColor.Red : ConsoleColor.Gray;
        Console.Write($"{n.Key} [{(n.Color == Col.Red ? "R" : "B")}]");
        Console.ForegroundColor = old;
        Console.WriteLine();

        if (n.Left != _nil) Print(n.Left!, prefix + (tail ? "    " : "│   "), true);
    }
}
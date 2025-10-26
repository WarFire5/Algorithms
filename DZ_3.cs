namespace Algorithms;

public static class DZ3
{
    public static void Run()
    {
        Console.WriteLine("\n========================= Домашняя работа 3 =========================");
        Console.WriteLine("Задание 1.");
        Console.WriteLine("Заполнить хэш-таблицу из 20 строк следующим набором данных:\n");

        const int Mod = 20;

        (string k, int v)[] data =
        {
            ("Водоросли", 280),
            ("Картофель", 260),
            ("Лук-порей", 59),
            ("Манго", 291),
            ("Орехи грецкие", 266),
            ("Салями", 225),
            ("Специи", 283),
            ("Сыр сливочный", 152),
            ("Творог", 215),
            ("Тофу", 142),
            ("Хек", 248),
            ("Чай черный", 118),
            ("Чернила каракатицы", 95),
            ("Шампиньоны", 101),
            ("Финик", 104)
        };

        PrintInsertData(data, Mod);

        var ht = new HashTableOA(Mod, keyLenMod: Mod, verbose: true);

        Console.WriteLine("\nВносим данные в таблицу:");
        foreach (var (k, v) in data) ht.Add(k, v);

        Console.Write("\nЗаполненная таблица:");
        ht.Print();

        Console.WriteLine();
        Console.WriteLine("\nЗадание 2.");
        Console.WriteLine("Выполнить удаление из заполненной хэш-таблицы следующих данных в порядке их представления:\n");

        string[] toDelete = { "Орехи грецкие", "Водоросли", "Специи", "Манго" };
        PrintDeleteData(toDelete, data, Mod);

        Console.WriteLine();
        foreach (var k in toDelete)
        {
            Console.WriteLine($"Удаляем: {k};");
            ht.Remove(k);
        }

        Console.Write("\nСостояние таблицы после удалений:");
        ht.Print();
        Console.WriteLine();
    }

    private static void PrintInsertData((string k, int v)[] data, int mod)
    {
        Console.WriteLine("Ключ               | Значение | Длина");
        Console.WriteLine(new string('─', 40));
        foreach (var (k, v) in data)
        {
            int len = k.Length;
            Console.WriteLine($"{k.PadRight(18)} | {v,8} | {len,5}");
        }
    }

    private static void PrintDeleteData(string[] keys, (string k, int v)[] data, int mod)
    {
        var map = new Dictionary<string, int>(data.Length);
        foreach (var (k, v) in data) map[k] = v;

        Console.WriteLine("Ключ               | Значение | Длина");
        Console.WriteLine(new string('─', 40));
        foreach (var key in keys)
        {
            int len = key.Length;
            map.TryGetValue(key, out int val);
            Console.WriteLine($"{key.PadRight(18)} | {val,8} | {len,5}");
        }
    }
}

public sealed class HashTableOA
{
    private enum State { Empty, Occupied, Deleted }

    private sealed class Entry
    {
        public string? Key;
        public int Value;
        public State State;
    }

    private readonly Entry[] _t;
    private readonly int _cap;
    private readonly int _mod;
    private readonly bool _verbose;

    public HashTableOA(int capacity, int keyLenMod, bool verbose = false)
    {
        _cap = capacity;
        _mod = keyLenMod;
        _verbose = verbose;

        _t = new Entry[_cap];
        for (int i = 0; i < _cap; i++) _t[i] = new Entry { State = State.Empty };
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    private int HashIndex(string key) => (key.Length % _mod) % _cap;

    public void Add(string key, int value)
    {
        int start = HashIndex(key);
        int firstDeleted = -1;
        if (_verbose) Console.WriteLine($"Добавляем \"{key}\" (длина {key.Length}, хэш = {start}).");

        for (int step = 0, i = start; step < _cap; step++, i = (i + 1) % _cap)
        {
            var e = _t[i];

            if (e.State == State.Occupied && e.Key == key)
            {
                if (_verbose) Console.WriteLine($" → обновляем {key} в ячейке {i}");
                e.Value = value;
                return;
            }

            if (e.State == State.Deleted && firstDeleted == -1)
                firstDeleted = i;

            if (e.State == State.Empty)
            {
                int place = (firstDeleted != -1) ? firstDeleted : i;
                var dst = _t[place];
                dst.Key = key;
                dst.Value = value;
                dst.State = State.Occupied;
                if (_verbose) Console.WriteLine($" → вставлено в ячейку {place}");
                return;
            }

            if (_verbose) Console.WriteLine($"   Коллизия в {i} (занято \"{e.Key}\") → пробуем {(i + 1) % _cap}");
        }

        throw new InvalidOperationException("Таблица переполнена.");
    }

    public bool Remove(string key)
    {
        int start = HashIndex(key);

        for (int step = 0, i = start; step < _cap; step++, i = (i + 1) % _cap)
        {
            var e = _t[i];

            if (e.State == State.Empty) return false;
            if (e.State == State.Occupied && e.Key == key)
            {
                e.State = State.Deleted;
                e.Key = null;
                e.Value = default;
                return true;
            }
        }
        return false;
    }

    public void Print()
    {
        const int wKey = 18;
        Console.WriteLine("\nИндекс| Состояние| Ключ               | Значение | Хэш (длина % 20)");
        Console.WriteLine(new string('─', 68));

        for (int i = 0; i < _cap; i++)
        {
            var e = _t[i];
            string state = e.State switch
            {
                State.Empty => "Empty   ",
                State.Deleted => "Deleted ",
                _ => "Occupied"
            };

            string key = e.Key ?? "";
            string hashTxt = e.State == State.Occupied ? (key.Length % _mod).ToString() : "";

            Console.WriteLine($"{i,5} | {state} | {TrimPad(key, wKey)} | {((e.State == State.Occupied) ? e.Value.ToString() : ""),8} | {hashTxt,8}");
        }
    }

    private static string TrimPad(string s, int n)
    {
        if (s.Length > n) return string.Concat(s.AsSpan(0, n - 1), "…");
        return s.PadRight(n);
    }
}
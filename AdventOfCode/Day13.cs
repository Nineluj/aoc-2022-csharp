namespace AdventOfCode;

public sealed class Day13 : CustomDirBaseDay
{
    private readonly string _input;

    public Day13()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    private static int[] FindSplitIndices(string l)
    {
        var indices = new List<int>();
        var subListCount = 0;
        for (var i = 0; i < l.Length; i++)
            switch (l[i])
            {
                case ',' when subListCount == 0:
                    indices.Add(i);
                    break;
                case '[':
                    subListCount++;
                    break;
                case ']':
                    subListCount--;
                    break;
            }

        return indices.ToArray();
    }

    private static string[] SplitPacketList(string s)
    {
        var inner = s[1..^1];
        var splitIndices = FindSplitIndices(inner);
        if (!splitIndices.Any()) return inner.Length == 0 ? Array.Empty<string>() : new[] { inner };

        var start = 0;
        var parts = new List<string>();
        foreach (var index in splitIndices)
        {
            parts.Add(inner[start..index]);
            start = index + 1;
        }

        if (splitIndices.Any()) parts.Add(inner[start..]);
        return parts.ToArray();
    }

    public static IPacket ParsePacket(string s)
    {
        if (s.StartsWith("["))
            return new PacketList(
                SplitPacketList(s)
                    .Select(ParsePacket)
                    .ToList());

        return new PacketInt(int.Parse(s));
    }

    private IEnumerable<Tuple<IPacket, IPacket>> GetPairs(string input)
    {
        return Utils
            .GetLines(input)
            .Chunk(2)
            .Select(chunk =>
                new Tuple<IPacket, IPacket>(ParsePacket(chunk[0]), ParsePacket(chunk[1])));
    }

    private static Ordered IsPacketListRightOrdered(PacketList l1, PacketList l2)
    {
        while (true)
        {
            var emptyL1 = !l1.Content.Any();
            var emptyL2 = !l2.Content.Any();

            if (emptyL1) return Ordered.True;
            if (emptyL2) return Ordered.False; // must be false if L1 is non-empty

            var firstItemsOrdering = IsPairRightOrdered(l1.Content[0], l2.Content[0]);
            if (firstItemsOrdering == Ordered.False) return Ordered.False;

            var l1Remain = l1.Content.Slice(1).ToList();
            var l2Remain = l2.Content.Slice(1).ToList();

            if (firstItemsOrdering == Ordered.True && l2Remain.Count == 0) return Ordered.True;

            l1 = new PacketList(l1Remain);
            l2 = new PacketList(l2Remain);
        }
    }

    private static Ordered IsPairRightOrdered(IPacket p1, IPacket p2)
    {
        return p1 switch
        {
            PacketInt x1 when p2 is PacketInt x2 => x1.Value < x2.Value
                ? Ordered.True
                : x1.Value > x2.Value
                    ? Ordered.False
                    : Ordered.Unknown,
            PacketList l1 when p2 is PacketList l2 => IsPacketListRightOrdered(l1, l2),
            PacketList l1 when p2 is PacketInt x2 => IsPairRightOrdered(l1, new PacketList(new List<IPacket> { x2 })),
            PacketInt x1 when p2 is PacketList l2 => IsPairRightOrdered(new PacketList(new List<IPacket> { x1 }), l2),
            _ => throw new ArgumentOutOfRangeException(nameof(p1), p1, null)
        };
    }

    private static bool IsPairRightOrderedBool(IPacket p1, IPacket p2)
    {
        return IsPairRightOrdered(p1, p2) == Ordered.True;
    }

    public override ValueTask<string> Solve_1()
    {
        var pairs = GetPairs(_input);

        // foreach (var pp in pairs)
        // {
        //     Console.WriteLine(IsPairRightOrderedBool(pp.Item1, pp.Item2) ? $"Yes: {pp}" : $"no: {pp}");
        // }

        var result = pairs
                .Select((pp, i) => IsPairRightOrderedBool(pp.Item1, pp.Item2) ? i + 1 : 0)
            // .Sum()
            ;
        return new ValueTask<string>(string.Join(",", result));
        return new ValueTask<string>(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var result = "";
        throw new NotImplementedException();
        // return new ValueTask<string>(result.ToString());
    }

    private enum Ordered
    {
        True,
        False,
        Unknown
    }

    public interface IPacket
    {
    }

    private record PacketList(List<IPacket> Content) : IPacket
    {
        public override string ToString()
        {
            var inner = string.Join(',', Content);
            return $"[{inner}]";
        }
    }

    private record PacketInt(int Value) : IPacket
    {
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
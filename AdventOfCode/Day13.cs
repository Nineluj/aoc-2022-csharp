namespace AdventOfCode;

public sealed class Day13 : CustomDirBaseDay
{
    public enum Ordered
    {
        True,
        False,
        Unknown
    }

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

            if (emptyL1 && emptyL2) return Ordered.Unknown;
            if (emptyL1) return Ordered.True;
            if (emptyL2) return Ordered.False;

            var firstItemsOrdering = IsPairRightOrdered(l1.Content[0], l2.Content[0]);
            if (firstItemsOrdering != Ordered.Unknown) return firstItemsOrdering;

            var l1Remain = l1.Content.Slice(1).ToList();
            var l2Remain = l2.Content.Slice(1).ToList();

            l1 = new PacketList(l1Remain);
            l2 = new PacketList(l2Remain);
        }
    }

    public static Ordered IsPairRightOrdered(IPacket p1, IPacket p2)
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

    public static bool IsPairRightOrderedBool(IPacket p1, IPacket p2)
    {
        return IsPairRightOrdered(p1, p2) != Ordered.False;
    }

    public override ValueTask<string> Solve_1()
    {
        var pairs = GetPairs(_input).ToList();
        var result = pairs
            .Select((pp, i) => IsPairRightOrderedBool(pp.Item1, pp.Item2) ? i + 1 : 0)
            .Sum();
        return new ValueTask<string>(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var packets = Utils.GetLines(_input).Select(ParsePacket).ToList();
        var dividerPacket1 = ParsePacket("[[2]]");
        var dividerPacket2 = ParsePacket("[[6]]");
        packets.Add(dividerPacket1);
        packets.Add(dividerPacket2);
        packets.Sort((a, b) => IsPairRightOrderedBool(a, b) ? -1 : 1);
        // I hate one indexing
        var result = (1 + packets.IndexOf(dividerPacket1)) * (1 + packets.IndexOf(dividerPacket2));
        return new ValueTask<string>(result.ToString());
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
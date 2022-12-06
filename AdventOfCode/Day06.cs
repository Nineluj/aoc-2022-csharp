using System.Collections;

namespace AdventOfCode;

public sealed class Day06 : BaseDay
{
    private readonly string _input;

    public Day06()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    private bool AreItemsUnique<T>(LinkedList<T> linkedList)
    {
        return linkedList.Distinct().Count() == linkedList.Count;
    }
    private int IndexOfStartOfPacketMarker(string input, int k)
    {
        var items = new LinkedList<char>(input[..k]);
        for (var i = k; i < input.Length; i++)
        {
            if (AreItemsUnique(items))
            {
                return i;
            }
            
            items.RemoveFirst();
            items.AddLast(input[i]);
        }

        throw new KeyNotFoundException("couldn't find start of packet marker");
    }

    public override ValueTask<string> Solve_1()
    {
        var result = IndexOfStartOfPacketMarker(Utils.GetLines(_input).ToList()[0], 4);
        return new ValueTask<string>(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var result = IndexOfStartOfPacketMarker(Utils.GetLines(_input).ToList()[0], 14);
        return new ValueTask<string>(result.ToString());
    }
}
using System.Collections;

namespace AdventOfCode;

public sealed class Day06 : BaseDay
{
    private readonly string _input;

    public Day06()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    private int IndexOfStartOfPacketMarker(string input, int k)
    {
        var counts = new Dictionary<char, uint>();
        var left = 0;
        var right = 0;

        /*
         * move right as far as possible until:
         * - reached window size = k => return
         * - we have a duplicate character, move left until there is no longer a dupe
         * will iterate over each value at most twice (once for left and once for right index pointers)
         */
        while (right < input.Length)
        {
            var rightItem = input[right];
            counts.TryGetValue(rightItem, out var rightCharCount);
            
            if (rightCharCount != 0)
            {
                while (counts[rightItem] > 0)
                {
                    var leftItem = input[left];
                    counts[leftItem] -= 1;
                    left++;
                }
            }
            else if (right - left == k)
            {
                return right;
            }

            counts[rightItem] = 1;
            right++;
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
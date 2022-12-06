using System.Text.RegularExpressions;

namespace AdventOfCode;

public sealed class Day05 : CustomDirBaseDay
{
    private readonly string _input;

    public Day05()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    private record HanoiInstruction(int Quantity, int FromPile, int ToPile);
    private record HanoiGame(Dictionary<int, Stack<char>> Stacks);
    private record HanoiGameWithInstructions(HanoiGame Game, List<HanoiInstruction> Instructions);
    
    private static Dictionary<int, Stack<char>> GetStacks(List<string> lines)
    {
        var result = new Dictionary<int, Stack<char>>();
        // last line contains the indices, the other ones are the stacks
        var indicesLine = lines.Last();

        for (var x = 0; x < indicesLine.Length; x++)
        {
            var indexVal = indicesLine[x];
            if (indexVal == ' ') continue;
            
            var stack = new Stack<char>();
            for (var y = lines.Count - 2; y >= 0; y--)
            {
                var stackVal = lines[y][x];
                if (stackVal == ' ') break;
                stack.Push(stackVal);
            }
            result.Add(int.Parse(indexVal.ToString()), stack);
        }

        return result;
    }

    private static List<HanoiInstruction> GetInstructions(List<string> lines)
    {
        var instructionRe = new Regex(@"move (\d+) from (\d+) to (\d+)");
        return lines.Select(t => instructionRe.Match(t).Groups.Values
                .Skip(1)
                .Select(v => int.Parse(v.Value)).ToList())
                .Select(values => 
                    new HanoiInstruction(Quantity: values[0], FromPile: values[1], ToPile: values[2]))
                .ToList();
    }
    private static HanoiGameWithInstructions ParseInput(string input)
    {
        var text = Utils.GetLinesIncludeEmpty(input).ToList();
        var i = 0;
        for (; i < text.Count; i++)
        {
            if (!text[i].Any()) break;
        }

        var stacks = GetStacks(text.GetRange(0, i));
        var instructions = GetInstructions(text.GetRange(i + 1, text.Count - (i + 1)));

        return new HanoiGameWithInstructions(new HanoiGame(Stacks: stacks), Instructions: instructions);
    }

    private static HanoiGame RunInstructions(HanoiGame game, IEnumerable<HanoiInstruction> instructions)
    {
        foreach (var inst in instructions)
        {
            var from = game.Stacks[inst.FromPile];
            var to = game.Stacks[inst.ToPile];
            for (var i = 0; i < inst.Quantity; i++)
            {
                to.Push(from.Pop());
            }
        }

        return game;
    }

    private static HanoiGame RunInstructionsMultiMove(HanoiGame game, IEnumerable<HanoiInstruction> instructions)
    {
        foreach (var inst in instructions)
        {
            var from = game.Stacks[inst.FromPile];
            var to = game.Stacks[inst.ToPile];
            var itemsToMove = new Stack<char>();
            for (var i = 0; i < inst.Quantity; i++)
            {
                itemsToMove.Push(from.Pop());
            }

            foreach (var item in itemsToMove)
            {
                to.Push(item);
            }
        }

        return game;
    }

    public override ValueTask<string> Solve_1()
    {
        var gameWithInstructions = ParseInput(_input);
        var topItems = RunInstructions(gameWithInstructions.Game, gameWithInstructions.Instructions)
            .Stacks.Select(stack => stack.Value.Peek());
        return new ValueTask<string>(string.Join("", topItems));
    }

    public override ValueTask<string> Solve_2()
    {
        var gameWithInstructions = ParseInput(_input);
        var topItems = RunInstructionsMultiMove(gameWithInstructions.Game, gameWithInstructions.Instructions)
            .Stacks.Select(stack => stack.Value.Peek());
        return new ValueTask<string>(string.Join("", topItems));
    }
}

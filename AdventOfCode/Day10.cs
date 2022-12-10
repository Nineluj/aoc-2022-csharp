namespace AdventOfCode;

public sealed class Day10 : CustomDirBaseDay
{
    private readonly string _input;

    public Day10()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    private IEnumerable<CathodeInstruction> ParseInstructions(string input)
    {
        return Utils.GetLines(input).Select<string, CathodeInstruction>(line =>
        {
            if (line.StartsWith("noop")) return new NoopInstruction();
            if (line.StartsWith("addx")) return new AddXInstruction(int.Parse(line[5..]));
            throw new ArgumentException($"Unsupported instruction: {line}");
        });
    }

    private int SimulateInstructions(List<CathodeInstruction> instructions, ICollection<int> snapshotCycles)
    {
        var signalSum = 0;
        var instructionIndex = -1;
        var register = 1;

        CathodeInstruction pendingInstruction = new NoopInstruction();
        var timeLeftForInstruction = 0;

        for (var cycle = 1; cycle <= snapshotCycles.Max(); cycle++)
        {
            if (timeLeftForInstruction == 0)
            {
                register += pendingInstruction.GetIncrement();
                instructionIndex++;
                pendingInstruction = instructions[instructionIndex];
                timeLeftForInstruction = pendingInstruction.GetCyclesRequiredToComplete();
            }

            timeLeftForInstruction -= 1;

            if (snapshotCycles.Contains(cycle))
                // Console.WriteLine($"{cycle}: register {register}, signal strength {register * cycle}");
                signalSum += register * cycle;
        }

        return signalSum;
    }

    public override ValueTask<string> Solve_1()
    {
        var instructions = ParseInstructions(_input);
        var result = SimulateInstructions(
            instructions.ToList(), new HashSet<int> { 20, 60, 100, 140, 180, 220 });
        return new ValueTask<string>(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var result = "";
        var crt = new CRTEmulator();
        crt.DrawCRT();
        throw new NotImplementedException();
        // return new ValueTask<string>(result.ToString());
    }

    private abstract class CathodeInstruction
    {
        public abstract int GetCyclesRequiredToComplete();
        public abstract int GetIncrement();
    }

    private class NoopInstruction : CathodeInstruction
    {
        public override int GetCyclesRequiredToComplete()
        {
            return 1;
        }

        public override int GetIncrement()
        {
            return 0;
        }
    }

    private class AddXInstruction : CathodeInstruction
    {
        private readonly int _x;

        public AddXInstruction(int val)
        {
            _x = val;
        }

        public override int GetCyclesRequiredToComplete()
        {
            return 2;
        }

        public override int GetIncrement()
        {
            return _x;
        }
    }


    private class CRTEmulator
    {
        private readonly char[,] _data;
        private readonly int CRTHeight = 6;
        private readonly int CRTWidth = 40;

        public CRTEmulator()
        {
            _data = new char[CRTHeight, CRTWidth];
            for (var y = 0; y < _data.GetLength(0); y++)
            for (var x = 0; x < _data.GetLength(1); x++)
                _data[y, x] = '.';
        }

        public void DrawCRT()
        {
            var delimiter = new string('=', 45);
            Console.WriteLine(delimiter);
            Console.WriteLine("CRT");
            for (var y = 0; y < _data.GetLength(0); y++)
            {
                for (var x = 0; x < _data.GetLength(1); x++) Console.Write(_data[y, x]);
                Console.WriteLine("");
            }

            Console.WriteLine(delimiter);
        }
    }
}
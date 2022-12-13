using System.Text;

namespace AdventOfCode;

public sealed class Day10 : CustomDirBaseDay
{
    private readonly string _input;

    public Day10()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    private static List<CathodeInstruction> ParseInstructions(string input)
    {
        return Utils.GetLines(input).Select<string, CathodeInstruction>(line =>
        {
            if (line.StartsWith("noop")) return new NoopInstruction();
            if (line.StartsWith("addx")) return new AddXInstruction(int.Parse(line[5..]));
            throw new ArgumentException($"Unsupported instruction: {line}");
        }).ToList();
    }


    public void SimulateInstructions(IEnumerable<CathodeInstruction> instructions, Action<int, int> action,
        int cycleMax)
    {
        var register = 1;

        CathodeInstruction pendingInstruction = new NoopInstruction();
        CathodeInstruction currentInstruction = new NoopInstruction();
        var timeLeftForInstruction = 0;

        using var instructionEnumerator = instructions.GetEnumerator();
        for (var cycle = 1; cycle <= cycleMax; cycle++)
        {
            // start
            if (timeLeftForInstruction == 0)
            {
                currentInstruction = pendingInstruction;
                pendingInstruction = Utils.EnumeratorGetNext(instructionEnumerator);
                timeLeftForInstruction = pendingInstruction.GetCyclesRequiredToComplete();
            }

            // during 
            register += currentInstruction.GetIncrement();
            action(cycle, register);

            // after
            currentInstruction = new NoopInstruction();
            timeLeftForInstruction -= 1;
        }
    }

    public override ValueTask<string> Solve_1()
    {
        var instructions = ParseInstructions(_input);
        var signalSum = 0;

        void SnapshotFn(int cycle, int register)
        {
            if ((cycle - 20) % 40 == 0) signalSum += cycle * register;
        }

        SimulateInstructions(
            instructions, SnapshotFn, 220);
        return new ValueTask<string>(signalSum.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var instructions = ParseInstructions(_input);
        var crt = new CRTEmulator();
        SimulateInstructions(instructions, crt.DrawAt, 240);
        return new ValueTask<string>(crt.GetCRTDrawing());
    }

    public abstract class CathodeInstruction
    {
        public abstract int GetCyclesRequiredToComplete();
        public abstract int GetIncrement();
    }

    public class NoopInstruction : CathodeInstruction
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

    public class AddXInstruction : CathodeInstruction
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


    public class CRTEmulator
    {
        private const int CRTHeight = 6;
        private const int CRTWidth = 40;
        private readonly char[,] _data;

        public CRTEmulator()
        {
            _data = new char[CRTHeight, CRTWidth];
            for (var y = 0; y < _data.GetLength(0); y++)
            for (var x = 0; x < _data.GetLength(1); x++)
                _data[y, x] = '.';
        }

        public void DrawAt(int cycle, int register)
        {
            var zeroIndexedCycle = cycle - 1;
            var drawPos = zeroIndexedCycle % 40;
            var row = zeroIndexedCycle / 40;
            if (Math.Abs(drawPos - register) <= 1) _data[row, drawPos] = '#';
        }

        public string GetCRTDrawing()
        {
            var sb = new StringBuilder();
            var delimiter = new string('=', 45);
            sb.AppendLine(delimiter);
            sb.AppendLine("CRT");
            for (var y = 0; y < CRTHeight; y++)
            {
                for (var x = 0; x < CRTWidth; x++) sb.Append(_data[y, x]);
                sb.AppendLine();
            }

            sb.AppendLine(delimiter);
            return sb.ToString();
        }
    }
}
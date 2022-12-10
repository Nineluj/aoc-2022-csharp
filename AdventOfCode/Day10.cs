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


    private void SimulateInstructions(List<CathodeInstruction> instructions, Action<int, int> fn, int cycleMax)
    {
        var instructionIndex = -1;
        var register = 1;

        CathodeInstruction pendingInstruction = new NoopInstruction();
        var timeLeftForInstruction = 0;

        for (var cycle = 1; cycle <= cycleMax; cycle++)
        {
            if (timeLeftForInstruction == 0)
            {
                register += pendingInstruction.GetIncrement();
                instructionIndex++;
                pendingInstruction = instructions[instructionIndex];
                timeLeftForInstruction = pendingInstruction.GetCyclesRequiredToComplete();
            }

            timeLeftForInstruction -= 1;
            fn(cycle, register);
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

        void DrawToCRTFn(int cycle, int register)
        {
            if (Math.Abs(cycle - register) <= 1) crt.DrawAt(cycle);
        }

        SimulateInstructions(instructions, DrawToCRTFn, 240);
        return new ValueTask<string>(crt.GetCRTDrawing());
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
        private const int CRTHeight = 6;
        private const int CRTWidth = 40;
        private readonly char[] _data;

        public CRTEmulator()
        {
            _data = new char[CRTHeight * CRTWidth];
            for (var i = 0; i < _data.Length; i++) _data[i] = '.';
        }

        public void DrawAt(int index)
        {
            _data[index - 1] = '#';
        }

        public string GetCRTDrawing()
        {
            var sb = new StringBuilder();
            var delimiter = new string('=', 45);
            sb.AppendLine(delimiter);
            sb.AppendLine("CRT");
            for (var y = 0; y < CRTHeight; y++)
            {
                for (var x = 0; x < CRTWidth; x++) sb.Append(_data[y * CRTHeight + x]);
                sb.AppendLine();
            }

            sb.AppendLine(delimiter);
            return sb.ToString();
        }
    }
}
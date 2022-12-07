using System.Text.RegularExpressions;

namespace AdventOfCode;

public sealed class Day07 : CustomDirBaseDay
{
    private readonly Regex _cdRe = new(@"\$ cd (\S+)");
    private readonly string _input;
    private readonly Regex _lsDirRe = new(@"dir (\S+)");
    private readonly Regex _lsFileRe = new(@"(\d+) (\S+)");

    public Day07()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    private FS ParseFromTerminal(string input)
    {
        var lines = Utils.GetLines(input).ToList();
        var currentPath = new Path();

        var lookup = new Dictionary<string, FSItem>();
        var root = new FSDir("/");
        lookup.Add("/", root);

        for (var i = 1; i < lines.Count; i++)
        {
            var l = lines[i];
            if (l.StartsWith("$ cd"))
            {
                var cdDest = Utils.GetFirstMatchRegex(_cdRe, l);
                currentPath.ChangeDirectory(cdDest);
            }
            else if (l.StartsWith("$ ls"))
            {
                var j = i + 1;
                while (j < lines.Count && !lines[j].StartsWith("$"))
                {
                    var l2 = lines[j];
                    FSItem newItem;

                    if (l2.StartsWith("dir"))
                    {
                        var m = Utils.GetAllRegexMatches(_lsDirRe, l2).ToList();
                        newItem = new FSDir(m[0]);
                    }
                    else
                    {
                        var m = Utils.GetAllRegexMatches(_lsFileRe, l2).ToList();
                        newItem = new FSFile(m[1], int.Parse(m[0]));
                    }

                    lookup[currentPath.GetPathForItemInDirectory(newItem.Name)] = newItem;
                    (lookup[currentPath.GetPathString()] as FSDir)?.Content.Add(newItem);

                    j++;
                }

                i = j - 1;
            }
            else
            {
                throw new SolvingException("reached unexpected line");
            }
        }

        return new FS(root, lookup);
    }

    public override ValueTask<string> Solve_1()
    {
        var maxDirSize = 100_000;
        var fs = ParseFromTerminal(_input);

        var totalSize = fs.Lookup
            .Select(k => k.Value)
            .Where(x => x is FSDir)
            .Select(x => x.GetContentSize())
            .Where(x => x < maxDirSize)
            .Sum();
        return new ValueTask<string>(totalSize.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        const int diskSpace = 70_000_000;
        const int requiredSpace = 30_000_000;

        var fs = ParseFromTerminal(_input);

        var usedSpace = fs.RootDir.GetContentSize();
        var spaceToFree = requiredSpace - (diskSpace - usedSpace);

        var bestCandidate = fs.Lookup.Select(pair => pair.Value)
            .Where(x => x is FSDir)
            .Select(x => x.GetContentSize())
            .Where(size => size >= spaceToFree).MinBy(x => x);

        return new ValueTask<string>(bestCandidate.ToString());
    }

    private class Path
    {
        private readonly LinkedList<string> _data = new();

        public void ChangeDirectory(string destination)
        {
            if (destination == "..")
                _data.RemoveLast();
            else
                _data.AddLast(destination);
        }

        public string GetPathString()
        {
            if (!_data.Any()) return "/";

            return "/" + string.Join('/', _data);
        }

        public string GetPathForItemInDirectory(string itemName)
        {
            return GetPathString() + (_data.Any() ? "/" : "") + itemName;
        }
    }

    private abstract class FSItem
    {
        public FSItem(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public abstract int GetContentSize();
    }

    private class FSDir : FSItem
    {
        public readonly List<FSItem> Content = new();

        // used to avoid recomputing the directory size multiple times
        // won't work after a child node changes
        private int? _cachedSize;

        public FSDir(string name) : base(name)
        {
        }

        public override int GetContentSize()
        {
            if (_cachedSize is null)
            {
                var size = Content.Select(x => x.GetContentSize()).Sum();
                _cachedSize = size;
            }

            return _cachedSize.Value;
        }
    }

    private class FSFile : FSItem
    {
        internal FSFile(string name, int size) : base(name)
        {
            Size = size;
        }

        public int Size { get; }

        public override int GetContentSize()
        {
            return Size;
        }
    }

    private record FS(FSDir RootDir, Dictionary<string, FSItem> Lookup);
}
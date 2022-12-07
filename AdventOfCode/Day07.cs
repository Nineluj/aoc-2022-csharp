using System.Text.RegularExpressions;

namespace AdventOfCode;

public sealed class Day07 : CustomDirBaseDay
{
    private readonly string _input;

    public Day07()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    private string GetPathString(LinkedList<string> path)
    {
        return "/" + string.Join('/', path);
    }

    private string GetPathParentString(LinkedList<string> path)
    {
        return "/" + string.Join('/', path.SkipLast(1));
    }

    private FS ParseFromTerminal(string input)
    {
        var lines = Utils.GetLines(input).ToList();
        var cdRe = new Regex(@"\$ cd (\S+)");
        var lsFileRe = new Regex(@"(\d+) (\S+)");
        var lsDirRe = new Regex(@"dir (\S+)");

        var currentPath = new LinkedList<string>();

        var lookup = new Dictionary<string, FSItem>();
        var root = new FSDir("/");
        lookup.Add("/", root);

        for (var i = 1; i < lines.Count; i++)
        {
            var l = lines[i];
            if (l.StartsWith("$ cd"))
            {
                var cdDest = Utils.GetFirstMatchRegex(cdRe, l);
                if (cdDest == "..")
                    currentPath.RemoveLast();
                else
                    currentPath.AddLast(cdDest);
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
                        var m = Utils.GetAllRegexMatches(lsDirRe, l2).ToList();
                        newItem = new FSDir(m[0]);
                    }
                    else
                    {
                        var m = Utils.GetAllRegexMatches(lsFileRe, l2).ToList();
                        newItem = new FSFile(m[1], int.Parse(m[0]));
                    }

                    currentPath.AddLast(newItem.Name);
                    lookup[GetPathString(currentPath)] = newItem;
                    (lookup[GetPathParentString(currentPath)] as FSDir)?.Content.Add(newItem);
                    currentPath.RemoveLast();

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
        var maxDirSize = 100000;
        var fs = ParseFromTerminal(_input);

        var largeSizeDirectoryTotal = 0;
        foreach (var (_, item) in fs.Lookup)
        {
            if (item is not FSDir) continue;
            var contentSize = item.GetContentSize();
            if (contentSize < maxDirSize) largeSizeDirectoryTotal += contentSize;
        }

        return new ValueTask<string>(largeSizeDirectoryTotal.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var diskSpace = 70000000;
        var requiredSpace = 30000000;
        var fs = ParseFromTerminal(_input);

        var usedSpace = fs.RootDir.GetContentSize();
        var remainingSpace = diskSpace - usedSpace;
        var spaceToFree = requiredSpace - remainingSpace;

        var bestCandidate = fs.Lookup.Select(pair => pair.Value)
            .Where(x => x is FSDir)
            .Select(x => x.GetContentSize())
            .Where(size => size >= spaceToFree).MinBy(x => x);

        return new ValueTask<string>(bestCandidate.ToString());
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
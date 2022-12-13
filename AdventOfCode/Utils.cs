using System.Text.RegularExpressions;

namespace AdventOfCode;

public static class Utils
{
    public static int Mod(int x, int m)
    {
        return (x % m + m) % m;
    }

    public static IEnumerable<string> GetLines(string input)
    {
        return input.Split(Environment.NewLine).Where(line => line.Length != 0);
    }

    public static IEnumerable<string> GetLinesIncludeEmpty(string input)
    {
        return input.Split(Environment.NewLine);
    }

    public static string GetFirstMatchRegex(Regex regexp, string text)
    {
        return regexp.Match(text).Groups.Values.Skip(1).First().Value;
    }

    public static IEnumerable<string> GetAllRegexMatches(Regex regexp, string text)
    {
        return regexp.Match(text).Groups.Values.Skip(1).Select(g => g.Value);
    }

    public static T EnumeratorGetNext<T>(IEnumerator<T> list)
    {
        list.MoveNext();
        return list.Current;
    }

    // thanks to: https://stackoverflow.com/a/44626295
    public static IEnumerable<T> Slice<T>(this List<T> source, int from, int to)
    {
        return source.GetRange(from, to - from);
    }

    public static IEnumerable<T> Slice<T>(this List<T> source, int from)
    {
        return source.GetRange(from, source.Count - from);
    }
}
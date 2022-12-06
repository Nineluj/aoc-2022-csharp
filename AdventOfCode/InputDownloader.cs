using System.Net;

namespace AdventOfCode;

public static class InputDownloader
{
    public static async Task Download(string sessionToken, List<uint> days, uint year)
    {
        {
            if (sessionToken is null)
            {
                sessionToken = Environment.GetEnvironmentVariable("SESSION_TOKEN");
                if (sessionToken is null) Environment.FailFast("Couldn't get session token as option or env variable");
            }

            var day = days.Any() ? days[0] : (uint)DateTime.Today.Day;
            var baseAddress = new Uri("https://adventofcode.com");
            var cookieContainer = new CookieContainer();
            using var handler = new HttpClientHandler { CookieContainer = cookieContainer };
            using var client = new HttpClient(handler) { BaseAddress = baseAddress };

            cookieContainer.Add(baseAddress, new Cookie("session", sessionToken));
            var result = await client.GetAsync($"/{year}/day/{day}/input");
            result.EnsureSuccessStatusCode();
            Console.WriteLine("Input was successfully retrieved");

            var outFilePath = $"{day:D2}.txt";
            await using var output = File.OpenWrite(outFilePath);
            await using var input = await result.Content.ReadAsStreamAsync();
            await input.CopyToAsync(output);
            Console.WriteLine($"Saved to file {outFilePath}");

            if (Environment.OSVersion.Platform != PlatformID.Unix)
            {
                output.Close();
                // convert to this system's newline encoding in case it's not Unix
                File.WriteAllLines(outFilePath,
                    File.ReadAllLines(outFilePath)
                        .Select(line => line.Replace("\n", Environment.NewLine)));
            }
        }
    }
}
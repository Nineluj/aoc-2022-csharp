namespace AdventOfCode;

public sealed class SettingsSingleton
{
    private static SettingsSingleton _instance;
    public string InputFileDirPath = "Inputs";

    private SettingsSingleton()
    {
    }

    public static SettingsSingleton Instance => _instance ??= new SettingsSingleton();
}
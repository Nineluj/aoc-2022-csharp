namespace AdventOfCode;

public sealed class SettingsSingleton
{
    public string InputFileDirPath = "Inputs";

    private SettingsSingleton() {}
    private static SettingsSingleton _instance = null;
    public static SettingsSingleton Instance => _instance ??= new SettingsSingleton();
}

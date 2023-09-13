

namespace SchemaGenerator;

using System;
using System.IO;

/// <summary>
/// Utility class to assist in resolving file-system paths.
/// </summary>
public static class Repo
{
    private static string RootFolder { get; } = GetRoot();

    private static string _RootConfigFolder = string.Empty;
    public static string RootConfigFolder
    {
        get
        {
            return string.IsNullOrEmpty(_RootConfigFolder) ? $@"{Repo.RootFolder}\NlpSql.config" : _RootConfigFolder;
        }
        set
        {
            _RootConfigFolder = value;
        }
    }

    public static void CheckPath()
    {
        var subdirs = new string[] { "schema", "nlpsql" };
        foreach (var sub in subdirs)
        {
            var dir = Path.Combine(RootConfigFolder, sub);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
    }

    private static string GetRoot()
    {
        var current = Environment.CurrentDirectory;

        var folder = new DirectoryInfo(current);

        while (!File.Exists(Path.Combine(folder.FullName, "NlpSql.sln")))
        {
            folder =
                folder.Parent ??
                throw new DirectoryNotFoundException($"Unable to locate repo root folder: {current}");
        }

        return folder.FullName;
    }
}
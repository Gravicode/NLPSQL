

namespace SemanticKernel.Data.NlpSql;

using System;
using System.IO;
using System.Runtime.CompilerServices;

/// <summary>
/// Utility class to assist in resolving file-system paths.
/// </summary>
public static class Repo
{
    private static string RootFolder { get; } = GetRoot();

    private static string _RootConfigFolder = string.Empty;
    public static string RootConfigFolder { 
        get {
            return string.IsNullOrEmpty(_RootConfigFolder) ? $@"{Repo.RootFolder}\NlpSql.config" : _RootConfigFolder;
        }
        set {
            _RootConfigFolder = value;
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

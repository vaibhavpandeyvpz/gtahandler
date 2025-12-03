using System.Collections.Generic;
using System.IO;
using Unpaker;

namespace GTAHandler.Models;

/// <summary>
/// Stores metadata about a pak file that was opened
/// </summary>
public class PakFileInfo
{
    public string OriginalPakPath { get; set; } = string.Empty;
    public string HandlingCfgPathInPak { get; set; } = string.Empty;
    public string TempExtractedPath { get; set; } = string.Empty;
    public Version Version { get; set; } = Version.V11;
    public string MountPoint { get; set; } = string.Empty;
    public ulong? PathHashSeed { get; set; }
    public List<Compression> CompressionMethods { get; set; } = new();
    public byte[]? AesKey { get; set; }
    public PakReader? OriginalPakReader { get; set; }
    public Stream? OriginalPakStream { get; set; }
}


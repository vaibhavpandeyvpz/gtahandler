using System;

namespace GTAHandler.Services;

public enum ParseErrorType
{
    FileNotFound,
    FileEmpty,
    FileUnreadable,
    InvalidFormat,
    WrongGameFormat,
    NoVehiclesFound
}

public class HandlingParseException : Exception
{
    public ParseErrorType ErrorType { get; }
    public string? Details { get; }

    public HandlingParseException(ParseErrorType errorType, string message, string? details = null)
        : base(message)
    {
        ErrorType = errorType;
        Details = details;
    }

    public HandlingParseException(ParseErrorType errorType, string message, Exception innerException)
        : base(message, innerException)
    {
        ErrorType = errorType;
    }

    public static HandlingParseException FileNotFound(string path)
        => new(ParseErrorType.FileNotFound, $"File not found: {path}");

    public static HandlingParseException FileEmpty(string path)
        => new(ParseErrorType.FileEmpty, "The file is empty or contains no data.");

    public static HandlingParseException FileUnreadable(string path, Exception ex)
        => new(ParseErrorType.FileUnreadable, $"Cannot read file: {ex.Message}", ex);

    public static HandlingParseException InvalidFormat(string details)
        => new(ParseErrorType.InvalidFormat, "The file format is invalid or corrupted.", details);

    public static HandlingParseException WrongGameFormat(string expectedGame, int expectedColumns, int actualColumns)
        => new(ParseErrorType.WrongGameFormat,
            $"This file doesn't appear to be a valid {expectedGame} handling.cfg file.",
            $"Expected {expectedColumns} columns per vehicle, but found {actualColumns}. " +
            "Please check that you've selected the correct game.");

    public static HandlingParseException NoVehiclesFound()
        => new(ParseErrorType.NoVehiclesFound,
            "No vehicles found in the file.",
            "The file may be empty, contain only comments, or have an unrecognized format.");
}




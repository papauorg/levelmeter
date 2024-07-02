using System.Drawing;

using UnitsNet;

public record GraduationMark
{
    /// <summary>
    /// Defines the volume level of this specific graduation mark.
    /// </summary>
    public required Volume Volume { get; init; }

    /// <summary>
    /// The position of this graduation mark.
    /// </summary>
    public required (Length X, Length Y) Position { get; init; }

    /// <summary>
    /// The accompanying text for the graduation mark.
    /// </summary>
    public required string Text { get; init; }

    /// <summary>
    /// Length of the current mark.
    /// </summary>
    public required Length Length { get; init; }

    /// <summary>
    /// Height of the current mark.
    /// </summary>
    public required Length Height { get; init; }

    /// <summary>
    /// Font name or path to font file. Defines the font to write texts for the graduation mark.
    /// </summary>
    public required GraduationMarkSettings.FontSettings Font { get; init; }
}
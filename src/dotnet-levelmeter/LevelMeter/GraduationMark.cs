using System.Drawing;

using UnitsNet;

public record GraduationMark
{
    /// <summary>
    /// Defines the volume level of this specific graduation mark.
    /// </summary>
    public Volume Volume { get; init; } = Volume.FromLiters(0);

    /// <summary>
    /// The position of this graduation mark.
    /// </summary>
    public (Length X, Length Y) Position { get; init; } = (Length.Zero, Length.Zero);

    /// <summary>
    /// The accompanying text for the graduation mark.
    /// </summary>
    public string Text { get; init; } = string.Empty;

    /// <summary>
    /// Length of the current mark.
    /// </summary>
    public Length Length { get; init; } = Length.Zero;

    /// <summary>
    /// Height of the current markH
    /// </summary>
    public Length Height { get; init; } = Length.Zero;

    /// <summary>
    /// Font name or path to font file. Defines the font to write texts for the graduation mark.
    /// </summary>
    public string Font { get; init; } = "Arial Black";
    
    /// <summary>
    /// Font size for written text.
    /// </summary>
    public double FontSize { get; init; } = 7;
}
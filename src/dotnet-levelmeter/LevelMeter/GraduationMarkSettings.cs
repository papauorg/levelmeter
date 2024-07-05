
public record GraduationMarkSettings
{
    public enum TextAlignment { Left = 0, Center = 1, Right = 2 }
    public record FontSettings(string Family = "Helvetica", double Size = 5, double OffsetX = 0, double OffsetY = 0, TextAlignment TextAlignment = TextAlignment.Left);

    public static GraduationMarkSettings Empty { get; } = new GraduationMarkSettings();

    /// <summary>
    /// Interval between graduation marks. Defines the volume between graduationmarks.
    /// </summary>
    public double Interval { get; init; } = 1;

    /// <summary>
    /// Length of the mark.
    /// </summary>
    public double Length { get; init; } = 10;

    /// <summary>
    /// Height of the mark.
    /// </summary>
    public double Height { get; init; } = 1;

    /// <summary>
    /// Defines the level of indentation of the mark.
    /// </summary>
    public double Indentation { get; init; } = 0;

    /// <summary>
    /// Text template to print along the graduation marks. Can be used to print 'liters' or 'l' along with the content. 
    /// Available placeholders: 
    /// {0} - Volume.
    /// {1} - Volume unit abbreviation
    /// </summary>
    public string TextTemplate { get; init; } = "{0} {1}";

    /// <summary>
    /// Font settings to use for the text of the graduation mark.
    /// </summary>
    public FontSettings Font { get; init; } = new();

    internal void Validate()
    {
        if (Length <= 0)
            throw new ArgumentOutOfRangeException(nameof(Length), Length, "Value must be greater than 0;");
        
        if (Height <= 0)
            throw new ArgumentOutOfRangeException(nameof(Height), Height, "Value must be greater than 0");
    }
}
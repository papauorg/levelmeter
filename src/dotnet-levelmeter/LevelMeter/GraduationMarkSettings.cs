public record GraduationMarkSettings
{
    public static GraduationMarkSettings Empty { get; } = new GraduationMarkSettings();

    /// <summary>
    /// Interval between graduation marks. Defines the volume between graduationmarks.
    /// </summary>
    public double Interval { get; init; } = 1;

    /// <summary>
    /// Length of the mark.
    /// </summary>
    public double Length { get; init; } = 10d;

    /// <summary>
    /// Height of the mark.
    /// </summary>
    public double Height { get; init; } = 1;

    /// <summary>
    /// Defines the level of indentation of the mark.
    /// </summary>
    public double Indentation { get; init; } = 0;

    /// <summary>
    /// Text template to print along the graduation marks. Can be used to print 'liters' or 'l' along with the content. Available placeholders: {0} - Volume.
    /// </summary>
    public string TextTemplate { get; init; } = "{0}";
}

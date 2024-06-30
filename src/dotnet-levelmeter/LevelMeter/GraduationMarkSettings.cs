using CommandLine;

public record GraduationMarkSettings
{
    public static GraduationMarkSettings Empty { get; } = new GraduationMarkSettings();

    [Option('i', "interval", Default = 1, HelpText = "Interval between graduation marks. Defines the volume between graduationmarks.")]
    public double Interval { get; init; } = 1;

    [Option('l', "mark-length", Default = 10, HelpText = "Length of the mark in millimeters")]
    public double Length { get; init; } = 10d;

    [Option('h', "height", Default = 1, HelpText = "Height of the mark in millimeters.")]
    public double Height { get; init; } = 1;

    [Option("indentation", Default = 0, HelpText = "Defines the level of indentation of the mark in mm.")]
    public double Indentation { get; init; } = 0;

    [Option('t', "text-template", Default = "{0}", HelpText = "Text template to print along the graduation marks. Can be used to print 'liters' or 'l' along with the content. Available placeholders: {0} - Volume.")]
    public string TextTemplate { get; init; } = "{0}";
}

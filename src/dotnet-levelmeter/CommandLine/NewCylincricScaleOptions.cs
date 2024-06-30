using CommandLine;

using UnitsNet;

[Verb("new-cylindric-scale", HelpText = "Create a new svg scale for measuring liquid levels in a cylindric container.")]
public record NewCylincricScaleOptions
{

    [Option('d', "diameter", Required = true, HelpText = "Diameter of your container in millimeters.")]
    public double DiameterInMm { get; init; }

    [Option("min-volume", HelpText = "Min value for the volume scale to begin.")]
    public int MinVolume { get; init; } = 0;

    [Option('h', "height", Required = true, HelpText = "Maximum height of the scale to create.")]
    public double HeightInMm { get; init; }

    [Option('o', "output", HelpText = "Defines the file to output the svg to. Otherwise it's printed to stdout.")]
    public string OutputFile { get; init; } = string.Empty;
    
    [Option("max-volume", HelpText = "Max value for the volume on the scale. Can be used to skip volumes that would be possible for the container height.")]
    public int MaxVolume { get; init; } = 0;
}
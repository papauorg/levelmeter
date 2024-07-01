using CommandLine;

using UnitsNet;
using UnitsNet.Units;

[Verb("new-cylindric-scale", HelpText = "Create a new svg scale for measuring liquid levels in a cylindric container.")]
public record NewCylincricScaleOptions
{
    private VolumeUnit _volumeUnit = UnitsNet.Units.VolumeUnit.Liter;
    private LengthUnit _lengthUnit = UnitsNet.Units.LengthUnit.Millimeter;


    [Option('d', "diameter", Required = true, HelpText = "Diameter of your container in millimeters.")]
    public double Diameter { get; init; }

    [Option("min-volume", HelpText = "Min value for the volume scale to begin.")]
    public int MinVolume { get; init; } = 0;

    [Option('h', "height", Required = true, HelpText = "Maximum height of the scale to create.")]
    public double Height { get; init; }

    [Option('o', "output", HelpText = "Defines the file to output the svg to. Otherwise it's printed to stdout.")]
    public string Output { get; init; } = string.Empty;
    
    [Option("max-volume", HelpText = "Max value for the volume on the scale. Can be used to skip volumes that would be possible for the container height.")]
    public int MaxVolume { get; init; } = 0;

    [Option('c', "config", HelpText = "Path to configuration file (.json) that contains settings for graduation marks and the command line args of this help page.")]
    public string ConfigFile { get; init; } = string.Empty;

    [Option('l', "length-unit", HelpText = "Unit of length for the given measurements. (Default: mm)")]
    public string LengthUnit 
    { 
        get => Length.GetAbbreviation(_lengthUnit); 
        init => _lengthUnit = Length.ParseUnit(value); 
    }
    
    [Option('v', "volume-unit", Default = "l", HelpText = "Unit of volume for the given measurements and output on scale. (Default: l)")]
    public string VolumeUnit
    { 
        get => Volume.GetAbbreviation(_volumeUnit);
        init => _volumeUnit = Volume.ParseUnit(value);
    }

    internal Volume GetMinVolume() => Volume.From(MinVolume, _volumeUnit);
    internal Volume GetMaxVolume() => Volume.From(MaxVolume, _volumeUnit);
    internal Length GetDiameter() => Length.From(Diameter, _lengthUnit);
    internal Length GetHeight() => Length.From(Height, _lengthUnit);
    internal LengthUnit GetLengthUnit() => _lengthUnit;
    internal VolumeUnit GetVolumeUnit() => _volumeUnit;

    public GraduationMarkSettings[] GraduationMarkSettings { get; init; } = [ new GraduationMarkSettings() ];
}
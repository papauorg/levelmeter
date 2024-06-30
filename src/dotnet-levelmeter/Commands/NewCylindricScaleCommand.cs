using UnitsNet;
using UnitsNet.Units;

public class NewCylindricScaleCommand
{
    public NewCylincricScaleOptions Options { get; }

    public NewCylindricScaleCommand(NewCylincricScaleOptions options)
    {
        Options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<int> InvokeAsync(CancellationToken cancellationToken)
    {
        GraduationMarkSettings[] graduationMarkSettings = [
            new GraduationMarkSettings { Interval = 0.5d, Length = 5, Height = 1, TextTemplate = "" },
            new GraduationMarkSettings { TextTemplate = "{0}l", Height = 1.5 }
        ];

        var calculator = new CylindricGraduationMarkCalculator(graduationMarkSettings, LengthUnit.Millimeter, VolumeUnit.Liter);
        var graduationMarks = calculator.CalculateScale(
            Length.FromMillimeters(Options.DiameterInMm), 
            Length.FromMillimeters(Options.HeightInMm), 
            Volume.FromLiters(Options.MinVolume),
            Volume.FromLiters(Options.MaxVolume));

        Stream outputStream;
        if (string.IsNullOrWhiteSpace(Options.OutputFile))
        {
            outputStream = Console.OpenStandardOutput();
        }
        else
        {
            outputStream = new FileStream(Options.OutputFile, FileMode.Create, FileAccess.Write, FileShare.Read);
            outputStream.SetLength(0); // make sure to overwrite if already exists
        }

        var painter = new SvgScalePainter();
        await painter.PaintAsync(graduationMarks, outputStream, cancellationToken).ConfigureAwait(false);

        return 0;
    }
}
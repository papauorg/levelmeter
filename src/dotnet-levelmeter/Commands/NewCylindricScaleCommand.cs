using System.Diagnostics;

using Papau.Levelmeter.LevelMeter;

namespace Papau.Levelmeter.Commands;

public class NewCylindricScaleCommand
{
    public NewCylincricScaleOptions Options { get; }

    public NewCylindricScaleCommand(NewCylincricScaleOptions options)
    {
        Options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<int> InvokeAsync(CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var calculator = new CylindricGraduationMarkCalculator(Options.GraduationMarkSettings, Options.GetLengthUnit(), Options.GetVolumeUnit());
        var graduationMarks = calculator.CalculateScale(
            Options.GetDiameter(),
            Options.GetHeight(),
            Options.GetMinVolume(),
            Options.GetMaxVolume());

        var calculationTime = stopwatch.ElapsedMilliseconds;

        Stream outputStream;
        if (string.IsNullOrWhiteSpace(Options.Output))
        {
            outputStream = Console.OpenStandardOutput();
        }
        else
        {
            // Ensure target directory exists
            var targetDir = Path.GetDirectoryName(Options.Output);
            Directory.CreateDirectory(targetDir!);

            outputStream = new FileStream(Options.Output, FileMode.Create, FileAccess.Write, FileShare.Read);
            outputStream.SetLength(0); // make sure to overwrite if already exists
        }

        var openStream = stopwatch.ElapsedMilliseconds;

        var painter = new SvgScalePainter();
        await painter.PaintAsync(graduationMarks, outputStream, cancellationToken).ConfigureAwait(false);

        var painted = stopwatch.ElapsedMilliseconds;
        await Console.Error.WriteLineAsync($"Finished! (Calculation: {calculationTime}, Stream: {openStream}, Paint: {painted})").ConfigureAwait(false);

        return 0;
    }
}
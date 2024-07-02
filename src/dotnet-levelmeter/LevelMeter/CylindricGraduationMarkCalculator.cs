using System.Drawing;
using System.Runtime.CompilerServices;

using UnitsNet;
using UnitsNet.Units;

namespace Papau.Levelmeter.LevelMeter;

public class CylindricGraduationMarkCalculator
{

    public GraduationMarkSettings[] GraduationMarkSettings { get; }
    public LengthUnit LengthUnit { get; }
    public VolumeUnit VolumeUnit { get; }


    public CylindricGraduationMarkCalculator(GraduationMarkSettings[] graduationMarkSettings, LengthUnit lengthUnit, VolumeUnit volumeUnit)
    {
        GraduationMarkSettings = graduationMarkSettings ?? throw new ArgumentNullException(nameof(graduationMarkSettings));
        LengthUnit = lengthUnit;
        VolumeUnit = volumeUnit;
    }

    public GraduationMark[] CalculateScale(Length diameter, Length height, Volume minVolume, Volume maxVolume)
    {
        var marksByVolume = new Dictionary<Volume, GraduationMark>();

        foreach (var setting in GraduationMarkSettings)
            CalculateGraduationMark(marksByVolume, setting, diameter, height, minVolume, maxVolume);

        return NormalizePositions(marksByVolume);
    }

    private static GraduationMark[] NormalizePositions(Dictionary<Volume, GraduationMark> marksByVolume)
    {
        var maxPosition = marksByVolume.Values.Min(m => m.Position.Y) * -1;
        return marksByVolume.Values
            .Select(v => v with { Position = (v.Position.X, v.Position.Y + maxPosition) })
            .OrderBy(v => v.Position.Y)
            .ToArray();
    }


    private void CalculateGraduationMark(Dictionary<Volume, GraduationMark> results, GraduationMarkSettings setting, Length diameter, Length height, Volume minVolume, Volume maxVolume)
    {
        maxVolume = GetMaximumScaleVolume(diameter, height, maxVolume);

        var volInterval = Volume.From(setting.Interval, VolumeUnit);
        for (var currentVolume = Volume.From(0, VolumeUnit); currentVolume <= maxVolume; currentVolume += volInterval)
        {
            if (currentVolume < minVolume)
                continue;

            if (currentVolume.Value % volInterval.Value != 0)
                continue;

            var currentHeight = GetHeightByVolume(currentVolume, diameter);

            if (currentHeight > height)
                break;

            var mark = new GraduationMark
            {
                Height = Length.From(setting.Height, LengthUnit),
                Length = Length.From(setting.Length, LengthUnit),
                Position = (Length.FromMillimeters(setting.Indentation), currentHeight * -1),
                Text = PrepareText(setting, currentVolume, maxVolume),
                Volume = currentVolume,
                Font = setting.Font
            };

            results[currentVolume] = mark;
        }
    }

    private static string PrepareText(GraduationMarkSettings setting, Volume currentVolume, Volume maxVolume)
    {
        var volumeValue = currentVolume.Value;
        var abbreviation = Volume.GetAbbreviation(currentVolume.Unit);

        return string.Format(setting.TextTemplate, volumeValue, abbreviation);
    }


    private Volume GetMaximumScaleVolume(Length diameter, Length height, Volume maxVolume)
    {
        var maximumVolumeByHeight = Volume.FromCubicMillimeters(Math.PI * Math.Pow(diameter.Millimeters / 2, 2) * height.Millimeters);
        if (maxVolume.Value == 0)
            maxVolume = maximumVolumeByHeight;
        else
            maxVolume = maximumVolumeByHeight > maxVolume ? maxVolume : maximumVolumeByHeight;

        return maxVolume.ToUnit(VolumeUnit);
    }


    private Length GetHeightByVolume(Volume currentVolume, Length diameter)
        => Length.FromMillimeters(currentVolume.CubicMillimeters / (Math.PI * Math.Pow(diameter.Millimeters / 2, 2))).ToUnit(LengthUnit);

}
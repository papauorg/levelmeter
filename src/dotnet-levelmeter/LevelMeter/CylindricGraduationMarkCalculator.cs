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

            var mark = new GraduationMark
            {
                Height = Length.From(setting.Height, LengthUnit),
                Length = Length.From(setting.Length, LengthUnit),
                Position = (Length.From(setting.Indentation, LengthUnit), currentHeight * -1),
                Text = PrepareText(setting, currentVolume, maxVolume),
                Volume = currentVolume,
                Font = setting.Font,
                ReferenceSetting = setting
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

        // use volume by height if no max is defined
        if (maxVolume.Value == 0)
            return maximumVolumeByHeight.ToUnit(VolumeUnit);

        // if volume by height is within 1% tolerance of the max volume,
        // use the maxVolume for even end of the scale
        if (maxVolume.Equals(maximumVolumeByHeight, maxVolume / 100))
            return maxVolume.ToUnit(VolumeUnit);

        // limit to max volume if the volume by height would be more
        if (maxVolume < maximumVolumeByHeight)
            return maxVolume.ToUnit(VolumeUnit);

        // if the volume by height is less than the maximum
        // use it.
        return maximumVolumeByHeight.ToUnit(VolumeUnit);
    }


    private Length GetHeightByVolume(Volume currentVolume, Length diameter)
        => Length.FromMillimeters(currentVolume.CubicMillimeters / (Math.PI * Math.Pow(diameter.Millimeters / 2, 2))).ToUnit(LengthUnit);

}
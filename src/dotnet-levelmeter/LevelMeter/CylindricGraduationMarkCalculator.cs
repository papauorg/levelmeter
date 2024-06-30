using System.Drawing;

using UnitsNet;
using UnitsNet.Units;

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

        return marksByVolume.Values.OrderByDescending(m => m.Volume).ToArray();
    }

    private void CalculateGraduationMark(Dictionary<Volume, GraduationMark> results, GraduationMarkSettings setting, Length diameter, Length height, Volume minVolume, Volume maxVolume)
    {
        maxVolume = GetMaximumScaleVolume(diameter, height, maxVolume);

        var currentVolume = minVolume.ToUnit(VolumeUnit);
        var currentHeight = GetHeightByVolume(currentVolume, diameter);

        while (currentVolume <= maxVolume)
        {
            var mark = new GraduationMark
            {
                Height = Length.From(setting.Height, LengthUnit),
                Length = Length.From(setting.Length, LengthUnit),
                Position = (Length.FromMillimeters(setting.Indentation), currentHeight),
                Text = string.Format(setting.TextTemplate, currentVolume.Value),
                Volume = currentVolume
            };

            results[currentVolume] = mark;

            currentVolume += Volume.From(setting.Interval, VolumeUnit);
            currentHeight = GetHeightByVolume(currentVolume, diameter);
        }
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
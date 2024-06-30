using FluentAssertions;

using UnitsNet;
using UnitsNet.Units;

public class CylindricGraduationMarkCalculatorTests
{

    public class CalculateScaleMethod : CylindricGraduationMarkCalculatorTests
    {
        private readonly CylindricGraduationMarkCalculator _metricCalculator;

        public CalculateScaleMethod()
        {
            var settings = new GraduationMarkSettings
            {
                Interval = 1, // 1 liter marks
                TextTemplate = "{0} l",
            };

            _metricCalculator = new CylindricGraduationMarkCalculator([settings], LengthUnit.Millimeter, VolumeUnit.Liter);
        }

        [Fact]
        public void CalculatesDistanceBetweenMarksCorrectly()
        {
            var result = _metricCalculator.CalculateScale(Length.FromMillimeters(360), Length.FromMillimeters(10), Volume.Zero, Volume.Zero);

            result.Should().HaveCount(2);
            result.Last().Position.Y.Millimeters.Should().Be(0);
            result.First().Position.Y.Millimeters.Should().BeApproximately(9.82, 0.01d);
        }

        [Fact]
        public void BeginsScaleAtMinimumVolume()
        {
            var result = _metricCalculator.CalculateScale(Length.FromMillimeters(360), Length.FromCentimeters(6), Volume.FromLiters(5), Volume.Zero);

            result.Should().HaveCount(2);
            result.Min(r => r.Volume).Should().Be(Volume.FromLiters(5));
        }

        [Fact]
        public void DoesNotExeedMaximumVolume()
        {
            var result = _metricCalculator.CalculateScale(Length.FromMillimeters(360), Length.FromCentimeters(6), Volume.Zero, Volume.FromLiters(2));

            result.Max(r => r.Volume).Should().BeLessThanOrEqualTo(Volume.FromLiters(2));
        }

        [Fact]
        public void ProducesCorrectScaleForSampleContainer()
        {
            var result = _metricCalculator.CalculateScale(Length.FromMillimeters(360), Length.FromCentimeters(33.5), Volume.Zero, Volume.Zero);

            result.First().Position.Y.Millimeters.Should().BeApproximately(335, 1);
            result.Last().Position.Y.Millimeters.Should().Be(0);
            result.Should().HaveCount(35);
        }
    }

    public class CalculateScaleMethodWithMultipleGraduationMarkSettings : CylindricGraduationMarkCalculatorTests
    {
        private readonly CylindricGraduationMarkCalculator _metricCalculator;

        public CalculateScaleMethodWithMultipleGraduationMarkSettings()
        {
            var settings = new GraduationMarkSettings
            {
                Interval = 1, // 1 liter marks
                TextTemplate = "{0} l",
            };

            var halfLiterSettings = new GraduationMarkSettings
            {
                Interval = 0.5d, // 1/2 liter marks
                TextTemplate = "",
                Length = 5,
                Height = 0.8
            };

            _metricCalculator = new CylindricGraduationMarkCalculator([halfLiterSettings, settings], LengthUnit.Millimeter, VolumeUnit.Liter);
        }

        [Fact]
        public void ProducesCorrectScaleForSampleContainer()
        {
            var result = _metricCalculator.CalculateScale(Length.FromMillimeters(360), Length.FromCentimeters(33.5), Volume.Zero, Volume.Zero);

            result.First().Position.Y.Millimeters.Should().BeApproximately(335, 1);
            result.Last().Position.Y.Millimeters.Should().Be(0);
            result.Should().HaveCount(69);
        }
    }
}

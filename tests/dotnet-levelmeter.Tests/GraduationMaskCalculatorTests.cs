using FluentAssertions;

using Papau.Levelmeter.LevelMeter;

using UnitsNet;
using UnitsNet.Units;

namespace Papau.Levelmeter.Tests;

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
            result.First().Position.Y.Millimeters.Should().Be(0);
            result.Last().Position.Y.Millimeters.Should().BeApproximately(9.82, 0.01d);
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

            result.Last().Position.Y.Millimeters.Should().BeApproximately(335, 1);
            result.First().Position.Y.Millimeters.Should().Be(0);
            result.Should().HaveCount(35);
        }
     }

    public class CalculateScaleMethodIrregularStart : CylindricGraduationMarkCalculatorTests
    {
        private readonly GraduationMark[] _result;
        private const int INTERVAL = 1;
        public CalculateScaleMethodIrregularStart()
        {
            var settings = new GraduationMarkSettings
            {
                Interval = INTERVAL, // 1 liter marks
                TextTemplate = "{0} l",
            };

            var metricCalculator = new CylindricGraduationMarkCalculator([settings], LengthUnit.Millimeter, VolumeUnit.Liter);
            
            _result = metricCalculator.CalculateScale(Length.FromMillimeters(360), Length.FromCentimeters(33.5), Volume.FromMilliliters(1), Volume.FromLiters(10));
        }

        [Fact]
        public void DoesNotContainVolumesThatAreNotDevidableByTheInterval()
        {
            _result.Should().NotContain(m => (m.Volume.Value % INTERVAL) != 0);
        }

        [Fact]
        public void ContainsExactlyTheMaximumVolumeAsAnGraduationMark()
        {
            _result.Select(m => m.Volume).Max().Should().Be(Volume.FromLiters(10));
        }

        [Fact]
        public void StartsWithTheNextDevidableVolumeAmount()
        {
            _result.Select(m => m.Volume).Min().Should().Be(Volume.FromLiters(1)); // not 0.1 and not 0
        }
    }

    public class CalculateScaleMethodWithMultipleGraduationMarkSettings : CylindricGraduationMarkCalculatorTests
    {
        private readonly CylindricGraduationMarkCalculator _metricCalculator;
        private readonly GraduationMark[] _result;


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
            
            _result = _metricCalculator.CalculateScale(Length.FromMillimeters(360), Length.FromCentimeters(33.5), Volume.Zero, Volume.Zero);
        }

        [Fact]
        public void ProducesCorrectScaleForSampleContainer()
        {
            _result.Last().Position.Y.Millimeters.Should().BeApproximately(335, 1);
            _result.First().Position.Y.Millimeters.Should().Be(0);
            _result.Should().HaveCount(69);
        }

        [Fact]
        public void GraduationMarksAreEquallySpaced()
        {
            var spacingPerLiterInMm = 9.82d;
            var spacingPerHalfLiterInMm = spacingPerLiterInMm / 2;

            for (var i = 1; i < _result.Length; ++i)
            {
                var spacing = _result[i].Position.Y - _result[i - 1].Position.Y;
                spacing.Millimeters.Should().BeApproximately(spacingPerHalfLiterInMm, 0.1, $"because the item {i} should be half a liter away from its previous item.");
            }
        }
    }

    public class CalculateScaleMethodImperial : CylindricGraduationMarkCalculatorTests
    {
        private readonly CylindricGraduationMarkCalculator _imperialCalculator;
        private readonly GraduationMark[] _result;


        public CalculateScaleMethodImperial()
        {
            var settings = new GraduationMarkSettings
            {
                Interval = 1, // 1 galon marks
                TextTemplate = "{0} gal",
            };

            _imperialCalculator = new CylindricGraduationMarkCalculator([settings], LengthUnit.Inch, VolumeUnit.UsGallon);
            
            _result = _imperialCalculator.CalculateScale(Length.FromMillimeters(360).ToUnit(LengthUnit.Inch), Length.FromCentimeters(33.5).ToUnit(LengthUnit.Inch), Volume.Zero, Volume.Zero);
        }

        [Fact]
        public void ReturnsCorrectAmountOfGallons()
        {
            _result.Should().HaveCount(10);
            _result.First().Volume.Should().Be(Volume.FromUsGallons(9));
            _result.Last().Volume.Should().Be(Volume.FromUsGallons(0));
        }
    }
}

using System;
using System.Collections.Generic;
using BandwidthCalculator.Models;

namespace BandwidthCalculator.Services
{
    /// <summary>
    /// Конвертер единиц измерения пропускной способности
    /// </summary>
    public class UnitConverter
    {
        // Константы для конвертации
        private const double BitsInByte = 8.0;
        private const double Kilo = 1000.0;
        private const double Mega = 1000000.0;
        private const double Giga = 1000000000.0;
        private const double Tera = 1000000000000.0;
        private const double Peta = 1000000000000000.0;

        /// <summary>
        /// Конвертирует значение из одной единицы измерения во все остальные
        /// </summary>
        public Dictionary<SpeedUnit, double> Convert(double value, SpeedUnit fromUnit)
        {
            double valueInBitsPerSecond = ConvertToBitsPerSecond(value, fromUnit);

            return new Dictionary<SpeedUnit, double>
            {
                // Bits per second
                [SpeedUnit.BitPerSecond] = valueInBitsPerSecond,
                [SpeedUnit.KilobitPerSecond] = valueInBitsPerSecond / Kilo,
                [SpeedUnit.MegabitPerSecond] = valueInBitsPerSecond / Mega,
                [SpeedUnit.GigabitPerSecond] = valueInBitsPerSecond / Giga,
                [SpeedUnit.TerabitPerSecond] = valueInBitsPerSecond / Tera,
                [SpeedUnit.PetabitPerSecond] = valueInBitsPerSecond / Peta,

                // Bytes per second
                [SpeedUnit.BytePerSecond] = valueInBitsPerSecond / BitsInByte,
                [SpeedUnit.KilobytePerSecond] = valueInBitsPerSecond / BitsInByte / Kilo,
                [SpeedUnit.MegabytePerSecond] = valueInBitsPerSecond / BitsInByte / Mega,
                [SpeedUnit.GigabytePerSecond] = valueInBitsPerSecond / BitsInByte / Giga,
                [SpeedUnit.TerabytePerSecond] = valueInBitsPerSecond / BitsInByte / Tera,
                [SpeedUnit.PetabytePerSecond] = valueInBitsPerSecond / BitsInByte / Peta
            };
        }

        /// <summary>
        /// Конвертирует значение в биты в секунду
        /// </summary>
        private double ConvertToBitsPerSecond(double value, SpeedUnit unit)
        {
            return unit switch
            {
                // Bits per second
                SpeedUnit.BitPerSecond => value,
                SpeedUnit.KilobitPerSecond => value * Kilo,
                SpeedUnit.MegabitPerSecond => value * Mega,
                SpeedUnit.GigabitPerSecond => value * Giga,
                SpeedUnit.TerabitPerSecond => value * Tera,
                SpeedUnit.PetabitPerSecond => value * Peta,

                // Bytes per second
                SpeedUnit.BytePerSecond => value * BitsInByte,
                SpeedUnit.KilobytePerSecond => value * Kilo * BitsInByte,
                SpeedUnit.MegabytePerSecond => value * Mega * BitsInByte,
                SpeedUnit.GigabytePerSecond => value * Giga * BitsInByte,
                SpeedUnit.TerabytePerSecond => value * Tera * BitsInByte,
                SpeedUnit.PetabytePerSecond => value * Peta * BitsInByte,

                _ => throw new ArgumentException($"Неизвестная единица измерения: {unit}")
            };
        }

        /// <summary>
        /// Конвертирует значение с форматированием результата
        /// </summary>
        public Dictionary<SpeedUnit, string> ConvertFormatted(
            double value,
            SpeedUnit fromUnit,
            int decimalPlaces = 1)
        {
            var rawResults = Convert(value, fromUnit);
            var formattedResults = new Dictionary<SpeedUnit, string>();

            foreach (var kvp in rawResults)
            {
                formattedResults[kvp.Key] = InputValidator.FormatNumber(kvp.Value, decimalPlaces);
            }

            return formattedResults;
        }
    }
}
using System;
using System.Reflection;

namespace BandwidthCalculator.Models
{
    public static class UnitHelper
    {
        public static string GetDisplayName(SpeedUnit unit)
        {
            var field = unit.GetType().GetField(unit.ToString());
            if (field == null) return unit.ToString();

            var attribute = field.GetCustomAttribute<UnitInfoAttribute>();
            return attribute?.DisplayName ?? unit.ToString();
        }

        public static bool AllowsFractional(SpeedUnit unit)
        {
            var field = unit.GetType().GetField(unit.ToString());
            if (field == null) return true;

            var attribute = field.GetCustomAttribute<UnitInfoAttribute>();
            return attribute?.AllowsFractional ?? true;
        }

        public static SpeedUnit[] BitsUnits => new SpeedUnit[]
        {
            SpeedUnit.BitPerSecond,
            SpeedUnit.KilobitPerSecond,
            SpeedUnit.MegabitPerSecond,
            SpeedUnit.GigabitPerSecond,
            SpeedUnit.TerabitPerSecond,
            SpeedUnit.PetabitPerSecond
        };

        public static SpeedUnit[] BytesUnits => new SpeedUnit[]
        {
            SpeedUnit.BytePerSecond,
            SpeedUnit.KilobytePerSecond,
            SpeedUnit.MegabytePerSecond,
            SpeedUnit.GigabytePerSecond,
            SpeedUnit.TerabytePerSecond,
            SpeedUnit.PetabytePerSecond
        };
    }
}
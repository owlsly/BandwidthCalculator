using System;

namespace BandwidthCalculator.Models
{
    public enum SpeedUnit
    {
        // Bits per second
        [UnitInfo("bit/s", false)]  // bits cannot be fractional
        BitPerSecond,

        [UnitInfo("Kbit/s", true)]
        KilobitPerSecond,

        [UnitInfo("Mbit/s", true)]
        MegabitPerSecond,

        [UnitInfo("Gbit/s", true)]
        GigabitPerSecond,

        [UnitInfo("Tbit/s", true)]
        TerabitPerSecond,

        [UnitInfo("Pbit/s", true)]
        PetabitPerSecond,

        // Bytes per second
        [UnitInfo("B/s", true)]  // bytes can be fractional
        BytePerSecond,

        [UnitInfo("KB/s", true)]
        KilobytePerSecond,

        [UnitInfo("MB/s", true)]
        MegabytePerSecond,

        [UnitInfo("GB/s", true)]
        GigabytePerSecond,

        [UnitInfo("TB/s", true)]
        TerabytePerSecond,

        [UnitInfo("PB/s", true)]
        PetabytePerSecond
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class UnitInfoAttribute : Attribute
    {
        public string DisplayName { get; }
        public bool AllowsFractional { get; }

        public UnitInfoAttribute(string displayName, bool allowsFractional)
        {
            DisplayName = displayName;
            AllowsFractional = allowsFractional;
        }
    }
}
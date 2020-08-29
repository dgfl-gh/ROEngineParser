namespace ROEngineParser
{
    public class ReliabilityData
    {
        public int RatedBurnTime { get; set; }
        public float IgnitionReliabilityStart { get; set; }
        public float IgnitionReliabilityEnd { get; set; }
        public float CycleReliabilityStart { get; set; }
        public float CycleReliabilityEnd { get; set; }

        public ReliabilityData()
        {
            RatedBurnTime = -1;

            IgnitionReliabilityStart = 1;

            IgnitionReliabilityEnd = 1;

            CycleReliabilityStart = 1;

            CycleReliabilityEnd = 1;
        }

        public ReliabilityData(ConfigBlock cfg)
        {
            RatedBurnTime = cfg.GetFieldValue("ratedBurnTime").ParseOrDefaultInt(-1);

            IgnitionReliabilityStart = cfg.GetFieldValue("ignitionReliabilityStart").ParseOrDefaultFloat(1);

            IgnitionReliabilityEnd = cfg.GetFieldValue("ignitionReliabilityEnd").ParseOrDefaultFloat(1);

            CycleReliabilityStart = cfg.GetFieldValue("cycleReliabilityStart").ParseOrDefaultFloat(1);

            CycleReliabilityEnd = cfg.GetFieldValue("cycleReliabilityEnd").ParseOrDefaultFloat(1);
        }
    }
}

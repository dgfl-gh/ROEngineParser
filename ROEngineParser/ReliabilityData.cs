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
            RatedBurnTime = 0;

            IgnitionReliabilityStart = 0;

            IgnitionReliabilityEnd = 0;

            CycleReliabilityStart = 0;

            CycleReliabilityEnd = 0;
        }

        public ReliabilityData(ConfigBlock cfg)
        {
            RatedBurnTime = cfg.GetFieldValue("ratedBurnTime").ParseInt();

            IgnitionReliabilityStart = cfg.GetFieldValue("ignitionReliabilityStart").ParseFloat();

            IgnitionReliabilityEnd = cfg.GetFieldValue("ignitionReliabilityEnd").ParseFloat();

            CycleReliabilityStart = cfg.GetFieldValue("cycleReliabilityStart").ParseFloat();

            CycleReliabilityEnd = cfg.GetFieldValue("cycleReliabilityEnd").ParseFloat();
        }
    }
}

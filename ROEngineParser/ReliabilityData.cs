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
            RatedBurnTime = int.Parse(cfg.GetFieldValue("ratedBurnTime"));

            IgnitionReliabilityStart = float.Parse(cfg.GetFieldValue("ignitionReliabilityStart"));

            IgnitionReliabilityEnd = float.Parse(cfg.GetFieldValue("ignitionReliabilityEnd"));

            CycleReliabilityStart = float.Parse(cfg.GetFieldValue("cycleReliabilityStart"));

            CycleReliabilityEnd = float.Parse(cfg.GetFieldValue("cycleReliabilityEnd"));
        }
    }
}

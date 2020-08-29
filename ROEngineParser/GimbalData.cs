namespace ROEngineParser
{
    public class GimbalData
    {
        public bool IsGimbaled { get; set; }
        public float Range { get; set; }

        public GimbalData()
        {
            Range = 0;
            IsGimbaled = false;
        }

        public GimbalData(ConfigBlock block)
        {
            if (block.name != "ModuleGimbal")
            {
                IsGimbaled = false;
                Range = 0;
            }

            Range = block.GetFieldValue("gimbalRange").ParseOrDefaultFloat(defVal: 0);

            if (Range > 0)
                IsGimbaled = true;

            foreach (var line in block.content)
            {
                if (line.Length == 2)
                {
                    string field = line[0];
                    string value = line[1];

                    field = field.RemoveOperator();

                    if (field.Contains("gimbalRange"))
                        Range = value.ParseOrDefaultFloat(defVal: 0);

                    if (Range > 0)
                        IsGimbaled = true;
                }
            }
        }
    }
}

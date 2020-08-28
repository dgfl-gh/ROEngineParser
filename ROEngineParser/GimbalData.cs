
namespace ROEngineParser
{
    public class GimbalData
    {
        public bool IsGimbaled { get; set; } = false;
        public float Range { get; set; } = 0;

        public GimbalData(ConfigBlock block)
        {
            if (block.name != "ModuleGimbal")
            {
                IsGimbaled = false;
                Range = 0;
            }

            Range = block.GetFieldValue("gimbalRange").ParseFloat(defVal: 0);

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
                        Range = value.ParseFloat(defVal: 0);

                    if (Range > 0)
                        IsGimbaled = true;
                }
            }
        }
    }
}

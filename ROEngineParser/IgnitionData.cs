using System.Collections.Generic;

namespace ROEngineParser
{
    public class IgnitionData
    {
        public int number;
        public Dictionary<PropellantData, float> resources = new Dictionary<PropellantData, float>();
    }
}

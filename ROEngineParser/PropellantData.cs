using System.Collections.Generic;

namespace ROEngineParser
{
    public class PropellantData
    {
        public string Name { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public double Density { get; set; } = 0;
        public double UnitCost { get; set; } = 0;

        private readonly static Dictionary<string, PropellantData> propellantsDict = new Dictionary<string, PropellantData>();

        public static PropellantData CreateProp(string name, string displayName = null, double? density = null, double? unitCost = null)
        {
            if (propellantsDict.ContainsKey(name))
            {
                if (displayName != null)
                    propellantsDict[name].DisplayName = displayName;
                if (density != null)
                    propellantsDict[name].Density = (double)density;
                if (unitCost != null)
                    propellantsDict[name].UnitCost = (double)unitCost;

                return propellantsDict[name];
            }

            if (displayName != null && density == null && unitCost == null)
                return new PropellantData(name, displayName);

            else if (displayName != null && density != null && unitCost == null)
                return new PropellantData(name, displayName, (double)density);

            else if (displayName != null && density != null && unitCost != null)
                return new PropellantData(name, displayName, (double)density, (double)unitCost);

            else
                return new PropellantData(name);
        }

        private PropellantData(string name)
        {
            if (propellantsDict.ContainsKey(name))
                return;

            Name = name;
            propellantsDict.Add(name, this);
        }

        private PropellantData(string name, string displayName)
        {
            if (propellantsDict.ContainsKey(name))
                return;

            Name = name;
            DisplayName = displayName;
            propellantsDict.Add(name, this);
        }

        private PropellantData(string name, string displayName, double density)
        {
            if (propellantsDict.ContainsKey(name))
                return;

            Name = name;
            DisplayName = displayName;
            Density = density;
            propellantsDict.Add(name, this);
        }

        private PropellantData(string name, string displayName, double density, double unitCost)
        {
            if (propellantsDict.ContainsKey(name))
                return;

            Name = name;
            DisplayName = displayName;
            Density = density;
            UnitCost = unitCost;
            propellantsDict.Add(name, this);
        }
    }
}

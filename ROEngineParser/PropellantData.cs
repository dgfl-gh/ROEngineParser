using System.Collections.Generic;

namespace ROEngineParser
{
    //TODO: Make this class store a reference to resources
    public class PropellantData
    {
        public string Name { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public float Density { get; set; } = 0;
        public float UnitCost { get; set; } = 0;

        private readonly static Dictionary<string, PropellantData> propellantsDict = new Dictionary<string, PropellantData>();

        public static PropellantData CreateProp(string name, string displayName = null, float? density = null, float? unitCost = null)
        {
            if (propellantsDict.ContainsKey(name))
            {
                if (displayName != null)
                    propellantsDict[name].DisplayName = displayName;
                if (density != null)
                    propellantsDict[name].Density = (float)density;
                if (unitCost != null)
                    propellantsDict[name].UnitCost = (float)unitCost;

                return propellantsDict[name];
            }

            if (displayName != null && density == null && unitCost == null)
                return new PropellantData(name, displayName);

            else if (displayName != null && density != null && unitCost == null)
                return new PropellantData(name, displayName, (float)density);

            else if (displayName != null && density != null && unitCost != null)
                return new PropellantData(name, displayName, (float)density, (float)unitCost);

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

        private PropellantData(string name, string displayName, float density)
        {
            if (propellantsDict.ContainsKey(name))
                return;

            Name = name;
            DisplayName = displayName;
            Density = density;
            propellantsDict.Add(name, this);
        }

        private PropellantData(string name, string displayName, float density, float unitCost)
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

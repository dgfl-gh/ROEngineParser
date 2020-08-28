using System.Collections.Generic;
using System.Threading;

namespace ROEngineParser
{
    public class EngineConfigData
    {
        public EngineData parentEngine;
        public string ConfigName { get; set; }
        public string ConfigDescription { get; set; }
        public float MaxThrust { get; set; }
        public float MinThrust { get; set; }
        public float MassMult { get; set; }
        public bool Ullage { get; set; }
        public bool PressureFed { get; set; }
        public float IspVacuum { get => ispData.IspVacuum; set => ispData.IspVacuum = value; }
        public float IspSeaLevel { get => ispData.IspSeaLevel; set => ispData.IspSeaLevel = value; }
        public float MinThrottle { get => MinThrust / MaxThrust; }
        public bool AirLightable { get => Ignition.number <= 0 && parentEngine.LiteralZeroIgnitions; }
        public IgnitionData Ignition { get; set; } = new IgnitionData();
        public ReliabilityData Reliability { get; set; } = new ReliabilityData();

        public Dictionary<PropellantData, double> Propellants = new Dictionary<PropellantData, double>();

        private IspData ispData = new IspData();

        public EngineConfigData(EngineData parent, ConfigBlock config)
        {
            if (config == null || config.content.Count == 0)
                return;

            parentEngine = parent;
            ConfigName = config.name;

            foreach (var line in config.content)
            {
                if (line.Length == 2)
                {
                    string field = line[0];
                    string value = line[1];

                    field = field.RemoveOperator();

                    switch (field)
                    {
                        case "description":
                            ConfigDescription = value;
                            break;
                        case "minThrust":
                            MinThrust = float.Parse(value);
                            break;
                        case "maxThrust":
                            MaxThrust = float.Parse(value);
                            break;
                        case "massMult":
                            MassMult = float.Parse(value);
                            break;
                        case "ullage":
                            Ullage = bool.Parse(value);
                            break;
                        case "pressureFed":
                            PressureFed = bool.Parse(value);
                            break;
                        case "ignitions":
                            Ignition.number = int.Parse(value);
                            break;
                    }
                }
            }

            foreach (var child in config.childrenBlocks)
            {
                if (child.type == BlockType.AtmosphereCurve)
                {
                    ispData = new IspData(child);
                }
                else if (child.type == BlockType.IgnitorResource)
                {
                    var resource = PropellantData.CreateProp(child.name);
                    var amount = float.Parse(child.GetFieldValue("amount"));

                    Ignition.resources[resource] = amount;
                }
                else if (child.type == BlockType.Propellant)
                {
                    var resource = PropellantData.CreateProp(child.name);
                    var ratio = float.Parse(child.GetFieldValue("ratio"));

                    Propellants[resource] = ratio;
                }
            }
        }

        public EngineConfigData(EngineData parent)
        {
            parentEngine = parent;
        }
    }
}

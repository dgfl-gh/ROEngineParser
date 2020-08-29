using Newtonsoft.Json;
using System.Collections.Generic;

namespace ROEngineParser
{
    public class EngineConfigData
    {
        [JsonIgnore]
        public EngineData parentEngine;
        public string ConfigName { get; set; }
        public string ConfigDescription { get; set; } = "";
        public float MaxThrust { get; set; }
        public float MinThrust { get; set; }
        public float MassMult { get; set; }
        public bool Ullage { get; set; }
        public bool PressureFed { get; set; }
        public float IspVacuum { get => ispData.IspVacuum; set => ispData.IspVacuum = value; }
        public float IspSeaLevel { get => ispData.IspSeaLevel; set => ispData.IspSeaLevel = value; }
        public float MinThrottle { get => MinThrust / MaxThrust; }
        public bool AirLightable { get => !(Ignition.number == 0 && (parentEngine?.LiteralZeroIgnitions ?? false)); }
        [JsonProperty(Order = 8)]
        public IgnitionData Ignition { get; set; } = new IgnitionData();
        [JsonProperty(Order = 9)]
        public Dictionary<string, float> Propellants = new Dictionary<string, float>();
        [JsonProperty(Order = 10)]
        public ReliabilityData Reliability { get; set; } = new ReliabilityData();

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
                            MinThrust = value.ParseOrDefaultFloat(defVal: 0);
                            break;
                        case "maxThrust":
                            MaxThrust = value.ParseOrDefaultFloat(defVal: 0);
                            break;
                        case "massMult":
                            MassMult = value.ParseOrDefaultFloat(defVal: 0);
                            break;
                        case "ullage":
                            Ullage = value.ParseOrDefaultBool(defVal: true);
                            break;
                        case "pressureFed":
                            PressureFed = value.ParseOrDefaultBool(defVal: false);
                            break;
                        case "ignitions":
                            Ignition.number = value.ParseOrDefaultInt(defVal: 0);
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
                    var resource = child.name;
                    var amount = child.GetFieldValue("amount").ParseOrDefaultFloat();

                    if(resource != null)
                        Ignition.resources[resource] = amount;
                }
                else if (child.type == BlockType.Propellant)
                {
                    var resource = child.name;
                    var ratio = child.GetFieldValue("ratio").ParseOrDefaultFloat();

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

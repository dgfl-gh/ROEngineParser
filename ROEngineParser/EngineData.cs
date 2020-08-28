using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ROEngineParser
{
    public enum EngineType
    {
        Liquid,
        Solid,
        Electric,
        Unknown
    }

    public class EngineData
    {
        public string Title { get; set; }
        public string Manufacturer { get; set; }
        public string Description { get; set; }
        public float OriginalMass { get; set; }
        public bool LiteralZeroIgnitions { get; set; }
        public EngineType EngineType { get; set; }
        public string DefaultConfig { get; set; }
        public GimbalData Gimbal { get; set; }
        public Dictionary<string, EngineConfigData> EngineConfigs = new Dictionary<string, EngineConfigData>();

        public EngineData(List<string[]> cfg)
        {
            if (cfg == null || cfg.Count == 0)
                return;

            FetchEngineData(cfg);
        }

        public string ToJson()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            return JsonSerializer.Serialize(this, options);
        }

        public void FromJson(string json)
        {

        }

        private void FetchEngineData(List<string[]> cfg)
        {
            for (int i = 0; i < cfg.Count; i++)
            {
                string[] line = cfg[i];
                if (line.Length == 2)
                {
                    string field = line[0];
                    string value = line[1];

                    field = field.RemoveOperator();

                    switch (field)
                    {
                        case "title":
                            Title = value;
                            break;
                        case "manufacturer":
                            Manufacturer = value;
                            break;
                        case "description": //TODO: field is duplicate in the engine configs, so find a way to distinguish
                            if (string.IsNullOrEmpty(Description))
                                Description = value;
                            break;
                    }
                }
                else if (line[0].Contains("MODULE") || line[0].Contains("TESTFLIGHT"))
                {
                    var block = ConfigBlock.ExtractBlock(cfg, line[0], ref i, true);

                    GetDataFromBlock(block);
                }
            }
        }

        private void GetDataFromBlock(ConfigBlock block)
        {
            switch (block.type)
            {
                case BlockType.EngineType:
                    EngineType = GetType(block);
                    break;
                case BlockType.ModuleGimbal:
                    Gimbal = new GimbalData(block);
                    break;
                case BlockType.ModuleEngineConfigs:
                    OriginalMass = float.Parse(block.GetFieldValue("origMass"));
                    LiteralZeroIgnitions = bool.Parse(block.GetFieldValue("literalZeroIgnitions"));
                    DefaultConfig = block.GetFieldValue("configuration");

                    foreach (var config in block.childrenBlocks ?? Enumerable.Empty<ConfigBlock>())
                    {
                        if (config.type == BlockType.EngineConfig)
                            EngineConfigs[config.name] = new EngineConfigData(this, config);
                    }
                    break;
                case BlockType.TestFlight:
                    if (EngineConfigs.ContainsKey(block.name))
                        EngineConfigs[block.name].Reliability = new ReliabilityData(block);
                    else
                    {
                        EngineConfigs.Add(block.name, new EngineConfigData(this));
                    }
                    break;
                case BlockType.Part:
                    FetchEngineData(block.content);
                    foreach(var b in block.childrenBlocks)
                    {
                        GetDataFromBlock(b);
                    }
                    break;
            }
        }

        private EngineType GetType(ConfigBlock cfg)
        {
            string s = cfg.GetFieldValue("EngineType");

            var type = s switch
            {
                "LiquidFuel" => EngineType.Liquid,
                "SolidBooster" => EngineType.Solid,
                "Electric" => EngineType.Electric,
                _ => EngineType.Unknown,
            };

            return type;
        }
    }
}

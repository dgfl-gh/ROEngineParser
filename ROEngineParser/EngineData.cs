using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        [JsonIgnore]
        public string fileName;
        [JsonProperty(Order = 1)]
        public string Title { get; set; }
        [JsonProperty(Order = 2)]
        public string Manufacturer { get; set; }
        [JsonProperty(Order = 3)]
        public string Description { get; set; }
        [JsonProperty(Order = 4)]
        public float OriginalMass { get; set; }
        [JsonProperty(Order = 5)]
        public bool LiteralZeroIgnitions { get; set; }
        [JsonProperty(Order = 6)]
        [JsonConverter(typeof(StringEnumConverter))]
        public EngineType EngineType { get; set; }
        [JsonProperty(Order = 7)]
        public string DefaultConfig { get; set; }
        [JsonProperty(Order = 8)]
        public GimbalData Gimbal { get; set; } = new GimbalData();
        [JsonProperty(Order = 9)]
        public Dictionary<string, EngineConfigData> EngineConfigs = new Dictionary<string, EngineConfigData>();

        public EngineData(List<string[]> cfg)
        {
            if (cfg == null || cfg.Count == 0)
                return;

            FetchEngineData(cfg);
        }

        public string ToJson()
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                PreserveReferencesHandling = PreserveReferencesHandling.All
            };

            return JsonConvert.SerializeObject(this, settings);
        }

        public bool ToJsonFile(string path, bool overwrite = true)
        {
            string basePath = path;

            int i = 1;
            while (File.Exists(path) && !overwrite)
            {
                i++;
                path = $"{basePath}({i}).json";
            }

            if (Path.GetFullPath(path) is string fullPath)
            {
                try
                {
                    using StreamWriter file = File.CreateText(@fullPath);
                    JsonSerializer serializer = new JsonSerializer
                    {
                        Formatting = Formatting.Indented,
                        //PreserveReferencesHandling = PreserveReferencesHandling.All
                    };

                    serializer.Serialize(file, this);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }

                return true;
            }

            return false;
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
                    OriginalMass = block.GetFieldValue("origMass").ParseOrDefaultFloat(defVal: 1);
                    LiteralZeroIgnitions = block.GetFieldValue("literalZeroIgnitions").ParseOrDefaultBool(defVal: false);
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
                    foreach (var b in block.childrenBlocks)
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

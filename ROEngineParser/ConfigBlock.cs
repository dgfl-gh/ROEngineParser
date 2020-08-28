using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ROEngineParser
{
    public enum BlockType
    {
        Delete,
        Part,
        EngineType,
        ModuleGimbal,
        ModuleEngineConfigs,
        EngineConfig,
        AtmosphereCurve,
        Propellant,
        IgnitorResource,
        TestFlight,
        Unknown
    }

    public class ConfigBlock
    {
        public string name;
        public BlockType type = BlockType.Unknown;
        public List<string[]> content;
        public List<ConfigBlock> childrenBlocks;

        private static readonly Regex namePattern = new Regex(@"\[\S+\]");

        public ConfigBlock(List<string[]> block)
        {
            if (block == null || block.Count == 0)
                return;

            if (block[0][0].StartsWith("!"))
                type = BlockType.Delete;
            else if (block[0][0].Contains("PART"))
                type = BlockType.Part;
            else if (block[0][0].Contains("ModuleEngines*"))
                type = BlockType.EngineType;
            else if (block[0][0].Contains("ModuleGimbal"))
                type = BlockType.ModuleGimbal;
            else if (block[0][0].Contains("CONFIG"))
                type = BlockType.EngineConfig;
            else if (block[0][0].Contains("atmosphereCurve"))
                type = BlockType.AtmosphereCurve;
            else if (block[0][0].Contains("PROPELLANT"))
                type = BlockType.Propellant;
            else if (block[0][0].Contains("IGNITOR_RESOURCE"))
                type = BlockType.IgnitorResource;
            else if (block[0][0].Contains("TESTFLIGHT"))
                type = BlockType.TestFlight;
            else
                type = BlockType.Unknown;

            if (type == BlockType.Unknown || type == BlockType.TestFlight || type == BlockType.EngineConfig)
            {
                Match m = namePattern.Match(block[0][0]);
                if (m.Success)
                    name = m.Value.Trim(new char[2] { '[', ']' });

                if (string.IsNullOrEmpty(name))
                {
                    foreach (var line in block)
                    {
                        if (line.Length == 2 && line[0] == "name")
                        {
                            name = line[1];
                            break;
                        }
                    }
                }

                if (name.Contains("ModuleEngineConfigs"))
                    type = BlockType.ModuleEngineConfigs;
            }
            else if (type == BlockType.Propellant || type == BlockType.IgnitorResource)
            {
                foreach (var line in block)
                {
                    if (line.Length == 2 && line[0] == "name")
                    {
                        name = line[1];
                        break;
                    }
                }
            }

            block.RemoveRange(0, 2);            // remove first and second line (node name and opening bracket)
            block.RemoveAt(block.Count - 1);    // remove closing bracket

            content = block;

            PopulateChildrenBlocks();
        }

        public static ConfigBlock ExtractBlock(List<string[]> input, string blockStart, ref int index, bool remove = false)
        {
            int blockStartIndex = input.Count;
            int length = 0;
            uint bracketCounter = 0;

            for (int i = index; i < input.Count; i++)
            {
                string[] line = input[i];

                if (line[0].Contains(blockStart))
                {
                    blockStartIndex = i;
                }
                else if (line[0] == "{")
                {
                    bracketCounter++;
                }
                else if (line[0] == "}" && i > blockStartIndex && --bracketCounter == 0)
                {
                    length = (i - blockStartIndex) + 1;
                    break;
                }
            }

            List<string[]> block = input.GetRange(blockStartIndex, length);

            if(remove)
                input.RemoveRange(blockStartIndex, length);

            index--;

            return new ConfigBlock(block);
        }

        public string GetFieldValue(string field)
        {
            if (string.IsNullOrEmpty(field) || content == null || content.Count == 0 || type == BlockType.Delete)
                return null;

            foreach (var line in content)
            {
                string s = line[0].RemoveOperator();
                if (line.Length == 2 && s.Contains(field))
                {
                    return line[1];
                }
            }

            return null;
        }

        public string[] GetFieldValues(string field)
        {
            if (string.IsNullOrEmpty(field) || content == null || content.Count == 0 || type == BlockType.Delete)
                return null;

            var list = new List<string>();

            foreach (var line in content)
            {
                string s = line[0].RemoveOperator();
                if (line.Length == 2 && s.Contains(field))
                {
                    list.Add(line[1]);
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Recursively finds children blocks from the parent
        /// </summary>
        private void PopulateChildrenBlocks()
        {
            childrenBlocks = new List<ConfigBlock>();

            if (content == null || content.Count <= 3 || type == BlockType.Delete)
                return;

            uint bracketCounter = 0;
            int blockStartIndex = content.Count;
            int length;
            for (int i = 0; i < content.Count; i++)
            {
                string[] line = content[i];

                if (line[0] == "{")
                {
                    bracketCounter++;

                    if(bracketCounter == 1)
                        blockStartIndex = i - 1;
                }
                if (line[0] == "}" && i > blockStartIndex && --bracketCounter == 0)
                {
                    length = (i - blockStartIndex) + 1;

                    if (length > 0)
                    {
                        childrenBlocks.Add(ExtractBlock(content, content[blockStartIndex][0], ref blockStartIndex, true));
                        i -= length;
                    }
                }
            }
        }
    }
}

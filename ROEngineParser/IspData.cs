using System;

namespace ROEngineParser
{
    public class IspData
    {
        public float IspVacuum { get; set; }
        public float IspSeaLevel { get; set; }

        public IspData()
        {
            IspVacuum = 0;
            IspSeaLevel = 0;
        }

        public IspData(float vac, float sl)
        {
            IspVacuum = vac;
            IspSeaLevel = sl;
        }

        public IspData(ConfigBlock config)
        {
            string[] keyArray = config.GetFieldValues("key");

            // keys are in the {pressureAtm} {Isp} format
            // usually 2 are present, but we only care about SL (pressureAtm = 1)
            // and vacuum (pressureAtm = 0)
            foreach (var key in keyArray)
            {
                string[] array = key.Split(" ", 2, StringSplitOptions.None);
                if (array.Length <= 0)
                    continue;
                else if (int.Parse(array[0]) == 0)
                    IspVacuum = float.Parse(array[1]);
                else if (int.Parse(array[0]) == 1)
                    IspSeaLevel = float.Parse(array[1]);
            }
        }
    }
}

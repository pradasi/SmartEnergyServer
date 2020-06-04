namespace SmartEnergy.ConfigClass
{
    public class PythonFile
    {
        public string solarModel { get; set; }
        public string windModel { get; set; }
        public string windCsv { get; set; }
        public string solarCsv { get; set; }
        public string init { get; set; }
        public string predictedSolar { get; set; }
        public string predictedWind { get; set; }

    }
}

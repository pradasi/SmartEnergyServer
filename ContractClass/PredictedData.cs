using System.Collections.Generic;
namespace SmartEnergy.ContractClass
{
    public class PredictedData
    {
        public string fullHour { get; set; }
        public IEnumerable<int> hour { get; set; }
        public IEnumerable<double> value { get; set; }
    }
}

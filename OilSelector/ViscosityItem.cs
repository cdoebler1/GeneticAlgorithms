using SQLite;

namespace OilSelector
{
    public class ViscosityItem
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public double Viscosity { get; set; }
        public double C { get; set; }
        public double F { get; set; }
        public string Standard { get; set; }
        public string StockCode { get; set; }
    }
}

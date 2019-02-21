namespace PizzaChallenge.Entities
{
    public class SliceStatistics
    {
        public int BestAreaFilled { get; set; }
        public int AreaFilled { get; set; }
        public int SlicesProcessed { get; set; }
        public long GetFirstCellNotInSliceTime { get;set;}
        public long GetAvailableSlicesTime { get;set;}
    }
}

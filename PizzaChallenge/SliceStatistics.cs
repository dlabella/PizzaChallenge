﻿namespace PizzaChallenge
{
    public class SliceStatistics
    {
        public int AreaFilled { get; set; }
        public int SlicesProcessed { get; set; }
        public long GetFirstCellNotInSliceTime { get;set;}
        public long GetAvailableSlicesTime { get;set;}
    }
}
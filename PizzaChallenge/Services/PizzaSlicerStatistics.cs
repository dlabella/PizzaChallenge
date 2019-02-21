using PizzaChallenge.Entities;
using System;

namespace PizzaChallenge.Services
{
    public class PizzaSlicerStatistics
    {
        private DateTime _targetStatisticsTime;
        private const int _slideSeconds = 3;

        public PizzaSlicerStatistics()
        {
            SliceStatistics = new SliceStatistics();
        }
        private void SlideTime()
        {
            _targetStatisticsTime = DateTime.Now.AddSeconds(_slideSeconds);
        }
        public SliceStatistics SliceStatistics { get; }

        public void ProcessStatistics(bool force = false)
        {
            if (_targetStatisticsTime < DateTime.Now || force)
            {
                Console.WriteLine($"Area Filled {SliceStatistics.AreaFilled}");
                SlideTime();
            }
        }
    }
}

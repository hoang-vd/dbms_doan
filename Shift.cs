using System;

namespace BookstoreDBMS.Models
{
    public class Shift
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTime ShiftDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public int BreakDuration { get; set; }
        public decimal? TotalHours { get; set; }
        public decimal OvertimeHours { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation property
        public Employee Employee { get; set; }

        public Shift()
        {
            ShiftDate = DateTime.Now.Date;
            BreakDuration = 0;
            OvertimeHours = 0;
            Status = "Scheduled";
            CreatedAt = DateTime.Now;
        }

        // Calculate total hours
        public void CalculateTotalHours()
        {
            if (EndTime.HasValue)
            {
                var totalMinutes = (EndTime.Value - StartTime).TotalMinutes - BreakDuration;
                TotalHours = (decimal)(totalMinutes / 60.0);
            }
        }
    }
}
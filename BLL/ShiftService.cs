using System;
using System.Collections.Generic;
using System.Linq;
using BookstoreDBMS.DAL;
using BookstoreDBMS.Models;

namespace BookstoreDBMS.BLL
{
    public class ShiftService
    {
        private readonly ShiftRepository _shiftRepo;
        private readonly EmployeeRepository _employeeRepo;

        public const int START_HOUR = 6;
        public const int END_HOUR = 22;

        public ShiftService()
        {
            _shiftRepo = new ShiftRepository();
            _employeeRepo = new EmployeeRepository();
        }

        public List<Shift> GetShiftsByDate(DateTime date) => _shiftRepo.GetByDate(date);

        public int AddShift(Shift shift)
        {
            ValidateShift(shift, false);
            if (shift.EndTime.HasValue)
                shift.CalculateTotalHours();
            return _shiftRepo.Insert(shift);
        }

        public bool UpdateShift(Shift shift)
        {
            ValidateShift(shift, true);
            if (shift.EndTime.HasValue)
                shift.CalculateTotalHours();
            return _shiftRepo.Update(shift);
        }

        public bool DeleteShift(int id) => _shiftRepo.Delete(id);

        public CoverageResult GetCoverage(DateTime date)
        {
            var shifts = GetShiftsByDate(date);
            var intervals = shifts
                .Where(s => s.EndTime.HasValue)
                .Select(s => (Start: s.StartTime, End: s.EndTime!.Value))
                .OrderBy(i => i.Start)
                .ToList();

            var merged = new List<(TimeSpan Start, TimeSpan End)>();
            foreach (var iv in intervals)
            {
                if (merged.Count == 0 || iv.Start > merged[^1].End)
                    merged.Add(iv);
                else if (iv.End > merged[^1].End)
                    merged[^1] = (merged[^1].Start, iv.End);
            }

            var requiredStart = TimeSpan.FromHours(START_HOUR);
            var requiredEnd = TimeSpan.FromHours(END_HOUR);
            var gaps = new List<(TimeSpan Start, TimeSpan End)>();
            var cursor = requiredStart;

            foreach (var block in merged)
            {
                if (block.End <= requiredStart || block.Start >= requiredEnd)
                    continue;
                var bs = block.Start < requiredStart ? requiredStart : block.Start;
                var be = block.End > requiredEnd ? requiredEnd : block.End;
                if (bs > cursor) gaps.Add((cursor, bs));
                if (be > cursor) cursor = be;
            }
            if (cursor < requiredEnd)
                gaps.Add((cursor, requiredEnd));

            return new CoverageResult
            {
                Date = date.Date,
                IsFullyCovered = gaps.Count == 0,
                MissingSegments = gaps,
                Shifts = shifts
            };
        }

        private void ValidateShift(Shift s, bool isUpdate)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            if (isUpdate && s.Id <= 0) throw new ArgumentException("ID ca không hợp lệ");
            if (s.EmployeeId <= 0) throw new ArgumentException("Phải chọn nhân viên");

            var start = s.StartTime;
            var end = s.EndTime;
            var min = TimeSpan.FromHours(START_HOUR);
            var max = TimeSpan.FromHours(END_HOUR);

            if (start < min || start > max)
                throw new ArgumentException("Giờ bắt đầu phải trong khung 06:00–22:00");
            if (end.HasValue)
            {
                if (end.Value <= start) throw new ArgumentException("Giờ kết thúc phải sau giờ bắt đầu");
                if (end.Value > max) throw new ArgumentException("Giờ kết thúc không được vượt quá 22:00");
            }
            if (s.BreakDuration < 0) throw new ArgumentException("Thời gian nghỉ không hợp lệ");
            if (end.HasValue)
            {
                var totalMinutes = (end.Value - start).TotalMinutes;
                if (s.BreakDuration > totalMinutes)
                    throw new ArgumentException("Thời gian nghỉ vượt quá tổng thời gian ca");
            }
        }
    }

    public class CoverageResult
    {
        public DateTime Date { get; set; }
        public bool IsFullyCovered { get; set; }
        public List<(TimeSpan Start, TimeSpan End)> MissingSegments { get; set; } = new();
        public List<Shift> Shifts { get; set; } = new();
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BookstoreDBMS.Models;

namespace BookstoreDBMS.DAL
{
    public class ShiftRepository
    {
        public List<Shift> GetByDate(DateTime date)
        {
            var result = new List<Shift>();
            using var conn = DatabaseHelper.GetConnection();
            var cmd = new SqlCommand(@"SELECT s.id, s.employee_id, s.shift_date, s.start_time, s.end_time,
                         s.break_duration, s.overtime_hours, s.status, s.notes, s.created_at,
                         e.name as employee_name, e.role_id, r.name as role_name
                  FROM shifts s
                  INNER JOIN employees e ON s.employee_id = e.id
                  INNER JOIN roles r ON e.role_id = r.id
                  WHERE s.shift_date = @d
                  ORDER BY s.start_time", conn);
            cmd.Parameters.AddWithValue("@d", date.Date);
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                result.Add(new Shift
                {
                    Id = rd.GetInt32(rd.GetOrdinal("id")),
                    EmployeeId = rd.GetInt32(rd.GetOrdinal("employee_id")),
                    ShiftDate = rd.GetDateTime(rd.GetOrdinal("shift_date")),
                    StartTime = (TimeSpan)rd["start_time"],
                    EndTime = rd["end_time"] == DBNull.Value ? null : (TimeSpan?)rd["end_time"],
                    BreakDuration = rd.GetInt32(rd.GetOrdinal("break_duration")),
                    OvertimeHours = rd.GetDecimal(rd.GetOrdinal("overtime_hours")),
                    Status = rd.GetString(rd.GetOrdinal("status")),
                    Notes = rd["notes"] == DBNull.Value ? null : rd.GetString(rd.GetOrdinal("notes")),
                    CreatedAt = rd.GetDateTime(rd.GetOrdinal("created_at")),
                    Employee = new Employee
                    {
                        Id = rd.GetInt32(rd.GetOrdinal("employee_id")),
                        Name = rd.GetString(rd.GetOrdinal("employee_name")),
                        RoleId = rd.GetInt32(rd.GetOrdinal("role_id")),
                        Role = new Role { Id = rd.GetInt32(rd.GetOrdinal("role_id")), Name = rd.GetString(rd.GetOrdinal("role_name")) }
                    }
                });
            }
            return result;
        }

        public int Insert(Shift s)
        {
            using var conn = DatabaseHelper.GetConnection();
            var cmd = new SqlCommand(@"INSERT INTO shifts (employee_id, shift_date, start_time, end_time, break_duration, overtime_hours, status, notes)
                  VALUES (@emp,@date,@start,@end,@break,@ot,@status,@notes);
                  SELECT SCOPE_IDENTITY();", conn);
            cmd.Parameters.AddWithValue("@emp", s.EmployeeId);
            cmd.Parameters.AddWithValue("@date", s.ShiftDate.Date);
            cmd.Parameters.AddWithValue("@start", s.StartTime);
            cmd.Parameters.AddWithValue("@end", (object)s.EndTime ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@break", s.BreakDuration);
            cmd.Parameters.AddWithValue("@ot", s.OvertimeHours);
            cmd.Parameters.AddWithValue("@status", s.Status ?? "Scheduled");
            cmd.Parameters.AddWithValue("@notes", (object)s.Notes ?? DBNull.Value);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool Update(Shift s)
        {
            using var conn = DatabaseHelper.GetConnection();
            var cmd = new SqlCommand(@"UPDATE shifts SET employee_id=@emp, shift_date=@date, start_time=@start,
                   end_time=@end, break_duration=@break, overtime_hours=@ot, status=@status, notes=@notes
                   WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@id", s.Id);
            cmd.Parameters.AddWithValue("@emp", s.EmployeeId);
            cmd.Parameters.AddWithValue("@date", s.ShiftDate.Date);
            cmd.Parameters.AddWithValue("@start", s.StartTime);
            cmd.Parameters.AddWithValue("@end", (object)s.EndTime ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@break", s.BreakDuration);
            cmd.Parameters.AddWithValue("@ot", s.OvertimeHours);
            cmd.Parameters.AddWithValue("@status", s.Status ?? "Scheduled");
            cmd.Parameters.AddWithValue("@notes", (object)s.Notes ?? DBNull.Value);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Delete(int id)
        {
            using var conn = DatabaseHelper.GetConnection();
            var cmd = new SqlCommand("DELETE FROM shifts WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteNonQuery() > 0;
        }

        public List<Shift> GetByEmployee(int employeeId, DateTime from, DateTime to)
        {
            var result = new List<Shift>();
            using var conn = DatabaseHelper.GetConnection();
            var cmd = new SqlCommand(@"SELECT id, employee_id, shift_date, start_time, end_time, break_duration, overtime_hours, status, notes, created_at
                  FROM shifts
                  WHERE employee_id=@emp AND shift_date BETWEEN @f AND @t
                  ORDER BY shift_date, start_time", conn);
            cmd.Parameters.AddWithValue("@emp", employeeId);
            cmd.Parameters.AddWithValue("@f", from.Date);
            cmd.Parameters.AddWithValue("@t", to.Date);
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                result.Add(new Shift
                {
                    Id = rd.GetInt32(rd.GetOrdinal("id")),
                    EmployeeId = rd.GetInt32(rd.GetOrdinal("employee_id")),
                    ShiftDate = rd.GetDateTime(rd.GetOrdinal("shift_date")),
                    StartTime = (TimeSpan)rd["start_time"],
                    EndTime = rd["end_time"] == DBNull.Value ? null : (TimeSpan?)rd["end_time"],
                    BreakDuration = rd.GetInt32(rd.GetOrdinal("break_duration")),
                    OvertimeHours = rd.GetDecimal(rd.GetOrdinal("overtime_hours")),
                    Status = rd.GetString(rd.GetOrdinal("status")),
                    Notes = rd["notes"] == DBNull.Value ? null : rd.GetString(rd.GetOrdinal("notes")),
                    CreatedAt = rd.GetDateTime(rd.GetOrdinal("created_at"))
                });
            }
            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BookstoreDBMS.BLL;
using BookstoreDBMS.Models;
using System.ComponentModel; // For design-time detection

namespace BookstoreDBMS.UI
{
    public class ShiftUserControl : UserControl
    {
        private ShiftService _shiftService;
        private EmployeeService _employeeService;
        private Shift _currentShift;
        private bool _isEditing = false;

        // Controls
        private DateTimePicker dtpDate;
        private DataGridView dgvShifts;
        private ComboBox cmbEmployee;
        private DateTimePicker dtpStart;
        private DateTimePicker dtpEnd;
        private NumericUpDown nudBreak;
    private TextBox txtNotes; // notes is already a TextBox (giữ nguyên dạng TextBox)
    private Button btnAdd; private Button btnEdit; private Button btnDelete; private Button btnSave; private Button btnCancel; private Button btnCoverage;
        private Label lblCoverage;
        private Panel pnlTop; private Panel pnlForm; private GroupBox grpForm;

        private bool _runtimeInitialized = false;
        private bool IsDesignEnvironment()
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return true;
            if (Site?.DesignMode == true) return true;
            try
            {
                var process = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
                if (string.Equals(process, "devenv", StringComparison.OrdinalIgnoreCase) || string.Equals(process, "Blend", StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            catch { }
            return false;
        }

        public ShiftUserControl()
        {
            InitializeComponent(); // Only build UI tree here; no service calls.
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (_runtimeInitialized) return;
            if (IsDesignEnvironment()) return; // Skip data/service init in designer

            _shiftService = new ShiftService();
            _employeeService = new EmployeeService();
            try
            {
                LoadEmployees();
                LoadShifts();
                UpdateCoverageLabel();
                // Attach events only after services ready
                dtpDate.ValueChanged += DtpDate_ValueChanged;
                dgvShifts.SelectionChanged += DgvShifts_SelectionChanged;
                btnAdd.Click += BtnAdd_Click;
                btnEdit.Click += BtnEdit_Click;
                btnDelete.Click += BtnDelete_Click;
                btnSave.Click += BtnSave_Click;
                btnCancel.Click += BtnCancel_Click;
                btnCoverage.Click += BtnCoverage_Click;
            }
            catch (Exception ex)
            {
                // Optional: show once at runtime only
                System.Diagnostics.Debug.WriteLine("ShiftUserControl init error: " + ex.Message);
            }
            SetFormState(false);
            _runtimeInitialized = true;
        }

        // Event handler implementations (runtime only)
        private void DtpDate_ValueChanged(object sender, EventArgs e)
        {
            if (_shiftService == null) return;
            LoadShifts();
            UpdateCoverageLabel();
        }
        private void DgvShifts_SelectionChanged(object sender, EventArgs e)
        {
            if (_shiftService == null) return;
            if(!_isEditing) LoadSelectedToForm(false);
            UpdateButtons();
        }
        private void BtnAdd_Click(object sender, EventArgs e) => StartAdd();
        private void BtnEdit_Click(object sender, EventArgs e) => StartEdit();
        private void BtnDelete_Click(object sender, EventArgs e) => DeleteShift();
        private void BtnSave_Click(object sender, EventArgs e) => SaveShift();
        private void BtnCancel_Click(object sender, EventArgs e) => CancelEdit();
        private void BtnCoverage_Click(object sender, EventArgs e) => UpdateCoverageLabel(true);

        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
            // Nền trắng cho toàn bộ khu vực xếp ca
            this.BackColor = Color.White;

            dtpDate = new DateTimePicker { Format = DateTimePickerFormat.Short, Value = DateTime.Today };

            btnAdd = new Button { Text = "Thêm"};
            btnEdit = new Button { Text = "Sửa"};
            btnDelete = new Button { Text = "Xóa"};
            btnSave = new Button { Text = "Lưu"};
            btnCancel = new Button { Text = "Hủy"};
            btnCoverage = new Button { Text = "Kiểm tra phủ"};

            pnlTop = new Panel { Dock = DockStyle.Top, Height = 45, Padding = new Padding(8) };
            pnlTop.Controls.AddRange(new Control[]{ dtpDate, btnAdd, btnEdit, btnDelete, btnSave, btnCancel, btnCoverage });
            int left = 10; foreach(Control c in pnlTop.Controls){ c.Location = new Point(left,8); left += c.Width + 8; }

            dgvShifts = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            // Màu nền trắng + tinh chỉnh màu lưới & selection
            dgvShifts.BackgroundColor = Color.White;
            dgvShifts.EnableHeadersVisualStyles = false;
            dgvShifts.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
            dgvShifts.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvShifts.DefaultCellStyle.BackColor = Color.White;
            dgvShifts.DefaultCellStyle.ForeColor = Color.Black;
            dgvShifts.DefaultCellStyle.SelectionBackColor = Color.FromArgb(200,225,255);
            dgvShifts.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvShifts.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248,248,248);
            dgvShifts.GridColor = Color.Gainsboro;
            // SelectionChanged gắn sau ở runtime (OnLoad)

            // Form panel
            pnlForm = new Panel { Dock = DockStyle.Right, Width = 340, Padding = new Padding(10) };
            grpForm = new GroupBox { Text = "Ca làm việc", Dock = DockStyle.Fill, Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold) };

            var lblEmp = new Label { Text = "Nhân viên:", AutoSize = true };
            cmbEmployee = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 200 };
            var lblStart = new Label { Text = "Bắt đầu:", AutoSize = true };
            dtpStart = new DateTimePicker { Format = DateTimePickerFormat.Time, ShowUpDown = true, Width = 120 };
            var lblEnd = new Label { Text = "Kết thúc:", AutoSize = true };
            dtpEnd = new DateTimePicker { Format = DateTimePickerFormat.Time, ShowUpDown = true, Width = 120 };
            var lblBreak = new Label { Text = "Nghỉ (phút):", AutoSize = true };
            nudBreak = new NumericUpDown { Minimum = 0, Maximum = 600, Increment = 5, Width = 120 };
            var lblNotes = new Label { Text = "Ghi chú:", AutoSize = true };
            txtNotes = new TextBox { Multiline = true, Height = 130, Width = 240, ScrollBars = ScrollBars.Vertical };

            lblCoverage = new Label { Text = "Coverage", Dock = DockStyle.Bottom, Height = 30, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };

            var tlp = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 7, Padding = new Padding(10) };
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,40));
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,60));
            for(int i=0;i<6;i++) tlp.RowStyles.Add(new RowStyle(SizeType.Absolute,35));
            tlp.RowStyles[4].Height = 150; // hàng ghi chú cao hơn
            tlp.RowStyles.Add(new RowStyle(SizeType.Percent,100));

            tlp.Controls.Add(lblEmp,0,0); tlp.Controls.Add(cmbEmployee,1,0);
            tlp.Controls.Add(lblStart,0,1); tlp.Controls.Add(dtpStart,1,1);
            tlp.Controls.Add(lblEnd,0,2); tlp.Controls.Add(dtpEnd,1,2);
            tlp.Controls.Add(lblBreak,0,3); tlp.Controls.Add(nudBreak,1,3);
            tlp.Controls.Add(lblNotes,0,4); tlp.Controls.Add(txtNotes,1,4);
            grpForm.Controls.Add(tlp);
            pnlForm.Controls.Add(grpForm);

            this.Controls.Add(dgvShifts);
            this.Controls.Add(pnlForm);
            this.Controls.Add(pnlTop);
            this.Controls.Add(lblCoverage);
        }

        private void LoadEmployees()
        {
            try
            {
                var employees = _employeeService.GetAllEmployees();
                cmbEmployee.DataSource = employees;
                cmbEmployee.DisplayMember = "Name";
                cmbEmployee.ValueMember = "Id";
                cmbEmployee.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải nhân viên: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadShifts()
        {
            if (_shiftService == null) return; // Designer guard
            try
            {
                var list = _shiftService.GetShiftsByDate(dtpDate.Value.Date);
                dgvShifts.DataSource = list.Select(s => new {
                    ID = s.Id,
                    NhânViên = s.Employee?.Name,
                    BắtĐầu = s.StartTime.ToString(@"hh\:mm"),
                    KếtThúc = s.EndTime.HasValue ? s.EndTime.Value.ToString(@"hh\:mm") : "(Chưa)",
                    NghỉPhút = s.BreakDuration,
                    TrạngThái = s.Status,
                    GhiChú = s.Notes
                }).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải ca: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StartAdd()
        {
            _currentShift = new Shift { ShiftDate = dtpDate.Value.Date, StartTime = TimeSpan.FromHours(ShiftService.START_HOUR) };
            _isEditing = true;
            ClearForm();
            SetFormState(true);
            dtpStart.Value = DateTime.Today.AddHours(ShiftService.START_HOUR);
            dtpEnd.Value = DateTime.Today.AddHours(ShiftService.START_HOUR + 1);
        }

        private void StartEdit()
        {
            if (dgvShifts.SelectedRows.Count == 0) return;
            LoadSelectedToForm(true);
        }

        private void LoadSelectedToForm(bool editing)
        {
            if (_shiftService == null) return; // Designer guard
            if (dgvShifts.SelectedRows.Count == 0) return;
            var id = (int)dgvShifts.SelectedRows[0].Cells["ID"].Value;
            var shift = _shiftService.GetShiftsByDate(dtpDate.Value.Date).FirstOrDefault(s => s.Id == id);
            if (shift == null) return;
            _currentShift = shift;
            _isEditing = editing;
            cmbEmployee.SelectedValue = shift.EmployeeId;
            dtpStart.Value = shift.ShiftDate.Date.Add(shift.StartTime);
            dtpEnd.Value = shift.EndTime.HasValue ? shift.ShiftDate.Date.Add(shift.EndTime.Value) : shift.ShiftDate.Date.AddHours(ShiftService.START_HOUR + 1);
            nudBreak.Value = shift.BreakDuration;
            txtNotes.Text = shift.Notes;
            SetFormState(editing);
        }

        private void SaveShift()
        {
            if (_currentShift == null) return;
            try
            {
                if (cmbEmployee.SelectedIndex == -1) { MessageBox.Show("Chọn nhân viên", "Thiếu dữ liệu"); return; }
                _currentShift.EmployeeId = (int)cmbEmployee.SelectedValue;
                _currentShift.ShiftDate = dtpDate.Value.Date;
                _currentShift.StartTime = dtpStart.Value.TimeOfDay;
                _currentShift.EndTime = dtpEnd.Value.TimeOfDay;
                _currentShift.BreakDuration = (int)nudBreak.Value;
                _currentShift.Notes = string.IsNullOrWhiteSpace(txtNotes.Text) ? null : txtNotes.Text.Trim();
                _currentShift.Status = "Scheduled";

                bool isNew = _currentShift.Id == 0;
                if (isNew)
                {
                    var newId = _shiftService.AddShift(_currentShift);
                    if (newId > 0) MessageBox.Show("Thêm ca thành công", "Thành công");
                }
                else
                {
                    if (_shiftService.UpdateShift(_currentShift)) MessageBox.Show("Cập nhật ca thành công", "Thành công");
                }
                _isEditing = false;
                SetFormState(false);
                LoadShifts();
                UpdateCoverageLabel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi lưu ca: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteShift()
        {
            if (dgvShifts.SelectedRows.Count == 0) return;
            var id = (int)dgvShifts.SelectedRows[0].Cells["ID"].Value;
            var confirm = MessageBox.Show("Xóa ca này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                try
                {
                    if (_shiftService.DeleteShift(id))
                    {
                        LoadShifts();
                        UpdateCoverageLabel();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi xóa ca: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void CancelEdit()
        {
            _isEditing = false;
            SetFormState(false);
            if (dgvShifts.SelectedRows.Count > 0) LoadSelectedToForm(false); else ClearForm();
        }

        private void ClearForm()
        {
            cmbEmployee.SelectedIndex = -1;
            nudBreak.Value = 0;
            txtNotes.Clear();
        }

        private void SetFormState(bool editing)
        {
            cmbEmployee.Enabled = editing;
            dtpStart.Enabled = editing;
            dtpEnd.Enabled = editing;
            nudBreak.Enabled = editing;
            txtNotes.ReadOnly = !editing;

            btnSave.Enabled = editing;
            btnCancel.Enabled = editing;
            btnAdd.Enabled = !editing;
            btnEdit.Enabled = !editing && dgvShifts.SelectedRows.Count > 0;
            btnDelete.Enabled = !editing && dgvShifts.SelectedRows.Count > 0;
            btnCoverage.Enabled = !editing;
            grpForm.Text = editing ? (_currentShift?.Id == 0 ? "Thêm ca mới" : "Chỉnh sửa ca") : "Ca làm việc";
        }

        private void UpdateButtons() => SetFormState(_isEditing);

        private void UpdateCoverageLabel(bool showMessage = false)
        {
            if (_shiftService == null) return; // Designer guard
            try
            {
                var coverage = _shiftService.GetCoverage(dtpDate.Value.Date);
                if (coverage.IsFullyCovered)
                {
                    lblCoverage.Text = "ĐÃ PHỦ ĐẦY 06:00 - 22:00";
                    lblCoverage.ForeColor = Color.Green;
                    if (showMessage) MessageBox.Show("Toàn bộ khung giờ đã được phủ", "Thông tin");
                }
                else
                {
                    lblCoverage.Text = "THIẾU: " + string.Join(", ", coverage.MissingSegments.Select(g => $"{g.Start:hh\\:mm}-{g.End:hh\\:mm}"));
                    lblCoverage.ForeColor = Color.Red;
                    if (showMessage) MessageBox.Show("Thiếu các khoảng: " + string.Join(", ", coverage.MissingSegments.Select(g => $"{g.Start:hh\\:mm}-{g.End:hh\\:mm}")), "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                lblCoverage.Text = "Lỗi tính coverage";
                lblCoverage.ForeColor = Color.DarkRed;
                if (showMessage) MessageBox.Show($"Lỗi coverage: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

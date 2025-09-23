using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BookstoreDBMS.BLL;
using BookstoreDBMS.Models;

namespace BookstoreDBMS.UI
{
    public partial class EmployeeUserControl : UserControl
    {
        private EmployeeService _employeeService;
        private RoleService _roleService;
        private Employee _currentEmployee;
        private bool _isEditing = false;

        // Controls
        private DataGridView dgvEmployees;
        private TextBox txtFullName;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private TextBox txtAddress;
        private ComboBox cmbRole;
        private DateTimePicker dtpHireDate;
        private DateTimePicker dtpBirthDate;
        private NumericUpDown nudSalary;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnSave;
        private Button btnCancel;
        private Button btnRefresh;
        private Label lblFullName;
        private Label lblEmail;
        private Label lblPhone;
        private Label lblAddress;
        private Label lblRole;
        private Label lblHireDate;
        private Label lblSalary;
        private Panel pnlControls;
        private Panel pnlForm;
        private GroupBox grpEmployeeInfo;
        private TableLayoutPanel tlpForm;

        public EmployeeUserControl()
        {
            InitializeComponent();
            _employeeService = new EmployeeService();
            _roleService = new RoleService();
            LoadRoles();
            LoadEmployees();
            SetFormState(false);
        }

        private void InitializeComponent()
        {
            this.dgvEmployees = new DataGridView();
            this.pnlControls = new Panel();
            this.pnlForm = new Panel();
            this.grpEmployeeInfo = new GroupBox();
            this.tlpForm = new TableLayoutPanel();
            this.txtFullName = new TextBox();
            this.txtEmail = new TextBox();
            this.txtPhone = new TextBox();
            this.txtAddress = new TextBox();
            this.cmbRole = new ComboBox();
            this.dtpHireDate = new DateTimePicker();
            this.nudSalary = new NumericUpDown();
            this.lblFullName = new Label();
            this.lblEmail = new Label();
            this.lblPhone = new Label();
            this.lblAddress = new Label();
            this.lblRole = new Label();
            this.lblHireDate = new Label();
            this.lblSalary = new Label();
            this.btnAdd = new Button();
            this.btnEdit = new Button();
            this.btnDelete = new Button();
            this.btnSave = new Button();
            this.btnCancel = new Button();
            this.btnRefresh = new Button();
            
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployees)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSalary)).BeginInit();
            this.pnlControls.SuspendLayout();
            this.pnlForm.SuspendLayout();
            this.grpEmployeeInfo.SuspendLayout();
            this.tlpForm.SuspendLayout();
            this.SuspendLayout();

            // 
            // dgvEmployees
            // 
            this.dgvEmployees.AllowUserToAddRows = false;
            this.dgvEmployees.AllowUserToDeleteRows = false;
            this.dgvEmployees.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvEmployees.BackgroundColor = SystemColors.Window;
            this.dgvEmployees.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEmployees.Dock = DockStyle.Fill;
            this.dgvEmployees.Location = new Point(0, 0);
            this.dgvEmployees.MultiSelect = false;
            this.dgvEmployees.Name = "dgvEmployees";
            this.dgvEmployees.ReadOnly = true;
            this.dgvEmployees.RowHeadersWidth = 25;
            this.dgvEmployees.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvEmployees.Size = new Size(600, 400);
            this.dgvEmployees.TabIndex = 0;
            this.dgvEmployees.SelectionChanged += new EventHandler(this.dgvEmployees_SelectionChanged);

            // 
            // pnlControls
            // 
            this.pnlControls.Controls.Add(this.btnRefresh);
            this.pnlControls.Controls.Add(this.btnDelete);
            this.pnlControls.Controls.Add(this.btnEdit);
            this.pnlControls.Controls.Add(this.btnAdd);
            this.pnlControls.Dock = DockStyle.Top;
            this.pnlControls.Location = new Point(0, 0);
            this.pnlControls.Name = "pnlControls";
            this.pnlControls.Size = new Size(1000, 50);
            this.pnlControls.TabIndex = 1;

            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = Color.FromArgb(0, 123, 255);
            this.btnAdd.FlatStyle = FlatStyle.Flat;
            this.btnAdd.ForeColor = Color.White;
            this.btnAdd.Location = new Point(10, 10);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new Size(80, 30);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "Thêm";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new EventHandler(this.btnAdd_Click);

            // 
            // btnEdit
            // 
            this.btnEdit.BackColor = Color.FromArgb(255, 193, 7);
            this.btnEdit.FlatStyle = FlatStyle.Flat;
            this.btnEdit.ForeColor = Color.Black;
            this.btnEdit.Location = new Point(100, 10);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new Size(80, 30);
            this.btnEdit.TabIndex = 1;
            this.btnEdit.Text = "Sửa";
            this.btnEdit.UseVisualStyleBackColor = false;
            this.btnEdit.Click += new EventHandler(this.btnEdit_Click);

            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = Color.FromArgb(220, 53, 69);
            this.btnDelete.FlatStyle = FlatStyle.Flat;
            this.btnDelete.ForeColor = Color.White;
            this.btnDelete.Location = new Point(190, 10);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new Size(80, 30);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "Xóa";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new EventHandler(this.btnDelete_Click);

            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = Color.FromArgb(108, 117, 125);
            this.btnRefresh.FlatStyle = FlatStyle.Flat;
            this.btnRefresh.ForeColor = Color.White;
            this.btnRefresh.Location = new Point(280, 10);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new Size(80, 30);
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.Text = "Làm mới";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new EventHandler(this.btnRefresh_Click);

            // 
            // pnlForm
            // 
            this.pnlForm.Controls.Add(this.grpEmployeeInfo);
            this.pnlForm.Dock = DockStyle.Right;
            this.pnlForm.Location = new Point(600, 50);
            this.pnlForm.Name = "pnlForm";
            this.pnlForm.Size = new Size(400, 500);
            this.pnlForm.TabIndex = 2;

            // 
            // grpEmployeeInfo
            // 
            this.grpEmployeeInfo.Controls.Add(this.tlpForm);
            this.grpEmployeeInfo.Dock = DockStyle.Fill;
            this.grpEmployeeInfo.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            this.grpEmployeeInfo.Location = new Point(0, 0);
            this.grpEmployeeInfo.Name = "grpEmployeeInfo";
            this.grpEmployeeInfo.Padding = new Padding(10);
            this.grpEmployeeInfo.Size = new Size(400, 500);
            this.grpEmployeeInfo.TabIndex = 0;
            this.grpEmployeeInfo.TabStop = false;
            this.grpEmployeeInfo.Text = "Thông tin nhân viên";

            // 
            // tlpForm
            // 
            this.tlpForm.ColumnCount = 2;
            this.tlpForm.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            this.tlpForm.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            // Add controls to table layout (now with Address row inserted at index 3)
            this.tlpForm.Controls.Add(this.lblFullName, 0, 0);
            this.tlpForm.Controls.Add(this.txtFullName, 1, 0);
            this.tlpForm.Controls.Add(this.lblEmail, 0, 1);
            this.tlpForm.Controls.Add(this.txtEmail, 1, 1);
            this.tlpForm.Controls.Add(this.lblPhone, 0, 2);
            this.tlpForm.Controls.Add(this.txtPhone, 1, 2);
            this.tlpForm.Controls.Add(this.lblAddress, 0, 3);
            this.tlpForm.Controls.Add(this.txtAddress, 1, 3);
            this.tlpForm.Controls.Add(this.lblRole, 0, 4);
            this.tlpForm.Controls.Add(this.cmbRole, 1, 4);
            this.tlpForm.Controls.Add(this.lblHireDate, 0, 5);
            this.tlpForm.Controls.Add(this.dtpHireDate, 1, 5);
            this.tlpForm.Controls.Add(this.lblSalary, 0, 6);
            this.tlpForm.Controls.Add(this.nudSalary, 1, 6);
            this.tlpForm.Controls.Add(this.btnSave, 0, 7);
            this.tlpForm.Controls.Add(this.btnCancel, 1, 7);
            this.tlpForm.Dock = DockStyle.Fill;
            this.tlpForm.Location = new Point(10, 23);
            this.tlpForm.Name = "tlpForm";
            this.tlpForm.RowCount = 8; // Added Address row
            this.tlpForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F)); // Full name
            this.tlpForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F)); // Email
            this.tlpForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F)); // Phone
            this.tlpForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F)); // Address
            this.tlpForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F)); // Role
            this.tlpForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F)); // Hire date
            this.tlpForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F)); // Salary
            this.tlpForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F)); // Buttons
            this.tlpForm.Size = new Size(380, 467);
            this.tlpForm.TabIndex = 0;

            // Labels
            this.lblFullName.Anchor = AnchorStyles.Left;
            this.lblFullName.AutoSize = true;
            this.lblFullName.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular);
            this.lblFullName.Location = new Point(3, 12);
            this.lblFullName.Name = "lblFullName";
            this.lblFullName.Size = new Size(62, 15);
            this.lblFullName.TabIndex = 0;
            this.lblFullName.Text = "Họ và tên:";

            this.lblEmail.Anchor = AnchorStyles.Left;
            this.lblEmail.AutoSize = true;
            this.lblEmail.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular);
            this.lblEmail.Location = new Point(3, 52);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new Size(42, 15);
            this.lblEmail.TabIndex = 2;
            this.lblEmail.Text = "Email:";

            this.lblPhone.Anchor = AnchorStyles.Left;
            this.lblPhone.AutoSize = true;
            this.lblPhone.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular);
            this.lblPhone.Location = new Point(3, 92);
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Size = new Size(75, 15);
            this.lblPhone.TabIndex = 4;
            this.lblPhone.Text = "Số điện thoại:";

            this.lblAddress.Anchor = AnchorStyles.Left;
            this.lblAddress.AutoSize = true;
            this.lblAddress.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular);
            this.lblAddress.Location = new Point(3, 132);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new Size(49, 15);
            this.lblAddress.TabIndex = 6;
            this.lblAddress.Text = "Địa chỉ:";

            this.lblRole.Anchor = AnchorStyles.Left;
            this.lblRole.AutoSize = true;
            this.lblRole.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular);
            this.lblRole.Location = new Point(3, 172);
            this.lblRole.Name = "lblRole";
            this.lblRole.Size = new Size(50, 15);
            this.lblRole.TabIndex = 8;
            this.lblRole.Text = "Chức vụ:";

            this.lblHireDate.Anchor = AnchorStyles.Left;
            this.lblHireDate.AutoSize = true;
            this.lblHireDate.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular);
            this.lblHireDate.Location = new Point(3, 212);
            this.lblHireDate.Name = "lblHireDate";
            this.lblHireDate.Size = new Size(79, 15);
            this.lblHireDate.TabIndex = 10;
            this.lblHireDate.Text = "Ngày vào làm:";

            this.lblSalary.Anchor = AnchorStyles.Left;
            this.lblSalary.AutoSize = true;
            this.lblSalary.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular);
            this.lblSalary.Location = new Point(3, 252);
            this.lblSalary.Name = "lblSalary";
            this.lblSalary.Size = new Size(45, 15);
            this.lblSalary.TabIndex = 12;
            this.lblSalary.Text = "Lương:";

            // Input Controls
            this.txtFullName.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            this.txtFullName.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular);
            this.txtFullName.Location = new Point(117, 9);
            this.txtFullName.MaxLength = 100;
            this.txtFullName.Name = "txtFullName";
            this.txtFullName.Size = new Size(260, 21);
            this.txtFullName.TabIndex = 1;

            this.txtEmail.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            this.txtEmail.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular);
            this.txtEmail.Location = new Point(117, 49);
            this.txtEmail.MaxLength = 100;
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new Size(260, 21);
            this.txtEmail.TabIndex = 3;

            this.txtPhone.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            this.txtPhone.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular);
            this.txtPhone.Location = new Point(117, 89);
            this.txtPhone.MaxLength = 15;
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new Size(260, 21);
            this.txtPhone.TabIndex = 5;

            this.txtAddress.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            this.txtAddress.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular);
            this.txtAddress.Location = new Point(117, 129);
            this.txtAddress.MaxLength = 255;
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new Size(260, 21);
            this.txtAddress.TabIndex = 7;

            this.cmbRole.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            this.cmbRole.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbRole.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular);
            this.cmbRole.FormattingEnabled = true;
            this.cmbRole.Location = new Point(117, 169);
            this.cmbRole.Name = "cmbRole";
            this.cmbRole.Size = new Size(260, 23);
            this.cmbRole.TabIndex = 9;

            this.dtpHireDate.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            this.dtpHireDate.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular);
            this.dtpHireDate.Format = DateTimePickerFormat.Short;
            this.dtpHireDate.Location = new Point(117, 209);
            this.dtpHireDate.Name = "dtpHireDate";
            this.dtpHireDate.Size = new Size(260, 21);
            this.dtpHireDate.TabIndex = 11;

            this.nudSalary.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            this.nudSalary.DecimalPlaces = 0;
            this.nudSalary.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular);
            this.nudSalary.Increment = new decimal(new int[] { 100000, 0, 0, 0 });
            this.nudSalary.Location = new Point(117, 249);
            this.nudSalary.Maximum = new decimal(new int[] { 999999999, 0, 0, 0 });
            this.nudSalary.Name = "nudSalary";
            this.nudSalary.Size = new Size(260, 21);
            this.nudSalary.TabIndex = 13;
            this.nudSalary.ThousandsSeparator = true;

            // Buttons
            this.btnSave.Anchor = AnchorStyles.Right;
            this.btnSave.BackColor = Color.FromArgb(40, 167, 69);
            this.btnSave.FlatStyle = FlatStyle.Flat;
            this.btnSave.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular);
            this.btnSave.ForeColor = Color.White;
            this.btnSave.Location = new Point(34, 339); // shifted down due to extra row
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new Size(77, 30);
            this.btnSave.TabIndex = 14;
            this.btnSave.Text = "Lưu";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new EventHandler(this.btnSave_Click);

            this.btnCancel.Anchor = AnchorStyles.Left;
            this.btnCancel.BackColor = Color.FromArgb(108, 117, 125);
            this.btnCancel.FlatStyle = FlatStyle.Flat;
            this.btnCancel.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular);
            this.btnCancel.ForeColor = Color.White;
            this.btnCancel.Location = new Point(117, 339); // shifted down due to extra row
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(77, 30);
            this.btnCancel.TabIndex = 15;
            this.btnCancel.Text = "Hủy";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);

            // 
            // EmployeeUserControl
            // 
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = SystemColors.Control;
            this.Controls.Add(this.dgvEmployees);
            this.Controls.Add(this.pnlForm);
            this.Controls.Add(this.pnlControls);
            this.Name = "EmployeeUserControl";
            this.Size = new Size(1000, 550);
            
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployees)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSalary)).EndInit();
            this.pnlControls.ResumeLayout(false);
            this.pnlForm.ResumeLayout(false);
            this.grpEmployeeInfo.ResumeLayout(false);
            this.tlpForm.ResumeLayout(false);
            this.tlpForm.PerformLayout();
            this.ResumeLayout(false);
        }

        private void LoadRoles()
        {
            try
            {
                var roles = _roleService.GetAllRoles();
                cmbRole.DisplayMember = "Name";
                cmbRole.ValueMember = "Id";
                cmbRole.DataSource = roles;
                cmbRole.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải chức vụ: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadEmployees()
        {
            try
            {
                var employees = _employeeService.GetAllEmployees();
                dgvEmployees.DataSource = employees.Select(e => new
                {
                    ID = e.Id,
                    HọVàTên = e.Name,
                    Email = e.Email,
                    SốĐiệnThoại = e.Phone,
                    ĐịaChỉ = string.IsNullOrWhiteSpace(e.Address) ? "" : e.Address,
                    ChứcVụ = e.Role?.Name ?? "N/A",
                    NgàyVàoLàm = e.HireDate.ToString("dd/MM/yyyy"),
                    Lương = (e.Salary ?? 0).ToString("N0") + " VNĐ",
                    NgàyTạo = e.CreatedAt.ToString("dd/MM/yyyy")
                }).ToList();

                // Format columns
                if (dgvEmployees.Columns.Count > 0)
                {
                    dgvEmployees.Columns["ID"].Width = 50;
                    dgvEmployees.Columns["HọVàTên"].Width = 150;
                    dgvEmployees.Columns["Email"].Width = 150;
                    dgvEmployees.Columns["SốĐiệnThoại"].Width = 100;
                    dgvEmployees.Columns["ĐịaChỉ"].Width = 150;
                    dgvEmployees.Columns["ChứcVụ"].Width = 100;
                    dgvEmployees.Columns["NgàyVàoLàm"].Width = 100;
                    dgvEmployees.Columns["Lương"].Width = 100;
                    dgvEmployees.Columns["NgàyTạo"].Width = 100;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải nhân viên: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvEmployees_SelectionChanged(object sender, EventArgs e)
        {
            bool hasSelection = dgvEmployees.SelectedRows.Count > 0;
            btnEdit.Enabled = hasSelection && !_isEditing;
            btnDelete.Enabled = hasSelection && !_isEditing;

            if (hasSelection && !_isEditing)
            {
                var selectedRow = dgvEmployees.SelectedRows[0];
                var employeeId = (int)selectedRow.Cells["ID"].Value;
                LoadEmployeeForDisplay(employeeId);
            }
        }

        private void LoadEmployeeForDisplay(int employeeId)
        {
            try
            {
                var employee = _employeeService.GetEmployeeById(employeeId);
                if (employee != null)
                {
                    txtFullName.Text = employee.Name;
                    txtEmail.Text = employee.Email;
                    txtPhone.Text = employee.Phone;
                    txtAddress.Text = employee.Address;
                    cmbRole.SelectedValue = employee.RoleId;
                    dtpHireDate.Value = employee.HireDate;
                    nudSalary.Value = employee.Salary ?? 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải thông tin nhân viên: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            _currentEmployee = new Employee();
            _isEditing = true;
            ClearForm();
            SetFormState(true);
            txtFullName.Focus();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvEmployees.SelectedRows.Count == 0) return;

            var selectedRow = dgvEmployees.SelectedRows[0];
            var employeeId = (int)selectedRow.Cells["ID"].Value;
            
            try
            {
                _currentEmployee = _employeeService.GetEmployeeById(employeeId);
                if (_currentEmployee != null)
                {
                    _isEditing = true;
                    LoadEmployeeForm();
                    SetFormState(true);
                    txtFullName.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải thông tin nhân viên: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvEmployees.SelectedRows.Count == 0) return;

            var selectedRow = dgvEmployees.SelectedRows[0];
            var employeeName = selectedRow.Cells["HọVàTên"].Value.ToString();
            var employeeId = (int)selectedRow.Cells["ID"].Value;

            var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa nhân viên '{employeeName}'?", 
                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    if (_employeeService.DeleteEmployee(employeeId))
                    {
                        MessageBox.Show("Xóa nhân viên thành công!", "Thành công", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadEmployees();
                        ClearForm();
                    }
                    else
                    {
                        MessageBox.Show("Không thể xóa nhân viên.", "Lỗi", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa nhân viên: {ex.Message}", "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateForm()) return;

            try
            {
                _currentEmployee.Name = txtFullName.Text.Trim();
                _currentEmployee.Email = txtEmail.Text.Trim();
                _currentEmployee.Phone = txtPhone.Text.Trim();
                _currentEmployee.Address = txtAddress.Text.Trim();
                _currentEmployee.RoleId = (int)cmbRole.SelectedValue;
                _currentEmployee.HireDate = dtpHireDate.Value.Date;
                _currentEmployee.Salary = nudSalary.Value;

                bool success;
                if (_currentEmployee.Id == 0)
                {
                    success = _employeeService.AddEmployee(_currentEmployee);
                    if (success)
                    {
                        MessageBox.Show("Thêm nhân viên thành công!", "Thành công", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    success = _employeeService.UpdateEmployee(_currentEmployee);
                    if (success)
                    {
                        MessageBox.Show("Cập nhật nhân viên thành công!", "Thành công", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                if (success)
                {
                    LoadEmployees();
                    SetFormState(false);
                    _isEditing = false;
                }
                else
                {
                    MessageBox.Show("Không thể lưu nhân viên.", "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu nhân viên: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SetFormState(false);
            _isEditing = false;
            
            // Reload display info if there's a selection
            if (dgvEmployees.SelectedRows.Count > 0)
            {
                var selectedRow = dgvEmployees.SelectedRows[0];
                var employeeId = (int)selectedRow.Cells["ID"].Value;
                LoadEmployeeForDisplay(employeeId);
            }
            else
            {
                ClearForm();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadEmployees();
            LoadRoles();
            if (!_isEditing)
            {
                ClearForm();
            }
        }

        private void SetFormState(bool isEditing)
        {
            txtFullName.ReadOnly = !isEditing;
            txtEmail.ReadOnly = !isEditing;
            txtPhone.ReadOnly = !isEditing;
            txtAddress.ReadOnly = !isEditing;

            cmbRole.Enabled = isEditing;
            dtpHireDate.Enabled = isEditing;
            nudSalary.Enabled = isEditing;
            
            btnSave.Visible = isEditing;
            btnCancel.Visible = isEditing;
            btnAdd.Enabled = !isEditing;
            btnEdit.Enabled = !isEditing && dgvEmployees.SelectedRows.Count > 0;
            btnDelete.Enabled = !isEditing && dgvEmployees.SelectedRows.Count > 0;
            btnRefresh.Enabled = !isEditing;
            
            if (isEditing)
            {
                grpEmployeeInfo.Text = _currentEmployee.Id == 0 ? "Thêm nhân viên mới" : "Chỉnh sửa nhân viên";
                txtFullName.BackColor = SystemColors.Window;
                txtEmail.BackColor = SystemColors.Window;
                txtPhone.BackColor = SystemColors.Window;
                txtAddress.BackColor = SystemColors.Window;

            }
            else
            {
                grpEmployeeInfo.Text = "Thông tin nhân viên";
                txtFullName.BackColor = SystemColors.Control;
                txtEmail.BackColor = SystemColors.Control;
                txtPhone.BackColor = SystemColors.Control;
                txtAddress.BackColor = SystemColors.Control;

            }
        }

        private void ClearForm()
        {
            txtFullName.Text = "";
            txtEmail.Text = "";
            txtPhone.Text = "";
            txtAddress.Text = "";

            cmbRole.SelectedIndex = -1;
            dtpHireDate.Value = DateTime.Today;
            nudSalary.Value = 0;
        }

        private void LoadEmployeeForm()
        {
            if (_currentEmployee != null)
            {
                txtFullName.Text = _currentEmployee.Name;
                txtEmail.Text = _currentEmployee.Email;
                txtPhone.Text = _currentEmployee.Phone;
                txtAddress.Text = _currentEmployee.Address;
                cmbRole.SelectedValue = _currentEmployee.RoleId;
                dtpHireDate.Value = _currentEmployee.HireDate;
                nudSalary.Value = _currentEmployee.Salary ?? 0;
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Vui lòng nhập họ và tên.", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFullName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Vui lòng nhập email.", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }

            if (!IsValidEmail(txtEmail.Text.Trim()))
            {
                MessageBox.Show("Email không hợp lệ.", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại.", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPhone.Focus();
                return false;
            }

            if (cmbRole.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn chức vụ.", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbRole.Focus();
                return false;
            }

            if (nudSalary.Value <= 0)
            {
                MessageBox.Show("Lương phải lớn hơn 0.", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nudSalary.Focus();
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
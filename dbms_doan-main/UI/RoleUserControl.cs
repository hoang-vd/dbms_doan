using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BookstoreDBMS.BLL;
using BookstoreDBMS.Models;

namespace BookstoreDBMS.UI
{
    public partial class RoleUserControl : UserControl
    {
        private RoleService _roleService;
        private Role _currentRole;
        private bool _isEditing = false;

        // Controls
        private DataGridView dgvRoles;
        private TextBox txtName;
        private TextBox txtDescription;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnSave;
        private Button btnCancel;
        private Button btnRefresh;
        private Label lblName;
        private Label lblDescription;
        private Panel pnlControls;
        private Panel pnlForm;
        private GroupBox grpRoleInfo;

        public RoleUserControl()
        {
            InitializeComponent();
            _roleService = new RoleService();
            LoadRoles();
            SetFormState(false);
        }

        private void InitializeComponent()
        {
            this.dgvRoles = new DataGridView();
            this.pnlControls = new Panel();
            this.pnlForm = new Panel();
            this.grpRoleInfo = new GroupBox();
            this.txtName = new TextBox();
            this.txtDescription = new TextBox();
            this.lblName = new Label();
            this.lblDescription = new Label();
            this.btnAdd = new Button();
            this.btnEdit = new Button();
            this.btnDelete = new Button();
            this.btnSave = new Button();
            this.btnCancel = new Button();
            this.btnRefresh = new Button();
            
            ((System.ComponentModel.ISupportInitialize)(this.dgvRoles)).BeginInit();
            this.pnlControls.SuspendLayout();
            this.pnlForm.SuspendLayout();
            this.grpRoleInfo.SuspendLayout();
            this.SuspendLayout();

            // 
            // dgvRoles
            // 
            this.dgvRoles.AllowUserToAddRows = false;
            this.dgvRoles.AllowUserToDeleteRows = false;
            this.dgvRoles.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvRoles.BackgroundColor = SystemColors.Window;
            this.dgvRoles.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRoles.Dock = DockStyle.Fill;
            this.dgvRoles.Location = new Point(0, 0);
            this.dgvRoles.MultiSelect = false;
            this.dgvRoles.Name = "dgvRoles";
            this.dgvRoles.ReadOnly = true;
            this.dgvRoles.RowHeadersWidth = 25;
            this.dgvRoles.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvRoles.Size = new Size(600, 400);
            this.dgvRoles.TabIndex = 0;
            this.dgvRoles.SelectionChanged += new EventHandler(this.dgvRoles_SelectionChanged);

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
            this.pnlControls.Size = new Size(900, 50);
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
            this.pnlForm.Controls.Add(this.grpRoleInfo);
            this.pnlForm.Dock = DockStyle.Right;
            this.pnlForm.Location = new Point(600, 50);
            this.pnlForm.Name = "pnlForm";
            this.pnlForm.Size = new Size(300, 400);
            this.pnlForm.TabIndex = 2;

            // 
            // grpRoleInfo
            // 
            this.grpRoleInfo.Controls.Add(this.btnCancel);
            this.grpRoleInfo.Controls.Add(this.btnSave);
            this.grpRoleInfo.Controls.Add(this.txtDescription);
            this.grpRoleInfo.Controls.Add(this.lblDescription);
            this.grpRoleInfo.Controls.Add(this.txtName);
            this.grpRoleInfo.Controls.Add(this.lblName);
            this.grpRoleInfo.Dock = DockStyle.Fill;
            this.grpRoleInfo.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            this.grpRoleInfo.Location = new Point(0, 0);
            this.grpRoleInfo.Name = "grpRoleInfo";
            this.grpRoleInfo.Padding = new Padding(10);
            this.grpRoleInfo.Size = new Size(300, 400);
            this.grpRoleInfo.TabIndex = 0;
            this.grpRoleInfo.TabStop = false;
            this.grpRoleInfo.Text = "Thông tin chức vụ";

            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular);
            this.lblName.Location = new Point(20, 40);
            this.lblName.Name = "lblName";
            this.lblName.Size = new Size(73, 15);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Tên chức vụ:";

            // 
            // txtName
            // 
            this.txtName.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular);
            this.txtName.Location = new Point(20, 60);
            this.txtName.MaxLength = 100;
            this.txtName.Name = "txtName";
            this.txtName.Size = new Size(250, 21);
            this.txtName.TabIndex = 1;

            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular);
            this.lblDescription.Location = new Point(20, 100);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new Size(48, 15);
            this.lblDescription.TabIndex = 2;
            this.lblDescription.Text = "Mô tả:";

            // 
            // txtDescription
            // 
            this.txtDescription.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular);
            this.txtDescription.Location = new Point(20, 120);
            this.txtDescription.MaxLength = 255;
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ScrollBars = ScrollBars.Vertical;
            this.txtDescription.Size = new Size(250, 80);
            this.txtDescription.TabIndex = 3;

            // 
            // btnSave
            // 
            this.btnSave.BackColor = Color.FromArgb(40, 167, 69);
            this.btnSave.FlatStyle = FlatStyle.Flat;
            this.btnSave.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular);
            this.btnSave.ForeColor = Color.White;
            this.btnSave.Location = new Point(20, 220);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new Size(80, 30);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Lưu";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new EventHandler(this.btnSave_Click);

            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = Color.FromArgb(108, 117, 125);
            this.btnCancel.FlatStyle = FlatStyle.Flat;
            this.btnCancel.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular);
            this.btnCancel.ForeColor = Color.White;
            this.btnCancel.Location = new Point(110, 220);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(80, 30);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Hủy";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);

            // 
            // RoleUserControl
            // 
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = SystemColors.Control;
            this.Controls.Add(this.dgvRoles);
            this.Controls.Add(this.pnlForm);
            this.Controls.Add(this.pnlControls);
            this.Name = "RoleUserControl";
            this.Size = new Size(900, 450);
            
            ((System.ComponentModel.ISupportInitialize)(this.dgvRoles)).EndInit();
            this.pnlControls.ResumeLayout(false);
            this.pnlForm.ResumeLayout(false);
            this.grpRoleInfo.ResumeLayout(false);
            this.grpRoleInfo.PerformLayout();
            this.ResumeLayout(false);
        }

        private void LoadRoles()
        {
            try
            {
                var roles = _roleService.GetAllRoles();
                dgvRoles.DataSource = roles.Select(r => new
                {
                    ID = r.Id,
                    TênChứcVụ = r.Name,
                    MôTả = r.Description,
                    NgàyTạo = r.CreatedAt.ToString("dd/MM/yyyy HH:mm")
                }).ToList();

                // Format columns
                if (dgvRoles.Columns.Count > 0)
                {
                    dgvRoles.Columns["ID"].Width = 50;
                    dgvRoles.Columns["TênChứcVụ"].Width = 150; 
                    dgvRoles.Columns["MôTả"].Width = 200;
                    dgvRoles.Columns["NgàyTạo"].Width = 120;
                    dgvRoles.Columns["NgàyCapNhat"].Width = 120;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải chức vụ: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvRoles_SelectionChanged(object sender, EventArgs e)
        {
            bool hasSelection = dgvRoles.SelectedRows.Count > 0;
            btnEdit.Enabled = hasSelection && !_isEditing;
            btnDelete.Enabled = hasSelection && !_isEditing;

            if (hasSelection && !_isEditing)
            {
                var selectedRow = dgvRoles.SelectedRows[0];
                var roleId = (int)selectedRow.Cells["ID"].Value;
                LoadRoleForDisplay(roleId);
            }
        }

        private void LoadRoleForDisplay(int roleId)
        {
            try
            {
                var role = _roleService.GetRoleById(roleId);
                if (role != null)
                {
                    txtName.Text = role.Name;
                    txtDescription.Text = role.Description ?? "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải thông tin chức vụ: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            _currentRole = new Role();
            _isEditing = true;
            ClearForm();
            SetFormState(true);
            txtName.Focus();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvRoles.SelectedRows.Count == 0) return;

            var selectedRow = dgvRoles.SelectedRows[0];
            var roleId = (int)selectedRow.Cells["ID"].Value;
            
            try
            {
                _currentRole = _roleService.GetRoleById(roleId);
                if (_currentRole != null)
                {
                    _isEditing = true;
                    LoadRoleForm();
                    SetFormState(true);
                    txtName.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải thông tin chức vụ: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvRoles.SelectedRows.Count == 0) return;

            var selectedRow = dgvRoles.SelectedRows[0];
            var roleName = selectedRow.Cells["TênChứcVụ"].Value.ToString();
            var roleId = (int)selectedRow.Cells["ID"].Value;

            var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa chức vụ '{roleName}'?", 
                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    if (_roleService.DeleteRole(roleId))
                    {
                        MessageBox.Show("Xóa chức vụ thành công!", "Thành công", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadRoles();
                        ClearForm();
                    }
                    else
                    {
                        MessageBox.Show("Không thể xóa chức vụ.", "Lỗi", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa chức vụ: {ex.Message}", "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateForm()) return;

            try
            {
                _currentRole.Name = txtName.Text.Trim();
                _currentRole.Description = txtDescription.Text.Trim();

                bool success;
                if (_currentRole.Id == 0)
                {
                    success = _roleService.AddRole(_currentRole);
                    if (success)
                    {
                        MessageBox.Show("Thêm chức vụ thành công!", "Thành công", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    success = _roleService.UpdateRole(_currentRole);
                    if (success)
                    {
                        MessageBox.Show("Cập nhật chức vụ thành công!", "Thành công", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                if (success)
                {
                    LoadRoles();
                    SetFormState(false);
                    _isEditing = false;
                }
                else
                {
                    MessageBox.Show("Không thể lưu chức vụ.", "Lỗi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu chức vụ: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SetFormState(false);
            _isEditing = false;
            
            // Reload display info if there's a selection
            if (dgvRoles.SelectedRows.Count > 0)
            {
                var selectedRow = dgvRoles.SelectedRows[0];
                var roleId = (int)selectedRow.Cells["ID"].Value;
                LoadRoleForDisplay(roleId);
            }
            else
            {
                ClearForm();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadRoles();
            if (!_isEditing)
            {
                ClearForm();
            }
        }

        private void SetFormState(bool isEditing)
        {
            txtName.ReadOnly = !isEditing;
            txtDescription.ReadOnly = !isEditing;
            btnSave.Visible = isEditing;
            btnCancel.Visible = isEditing;
            btnAdd.Enabled = !isEditing;
            btnEdit.Enabled = !isEditing && dgvRoles.SelectedRows.Count > 0;
            btnDelete.Enabled = !isEditing && dgvRoles.SelectedRows.Count > 0;
            btnRefresh.Enabled = !isEditing;
            
            if (isEditing)
            {
                grpRoleInfo.Text = _currentRole.Id == 0 ? "Thêm chức vụ mới" : "Chỉnh sửa chức vụ";
                txtName.BackColor = SystemColors.Window;
                txtDescription.BackColor = SystemColors.Window;
            }
            else
            {
                grpRoleInfo.Text = "Thông tin chức vụ";
                txtName.BackColor = SystemColors.Control;
                txtDescription.BackColor = SystemColors.Control;
            }
        }

        private void ClearForm()
        {
            txtName.Text = "";
            txtDescription.Text = "";
        }

        private void LoadRoleForm()
        {
            if (_currentRole != null)
            {
                txtName.Text = _currentRole.Name;
                txtDescription.Text = _currentRole.Description ?? "";
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên chức vụ.", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }

            if (txtName.Text.Trim().Length > 100)
            {
                MessageBox.Show("Tên chức vụ không được vượt quá 100 ký tự.", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }

            return true;
        }
    }
}
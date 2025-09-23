using System;
using System.Linq;
using System.Windows.Forms;
using BookstoreDBMS.BLL;
using BookstoreDBMS.Models;

namespace BookstoreDBMS.UI
{
    public partial class EmployeeForm : Form
    {
        private EmployeeService _employeeService;
        private RoleService _roleService;
        private Employee _currentEmployee;

        public EmployeeForm()
        {
            InitializeComponent();
            _employeeService = new EmployeeService();
            _roleService = new RoleService();
            LoadEmployees();
            LoadRoles();
        }

        private void InitializeComponent()
        {
            // Similar structure to CategoryForm and BookForm
            // This is a simplified version for brevity
            this.dataGridViewEmployees = new DataGridView();
            this.btnAdd = new Button();
            this.btnEdit = new Button();
            this.btnDelete = new Button();
            this.btnRefresh = new Button();
            this.groupBoxDetails = new GroupBox();
            
            // Form controls
            this.txtName = new TextBox();
            this.txtPhone = new TextBox();
            this.txtEmail = new TextBox();
            this.cmbRole = new ComboBox();
            this.dtpHireDate = new DateTimePicker();
            this.numSalary = new NumericUpDown();
            this.chkIsActive = new CheckBox();
            
            // Labels
            this.lblName = new Label();
            this.lblPhone = new Label();
            this.lblEmail = new Label();
            this.lblRole = new Label();
            this.lblHireDate = new Label();
            this.lblSalary = new Label();
            
            this.btnSave = new Button();
            this.btnCancel = new Button();

            // Set up the form layout similar to other forms
            // ... (layout code similar to other forms)
            
            this.Text = "Employee Management";
            this.Size = new System.Drawing.Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private DataGridView dataGridViewEmployees;
        private Button btnAdd, btnEdit, btnDelete, btnRefresh, btnSave, btnCancel;
        private GroupBox groupBoxDetails;
        private TextBox txtName, txtPhone, txtEmail;
        private ComboBox cmbRole;
        private DateTimePicker dtpHireDate;
        private NumericUpDown numSalary;
        private CheckBox chkIsActive;
        private Label lblName, lblPhone, lblEmail, lblRole, lblHireDate, lblSalary;

        private void LoadEmployees()
        {
            try
            {
                var employees = _employeeService.GetAllEmployees();
                dataGridViewEmployees.DataSource = employees.Select(e => new
                {
                    e.Id,
                    e.Name,
                    e.Phone,
                    e.Email,
                    Role = e.Role?.Name ?? "N/A",
                    HireDate = e.HireDate.ToString("dd/MM/yyyy"),
                    e.Salary,
                    e.IsActive
                }).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading employees: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                MessageBox.Show($"Error loading roles: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Event handlers similar to other forms...
        // btnAdd_Click, btnEdit_Click, btnDelete_Click, btnRefresh_Click, btnSave_Click, btnCancel_Click
    }
}
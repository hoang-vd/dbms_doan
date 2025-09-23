using System;
using System.Linq;
using System.Windows.Forms;
using BookstoreDBMS.BLL;
using BookstoreDBMS.Models;

namespace BookstoreDBMS.UI
{
    public partial class RoleForm : Form
    {
        private RoleService _roleService;
        private Role _currentRole;

        public RoleForm()
        {
            InitializeComponent();
            _roleService = new RoleService();
            LoadRoles();
        }

        private void InitializeComponent()
        {
            // Similar to CategoryForm structure
            this.dataGridViewRoles = new DataGridView();
            this.btnAdd = new Button();
            this.btnEdit = new Button();
            this.btnDelete = new Button();
            this.btnRefresh = new Button();
            this.groupBoxDetails = new GroupBox();
            this.txtName = new TextBox();
            this.txtDescription = new TextBox();
            this.lblName = new Label();
            this.lblDescription = new Label();
            this.btnSave = new Button();
            this.btnCancel = new Button();

            // Set up layout similar to CategoryForm
            this.Text = "Role Management";
            this.Size = new System.Drawing.Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private DataGridView dataGridViewRoles;
        private Button btnAdd, btnEdit, btnDelete, btnRefresh, btnSave, btnCancel;
        private GroupBox groupBoxDetails;
        private TextBox txtName, txtDescription;
        private Label lblName, lblDescription;

        private void LoadRoles()
        {
            try
            {
                var roles = _roleService.GetAllRoles();
                dataGridViewRoles.DataSource = roles.Select(r => new
                {
                    r.Id,
                    r.Name,
                    r.Description,
                    CreatedAt = r.CreatedAt.ToString("dd/MM/yyyy HH:mm")
                }).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading roles: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
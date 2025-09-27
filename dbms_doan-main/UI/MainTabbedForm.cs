using System;
using System.Drawing;
using System.Windows.Forms;
using BookstoreDBMS.DAL;

namespace BookstoreDBMS.UI
{
    public partial class MainTabbedForm : Form
    {
        private TabControl mainTabControl;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
        
        // Tab pages
        private TabPage employeeTabPage;
        private TabPage roleTabPage;
        private TabPage shiftTabPage; // Tab xếp ca
        
        // User Controls for each tab
        private EmployeeUserControl employeeControl;
        private RoleUserControl roleControl;
    private ShiftUserControl shiftControl;

        public MainTabbedForm()
        {
            InitializeComponent();
            CheckDatabaseConnection();
            LoadUserControls();
        }

        private void InitializeComponent()
        {
            this.mainTabControl = new TabControl();
            this.statusStrip = new StatusStrip();
            this.statusLabel = new ToolStripStatusLabel();
            
            // Create tab pages
            this.employeeTabPage = new TabPage("Nhân viên");
            this.roleTabPage = new TabPage("Vai trò");
            this.shiftTabPage = new TabPage("Xếp ca");
            
            this.statusStrip.SuspendLayout();
            this.mainTabControl.SuspendLayout();
            this.SuspendLayout();

            // 
            // mainTabControl
            // 
            this.mainTabControl.Dock = DockStyle.Fill;
            this.mainTabControl.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.mainTabControl.Location = new Point(0, 0);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new Size(1200, 700);
            this.mainTabControl.TabIndex = 0;
            
            // Add tab pages
            this.mainTabControl.TabPages.Add(this.employeeTabPage);
            this.mainTabControl.TabPages.Add(this.roleTabPage);
            this.mainTabControl.TabPages.Add(this.shiftTabPage);

            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new ToolStripItem[] { this.statusLabel });
            this.statusStrip.Location = new Point(0, 678);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new Size(1200, 22);
            this.statusStrip.TabIndex = 1;

            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new Size(39, 17);
            this.statusLabel.Text = "Ready";

            // 
            // MainTabbedForm
            // 
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1200, 700);
            this.Controls.Add(this.mainTabControl);
            this.Controls.Add(this.statusStrip);
            this.MinimumSize = new Size(1024, 600);
            this.Name = "MainTabbedForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Hệ thống quản lý nhà sách - Bookstore Management System";
            this.WindowState = FormWindowState.Maximized;
            
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.mainTabControl.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void LoadUserControls()
        {
            // Initialize and add user controls to tab pages
            employeeControl = new EmployeeUserControl();
            employeeControl.Dock = DockStyle.Fill;
            employeeTabPage.Controls.Add(employeeControl);

            roleControl = new RoleUserControl();
            roleControl.Dock = DockStyle.Fill;
            roleTabPage.Controls.Add(roleControl);
            // Shift scheduling control
            shiftControl = new ShiftUserControl();
            shiftControl.Dock = DockStyle.Fill;
            shiftTabPage.Controls.Add(shiftControl);
        }

        private void CheckDatabaseConnection()
        {
            try
            {
                if (DatabaseHelper.TestConnection())
                {
                    statusLabel.Text = "Kết nối database thành công";
                    statusLabel.ForeColor = Color.Green;
                }
                else
                {
                    statusLabel.Text = "Lỗi kết nối database";
                    statusLabel.ForeColor = Color.Red;
                    MessageBox.Show("Không thể kết nối đến database. Vui lòng kiểm tra connection string.", 
                        "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                statusLabel.Text = "Lỗi database";
                statusLabel.ForeColor = Color.Red;
                MessageBox.Show($"Lỗi kết nối database: {ex.Message}", 
                    "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
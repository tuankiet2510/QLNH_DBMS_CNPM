using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BUS;
using DTO;
namespace GUI
{

    public partial class frmAccount : Form
    {
        LoginBUS loginBUS;
        DataTable dt;
        string role;
        public frmAccount(string role)
        {
            InitializeComponent();
            this.role = role;
            loginBUS = new LoginBUS(role);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            frmAccountAdd frm = new frmAccountAdd(role);
            frm.txtTenTK.ReadOnly = false;
            frm.ShowDialog();
            LoadData();
        }
        void LoadData()
        {
            try
            {
                dt = new DataTable();
                dt.Clear();
                dt = loginBUS.LayTaiKhoan();
                dgvAcc.DataSource = dt;
                dgvAcc.AutoResizeColumns();

            }
            catch (SqlException ex)
            {
                MessageBox.Show("Không lấy được nội dung trong table TAIKHOAN. Lỗi rồi!!!" + ex.Message);
            }
        }

        private void frmAccount_Load(object sender, EventArgs e)
        {
            LoadData(); 
        }

        private void dgvAcc_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvAcc.CurrentCell.OwningColumn.Name == "dgvEdit")
            {
                frmAccountAdd frm = new frmAccountAdd(role);
                frm.txtTenTK.ReadOnly = true;
                frm.cbbTenNV.ValueMember = "ID";
                frm.cbbTenNV.DisplayMember = "Display";
                frm.txtTenTK.Text = dgvAcc.CurrentRow.Cells["dgvTenTK"].Value.ToString();
                if (frm.txtTenTK.Text == "admin")
                {
                    frm.cbbTenNV.Enabled = false;

                }
                else
                {
                    frm.cbbTenNV.Enabled = true;

                }
                frm.txtMK.Text = dgvAcc.CurrentRow.Cells["dgvMatKhau"].Value.ToString();
                frm.cbbTenNV.SelectedValue = dgvAcc.CurrentRow.Cells["dgvMaNV"].Value.ToString().Split('-')[0].Trim();

                frm.ShowDialog();
                if (txtSearchAcc.Text != "")
                {
                    dgvAcc.DataSource = loginBUS.FindAccountByEmployeeID(txtSearchAcc.Text);
                }
                else
                {
                    LoadData();
                }
            }
            else if (dgvAcc.CurrentCell.OwningColumn.Name == "dgvDel")
            {

                if (dgvAcc.CurrentRow.Cells["dgvTenTK"].Value.ToString() == "NVQL")
                {
                    MessageBox.Show("Không thể xoá tài khoản ADMIN");
                    return;
                }
                DialogResult result = MessageBox.Show("Bạn có muốn xoá tài khoản này không?", "Câu hỏi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    if (loginBUS.DeleteAccount(dgvAcc.CurrentRow.Cells["dgvMaNV"].Value.ToString()))
                    {
                        txtSearchAcc.Text = "";
                        LoadData();
                        MessageBox.Show("Xoá thành công!");
                    }
                    else
                    {
                        MessageBox.Show("Xoá không thành công. Lỗi: '" + loginBUS.err + "'");
                    }
                }
            }
        }
    }
}

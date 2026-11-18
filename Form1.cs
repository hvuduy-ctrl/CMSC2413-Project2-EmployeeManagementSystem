
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace EmployeeManagementSystem
{
    public partial class Form1 : Form
    {
        private BindingList<Employee> employees =
            new BindingList<Employee>();

        public Form1()
        {
            InitializeComponent();

            cmbDepartment.Items.AddRange(
                new object[] { "IT", "HR", "Sales", "Finance" });
            cmbDepartment.DropDownStyle = ComboBoxStyle.DropDownList;

            dgvEmployees.DataSource = employees;
            dgvEmployees.ReadOnly = true;
            dgvEmployees.AllowUserToAddRows = false;
            dgvEmployees.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvEmployees.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            dgvEmployees.MultiSelect = false;

            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
            btnSearch.Click += BtnSearch_Click;
            btnClear.Click += BtnClear_Click;
            dgvEmployees.CellClick += Grid_CellClick;

            ClearFields();
            UpdateSummary();
        }

        // Check employee information.
        private bool ReadEmployee(out Employee emp)
        {
            emp = null;
            int id;
            decimal salary, bonus;

            if (!int.TryParse(txtId.Text, out id) || id <= 0)
            {
                MessageBox.Show("Invalid employee ID.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                cmbDepartment.SelectedItem == null)
            {
                MessageBox.Show("Enter name and department.");
                return false;
            }

            if (!decimal.TryParse(txtSalary.Text, out salary) ||
                !decimal.TryParse(txtBonus.Text, out bonus) ||
                salary < 0 || bonus < 0)
            {
                MessageBox.Show("Invalid salary or bonus.");
                return false;
            }

            emp = new Employee
            {
                Id = id,
                Name = txtName.Text.Trim(),
                Department = cmbDepartment.Text,
                Salary = salary,
                Bonus = bonus
            };
            return true;
        }

        // Add employee.
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            Employee emp;
            if (!ReadEmployee(out emp)) return;

            if (employees.Any(x => x.Id == emp.Id))
            {
                MessageBox.Show("Employee ID already exists.");
                return;
            }

            employees.Add(emp);
            UpdateSummary();
            ClearFields();
            MessageBox.Show("Employee added!");
        }

        // Update employee.
        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            Employee selected = GetSelected();
            if (selected == null)
            {
                MessageBox.Show("Select an employee first.");
                return;
            }

            Employee updated;
            if (!ReadEmployee(out updated)) return;

            if (employees.Any(x =>
                x != selected && x.Id == updated.Id))
            {
                MessageBox.Show("Employee ID already exists.");
                return;
            }

            selected.Id = updated.Id;
            selected.Name = updated.Name;
            selected.Department = updated.Department;
            selected.Salary = updated.Salary;
            selected.Bonus = updated.Bonus;

            employees.ResetBindings();
            UpdateSummary();
            ClearFields();
            MessageBox.Show("Employee updated!");
        }

        // Delete employee.
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            Employee selected = GetSelected();
            if (selected == null)
            {
                MessageBox.Show("Select an employee first.");
                return;
            }

            if (MessageBox.Show("Delete this employee?",
                "Confirm", MessageBoxButtons.YesNo)
                == DialogResult.Yes)
            {
                employees.Remove(selected);
                UpdateSummary();
                ClearFields();
            }
        }

        // Search by ID or name.
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();

            if (keyword == "")
            {
                MessageBox.Show("Enter a search keyword.");
                return;
            }

            foreach (DataGridViewRow row in dgvEmployees.Rows)
            {
                Employee emp = row.DataBoundItem as Employee;

                if (emp != null &&
                    (emp.Id.ToString() == keyword ||
                     emp.Name.IndexOf(keyword,
                         StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    dgvEmployees.ClearSelection();
                    dgvEmployees.CurrentCell = row.Cells[0];
                    row.Selected = true;
                    ShowEmployee(emp);
                    return;
                }
            }

            MessageBox.Show("Employee not found.");
        }

        // Display selected employee.
        private void Grid_CellClick(
            object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            Employee emp = dgvEmployees.Rows[e.RowIndex]
                .DataBoundItem as Employee;

            if (emp != null) ShowEmployee(emp);
        }

        private Employee GetSelected()
        {
            if (dgvEmployees.SelectedRows.Count == 0)
                return null;

            return dgvEmployees.SelectedRows[0]
                .DataBoundItem as Employee;
        }

        private void ShowEmployee(Employee emp)
        {
            txtId.Text = emp.Id.ToString();
            txtName.Text = emp.Name;
            cmbDepartment.Text = emp.Department;
            txtSalary.Text = emp.Salary.ToString();
            txtBonus.Text = emp.Bonus.ToString();
        }

        // Clear all fields.
        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            txtId.Clear();
            txtName.Clear();
            txtSalary.Clear();
            txtBonus.Clear();
            txtSearch.Clear();
            cmbDepartment.SelectedIndex = -1;
            dgvEmployees.ClearSelection();
        }

        // Update number of employees and total payroll.
        private void UpdateSummary()
        {
            lblSummary.Text = "Employees: " + employees.Count +
                " | Total payroll: " +
                employees.Sum(x => x.TotalSalary).ToString("N2");
        }
    }
}

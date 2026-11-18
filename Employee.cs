
using System;

namespace EmployeeManagementSystem
{
    public class Employee
    {
        private decimal salary;
        private decimal bonus;

        public int Id { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }

        public decimal Salary
        {
            get { return salary; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Invalid salary.");
                salary = value;
            }
        }

        public decimal Bonus
        {
            get { return bonus; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Invalid bonus.");
                bonus = value;
            }
        }

        public decimal TotalSalary
        {
            get { return Salary + Bonus; }
        }
    }
}

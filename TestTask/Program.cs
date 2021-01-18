using System;
using System.Collections.Generic;

namespace coding_excercise
{
    class MonthlyContractAdjustment : IAdjustment
    {
        private readonly TimeSpan _probationPeriod = new TimeSpan(30, 0, 0, 0);

        public void Adjust(Employee employee)
        {
            if (DateTime.Now - employee.StartDate >= _probationPeriod)
                employee.EndTempPeriod();
        }
    }

    class VacationAdjustment : IAdjustment
    {
        public void Adjust(Employee employee) => employee.AdjustVacation(2);
    }

    class EndOfYearSalaryAdjustment : IAdjustment
    {
        public void Adjust(Employee employee) => employee.AdjustSalary(1.1);
    }

    abstract class Employee
    {
        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public bool IsTerminated { get; protected set; }

        public bool IsProbationOver { get; set; }

        public void EndTempPeriod()
        {
            IsProbationOver = true;
        }
        public abstract void AdjustVacation(int additionalDays);

        public abstract void AdjustSalary(double adjustmentFactor);
    }

    abstract class Contractor : Employee
    {
        public Contractor(double hourlyRate) : base()
        {
            HourlyRate = hourlyRate;
            IsProbationOver = false;
        }

        public double HourlyRate { get; set; }

        public override void AdjustVacation(int additionalDays) { return; }

        public override void AdjustSalary(double adjustmentFactor) => HourlyRate *= adjustmentFactor;
    }

    abstract class FullTime : Employee
    {
        public FullTime(double yearlySalary)
        {
            YearlySalary *= 0.75;
            VacationDays = 0;
            IsProbationOver = false;
        }

        public double YearlySalary { get; set; }

        public int VacationDays { get; set; }

        public override void AdjustVacation(int additionalDays) => VacationDays += additionalDays;

        public override void AdjustSalary(double adjustmentFactor) => YearlySalary *= adjustmentFactor;
    }

    class CopyWriter : FullTime
    {
        public CopyWriter(string name) : base(25000.0) => Name = name;
    }

    class Manager : FullTime
    {
        public Manager(string name) : base(45000.0) => Name = name;
    }

    class Developer : Contractor
    {
        public Developer(string name) : base(250000.0) => Name = name;
    }

    class Company
    {
        private List<Employee> _employees = new List<Employee>();

        public void AcceptAdjustments(List<IAdjustment> adjustments)
            => _employees.ForEach(employee => { adjustments.ForEach(adjustment => adjustment.Adjust(employee)); });

        public void AddEmployee(Employee employee) => _employees.Add(employee);

        public void RemoveEmployee(Employee employee) => _employees.Remove(employee);
    }

    interface IAdjustment
    {
        void Adjust(Employee employee);
    }

    class MainApp
    {
        static void Main()
        {
            //  Load from database
            var company = new Company();
            company.AddEmployee(new Manager("Greg"));
            company.AddEmployee(new CopyWriter("Ellen"));
            company.AddEmployee(new Developer("Susie"));

            var adjustments = new List<IAdjustment>()
            {
                new EndOfYearSalaryAdjustment(),
                new MonthlyContractAdjustment(),
                new VacationAdjustment()
            };

            //  Perform adjustments
            company.AcceptAdjustments(adjustments);

            /*
                ...save to database
            */
        }
    }
}
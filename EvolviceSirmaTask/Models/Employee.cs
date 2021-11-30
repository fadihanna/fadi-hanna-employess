using System;

namespace EvolviceSirmaTask.Models
{
    public class Employee
    {
        public string EmployeeID { get; set; }
        public string ProjectID { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
    public class Result
    {
        public string EmployeeID1 { get; set; }
        public string EmployeeID2 { get; set; }
        public string ProjectID { get; set; }
        public int Days { get; set; }
        public int Index { get; set; }
    }
}
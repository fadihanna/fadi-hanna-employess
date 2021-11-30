using EvolviceSirmaTask.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EvolviceSirmaTask.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Calculate(HttpPostedFileBase EmployeesFile)
        {
            /*
             Note that we could use benefit of file format "(101,10,2015,2016)" to set it into the sql insert query
            INSERT INTO #Employee VALUES (" + "reader.Read()" + ") ..
            and complete in sql join
             */
            var employeesList = new List<Employee>();
            if (EmployeesFile != null && EmployeesFile.ContentLength > 0)
                try
                {
                    var lines = new List<string>();
                    using (StreamReader reader = new StreamReader(EmployeesFile.InputStream))
                    {
                        //here we read the file inserted to fill employee type to (deal with dates, replace nulls, use employees list in our functionality then)
                        do
                        {
                            string textLine = reader.ReadLine();
                            var recordList = textLine.Trim().Split(',');
                            employeesList.Add(new Employee()
                            {
                                EmployeeID = recordList[0],
                                ProjectID = recordList[1],
                                DateFrom = Convert.ToDateTime(recordList[2], System.Threading.Thread.CurrentThread.CurrentCulture).Date,
                                DateTo = Convert.ToDateTime(recordList[3].Replace("NULL", DateTime.Today.ToString()), System.Threading.Thread.CurrentThread.CurrentCulture).Date    //here we replace every null string to today's date
                            });
                        } while (reader.Peek() != -1);

                        //make self join for our list 
                        var result = (from emp1 in employeesList
                                      join emp2 in employeesList on
                                      emp1.ProjectID equals emp2.ProjectID      //employees sharing same project
                                      where int.Parse(emp2.EmployeeID) > int.Parse(emp1.EmployeeID) &&      //to not select emp with him self or >> empId: 1, empId: 2 twice by swapping them, (1,2 then 2,1)
                                      (emp1.DateFrom < emp1.DateTo) && (emp2.DateFrom < emp2.DateTo) &&     //exclude wrong dates: datefrom  greater than dateto 
                                      emp1.DateFrom < emp2.DateTo && emp2.DateFrom < emp1.DateTo            //check if two records dates are basically overlapped 
                                      select new Result
                                      {
                                          EmployeeID1 = emp1.EmployeeID,
                                          EmployeeID2 = emp2.EmployeeID,
                                          ProjectID = emp1.ProjectID,
                                          Days = CalculateDays(emp1, emp2)  //calculate number of days between two dates
                                      }).OrderByDescending(i => i.Days);        //ordering then get greatest number help us get the most working days, here i retrieved all ordered so we have full information
                        return Json(new { data = Newtonsoft.Json.JsonConvert.SerializeObject(result) });
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            else
            {
                throw new Exception();
            }
        }
        /// <summary>
        /// used to calculate days between two different valid dates
        /// </summary>
        /// <param name="employee1"></param>
        /// <param name="employee2"></param>
        /// <returns></returns>
        public int CalculateDays(Employee employee1, Employee employee2)
        {
            var dateFrom = employee1.DateFrom > employee2.DateFrom ? employee1.DateFrom : employee2.DateFrom;
            var dateTo = employee1.DateTo < employee2.DateTo ? employee1.DateTo : employee2.DateTo;
            return dateTo.Subtract(dateFrom).Days;
        }
    }
}
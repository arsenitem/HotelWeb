using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelWebApplication.Models;

namespace HotelWebApplication.Controllers
{
    public class EmployController : Controller
    {
        // GET: Employ
        public ActionResult Index()
        {
            HotelDBEntities db = new HotelDBEntities();
            var res = from Сотрудники in db.Сотрудники
                      join Должность in db.Должность on Сотрудники.ID_должности equals Должность.ID_должности
                      select new 
                        {
                          emp_id=Сотрудники.ID_сотрудника,
                          emp_fio=Сотрудники.Имя,
                          emp_phone=Сотрудники.Телефон,
                          emp_positin=Должность.Название,
                          emp_salary=Должность.Оклад
                      };
            List<string[]> emplist = new List<string[]>();
            foreach(var r in res)
            {
                emplist.Add(new string[] { r.emp_id.ToString(),r.emp_fio,r.emp_phone,r.emp_positin,r.emp_salary.ToString()});
            }

            return View(emplist);
        }
        public ActionResult AddEmployeeView()
        {
            return PartialView();
        }
        [HttpPost]
        public void RemoveWorker(int id)
        {
           using(HotelDBEntities db=new HotelDBEntities())
            {
                var res = db.Сотрудники.Where(x => x.ID_сотрудника == id).FirstOrDefault();
                db.Сотрудники.Remove(res);
                db.SaveChanges();
            }
        }
        [HttpPost]
        public ActionResult AddEmployee(string fio,string position,string phone)
        {
            using(HotelDBEntities db=new HotelDBEntities())
            {
                //var id = db.Должность.Where(x => x.Название == position).Select(x => x.ID_должности).FirstOrDefault();
                Сотрудники emp = new Сотрудники();
                emp.Имя = fio;
                emp.Телефон = phone;
                emp.ID_должности = Convert.ToInt32(position);
                db.Сотрудники.Add(emp);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

    }
    
}
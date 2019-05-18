using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotelWebApplication.Models;

namespace HotelWebApplication.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            List<string[]> recordslist = new List<string[]>();
            using (HotelDBEntities db = new HotelDBEntities())
            {


                var res = from Учет in db.Учет
                          join Номера in db.Номера on Учет.ID_Номера equals Номера.ID
                          join Операции in db.Операции on Учет.Операция equals Операции.ID_Операц
                          join Клиенты in db.Клиенты on Учет.ID_Клиента equals Клиенты.ID_клиента
                          join Сотрудники in db.Сотрудники on Учет.Сотрудник equals Сотрудники.ID_сотрудника
                          join Категория_номера in db.Категория_номера on Номера.Тип equals Категория_номера.ID_типа
                          select new
                          {
                              room_id = Номера.ID,
                              room_type = Категория_номера.Название,
                              room_size = Номера.Вместимость,
                              room_price = Номера.Цена,
                              client_fio = Клиенты.ФИО,
                              begin_date = Учет.Дата_въезда,
                              end_date = Учет.Дата_выезда,
                              order_type = Операции.Название
                          };
                foreach (var r in res)
                {
                    double total_price = Convert.ToDateTime(r.end_date).Subtract(Convert.ToDateTime(r.begin_date)).Days * r.room_price;
                    recordslist.Add(new string[] { r.room_id.ToString(), r.room_type.ToString(), r.room_size.ToString(), r.room_price.ToString(), total_price.ToString(), r.client_fio, r.begin_date.ToString(), r.end_date.ToString(), r.order_type });
                }
            }
            return View(recordslist);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Rooms()
        {
            List<string[]> roomslist = new List<string[]>();
            using(HotelDBEntities db=new HotelDBEntities())
            {
                var res = from Номера in db.Номера
                          join Категория_номера in db.Категория_номера on Номера.Тип equals Категория_номера.ID_типа
                          select new
                          {
                              room_id = Номера.ID,
                              room_type = Категория_номера.Название,
                              room_size = Номера.Вместимость,
                              room_price = Номера.Цена
                          };
                foreach(var r in res)
                {
                    roomslist.Add(new string[] { r.room_id.ToString(), r.room_type, r.room_size.ToString(), r.room_price.ToString() });
                }
            }
            return View(roomslist);
        }
        public ActionResult NewOrder()
        {
            
            using (HotelDBEntities db = new HotelDBEntities())
            {
                var rooms = db.Номера.Select(x => x.ID);
                var emp = db.Сотрудники.Select(x => x.Имя);
                List<string> roomslist = new List<string>();
                List<string> emplist = new List<string>();
                foreach (var r in rooms)
                {
                    roomslist.Add(r.ToString());
                }
                foreach (var r in emp)
                {
                    emplist.Add(r.ToString());
                }
                ViewBag.Rooms = roomslist;
                ViewBag.Emp = emplist;

            }
          
                return View();
        }
        [HttpPost]
        public ActionResult NewOrder(string passport,string fio,string phone,string room_num,string order_type,string begin_date,string end_date,string emp)
        {
            try
            {
                using (HotelDBEntities db = new HotelDBEntities())
                {

                    Клиенты client = new Клиенты();
                    client.Паспорт = passport;
                    client.ФИО = fio;
                    client.Телефон = phone;
                    db.Клиенты.Add(client);
                    db.SaveChanges();
                    var clinet_id = db.Клиенты.Where(x => x.Паспорт == passport).Select(x => x.ID_клиента).FirstOrDefault();
                    Учет record = new Учет();
                    record.ID_Клиента = clinet_id;
                    record.ID_Номера = Convert.ToInt32(room_num) + 1;
                    record.Операция = Convert.ToInt32(order_type);
                    record.Дата_въезда = Convert.ToDateTime(begin_date);
                    record.Дата_выезда = Convert.ToDateTime(end_date);
                    var emp_id = db.Сотрудники.Where(x => x.Имя == emp).Select(x => x.ID_сотрудника).FirstOrDefault();
                    record.Сотрудник = emp_id;
                    int room_id = Convert.ToInt32(room_num) + 1;
                    var room_price = db.Номера.Where(x => x.ID == room_id).Select(x => x.Цена).FirstOrDefault();
                    record.Цена = Convert.ToDateTime(end_date).Subtract(Convert.ToDateTime(begin_date)).Days * room_price;
                    db.Учет.Add(record);
                    db.SaveChanges();
                }
            }
            catch
            {
                return RedirectToAction("Index");
            }
           
            return RedirectToAction("Index");
        }


        public ActionResult AddEmployeeView()
        {
            ViewBag.Adding = true;
            return PartialView();
        }
        public ActionResult Workers()
        {
            return View();
        }
        public ActionResult AddRoomView()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult AddRoom(string room_type,string room_size, string room_price)
        {
            using(HotelDBEntities db=new HotelDBEntities())
            {
                Номера room = new Номера();
                room.Тип = Convert.ToInt32(room_type);
                room.Вместимость =Convert.ToInt32( room_size);
                room.Цена = Convert.ToDouble(room_price);
                db.Номера.Add(room);
                db.SaveChanges();
            }
            return RedirectToAction("Rooms");
        }
        [HttpPost]
        public void RemoveRoom(int id)
        {
            using (HotelDBEntities db = new HotelDBEntities())
            {
                var res = db.Номера.Where(x => x.ID == id).FirstOrDefault();
                db.Номера.Remove(res);
                db.SaveChanges();
            }
        }
        [HttpPost]
        public ActionResult Find(string client_fio)
        {
            try
            {
                using (HotelDBEntities db = new HotelDBEntities())
                {
                    var guest_id = db.Клиенты.Where(x => x.ФИО == client_fio).Select(x => x.ID_клиента).FirstOrDefault();

                    var res = db.Учет.Where(x => x.ID_Клиента == guest_id).Select(x => x.ID_Номера).FirstOrDefault();
                    ViewBag.Res = true;
                    ViewBag.Num = res;
                    return PartialView();
                }
            }
            catch
            {
                ViewBag.Res = false;
                return PartialView();
            }

        }
    }
   
}
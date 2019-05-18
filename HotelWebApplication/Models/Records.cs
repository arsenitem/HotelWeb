using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelWebApplication.Models
{
    public class Records
    {
        public int room_id;
        public string room_type;
        public int room_size;
        public double room_price;
        public double total_price;
        public string client_fio;
        public DateTime begin_date;
        public DateTime end_date;
        public string order_type;
    }
  
}
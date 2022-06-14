using System;
using System.Globalization;

namespace Workshop.ServiceLayer.ServiceObjects
{
    public class SystemAdminDTO
    {
        public string Membername { get; set; }
        public string Password { get; set; }
        public DateTime Birthdate { get; set; }
        public SystemAdminDTO(string name, string pass, string date)
        {
            Membername = name;
            Password = pass;
            Birthdate = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        }
    }
}

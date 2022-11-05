using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OTP.Ring.Data.Models;

namespace OTP.Ring.Data
{
    public class DataAccess
    {
        private RingEntities _database;

        public DataAccess()
        {
            _database = new RingEntities();
        }

        public List<Sport> GetAllSports()
        {
            return _database.Sports.ToList();
        }
    }
}

using SIGD.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIGD.Services
{
    // TODO create methods to use ApplicationDbContext outside controllers
    public class DatabaseService
    {
        private readonly ApplicationDbContext _context;
        public T Save<T>(T y)
        {
            
            using (var context = new ApplicationDbContext())
            {
                context.Add(y);
               _context.SaveChanges();
            }

            T t = y;

            return t;
        }
    }
}

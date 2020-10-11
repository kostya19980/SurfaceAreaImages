using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data.Entity;
using System.Threading.Tasks;
using S.Models;

namespace S.Models
{
    public class BaseContext : DbContext
    {
        public BaseContext() :
            base("DefaultConnection")
        {

        }
        public DbSet<Picture> Image { get; set; }
        
    }
}
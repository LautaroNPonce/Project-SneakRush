using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Conexion486LP
    {
        public static string BD = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SneakRushDB;Integrated Security=True";

        // Si necesitás apuntar a otro servidor, comentá la de arriba y descomentá la que uses:
        //public static string BD = @"Data Source=.;Initial Catalog=SneakRushDB;Integrated Security=True";
        //public static string BD = @"Data Source=NOMBRE-PC\SQLEXPRESS;Initial Catalog=SneakRushDB;Integrated Security=True";
    }
}

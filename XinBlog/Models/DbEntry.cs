using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace System.Data
{
    public class DbEntry
    {
        public static IDbConnection MySqlDb()
        {
            return new MySql.Data.MySqlClient.MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlServer"].ToString());
        }
    }
}
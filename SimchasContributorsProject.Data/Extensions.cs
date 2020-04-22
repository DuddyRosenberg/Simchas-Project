using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace SimchasContributorsProject.Data
{
    public static class Extensions
    {
        public static T GEtOrNull<T>(this SqlDataReader reader, string column)
        {
            var obj = reader[column];
            if (obj == DBNull.Value)
            {
                return default(T);
            }
            return (T)obj;
        }
    }
}

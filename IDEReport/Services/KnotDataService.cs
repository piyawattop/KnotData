using Dapper;
using IDEReport.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDEReport.Services
{
    public class KnotDataService
    {
        private string connectionString = "";
        public KnotDataService(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public bool IsValidConnection()
        {
            var isOpen = false;
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    isOpen = true;
                    conn.Close();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return isOpen;
        }
        public List<DataKnotViewModel> Search(DateTime dateStart, DateTime dateEnd, string barcode = "", string model = "")
        {
            List<DataKnotViewModel> result;

            using (var conn = new SqlConnection(connectionString))
            {
                var sql = @"SELECT TransDateTime 
                            , Barcode
                            , Model
                            , AmbientSensor 
                            , Upperfoam
                            , LHFoam
                            , RHFoam
                            , AdaptorCord
                            , Torque1
                            , Angle1
                            , Code1
                            , Torque2
                            , Angle2
                            , Code2
                            , Torque3
                            , Angle3
                            , Code3
                            , Torque4
                            , Angle4
                            , Code4
                            , Torque5
                            , Angle5
                            , Code5
                            , Torque6
                            , Angle6
                            , Code6
                    FROM DataKnot 
                    WHERE (TransDateTime between @dateStart and @dateEnd) ";
                if (!string.IsNullOrEmpty(barcode))
                {
                    barcode = "%" + barcode + "%";
                    sql += " AND Barcode = @barcode ";
                }
                if (!string.IsNullOrEmpty(model))
                {
                    sql += " AND Model = @model ";
                }
                result = conn.Query<DataKnotViewModel>(sql, new { dateStart, dateEnd, barcode, model }).ToList();
                return result;
            }
        }

    }
}

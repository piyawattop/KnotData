using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnotBackgroundService.Services
{
    public class FusionDataService
    {
        private string connectionString = "";
        public FusionDataService(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public string GetDataStepSubResult(string DataStepUID)
        {
            using (var _connection = new SqlConnection(connectionString))
            {
                return _connection.Query<string>("SELECT ResultData FROM DataStepSub where  ResultData is not null and DataStepUID = @DataStepUID",
                    new { DataStepUID }).FirstOrDefault();
            }
        }
    }
}

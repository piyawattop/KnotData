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
        public string GetStepOutComeType(string CycleID, string StepName)
        {
            using (var _connection = new SqlConnection(connectionString))
            {
                return _connection.Query<string>("SELECT OutcomeType FROM DataStep where CycleID = @CycleID AND StepName = @StepName",
                    new { CycleID, StepName }).FirstOrDefault();
            }
        }
        public string GetModel(string CycleID)
        {
            using (var _connection = new SqlConnection(connectionString))
            {
                return _connection.Query<string>("SELECT TOP 1 REPLACE(StepName,'JUMP ','') FROM DataStep where CycleID = @CycleID AND StepName LIKE 'JUMP MODEL%'",
                    new { CycleID }).FirstOrDefault();
            }
        }

    }
}

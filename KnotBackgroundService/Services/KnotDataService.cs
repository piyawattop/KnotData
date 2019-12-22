using Dapper;
using KnotBackgroundService.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnotBackgroundService.Services
{
    public class KnotDataService
    {
        private string connectionString = "";
        public KnotDataService(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<DataKnotViewModel> GetDataToExport(string fileName)
        {
            using (var _connection = new SqlConnection(connectionString))
            {
                _connection.Execute("UPDATE DataKnot SET ExportedToFile = @fileName WHERE ExportedToFile IS NULL", new { fileName });
                return _connection.Query<DataKnotViewModel>(@"SELECT TransDateTime 
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
                                                    , Code6 FROM DataKnot WHERE ExportedToFile = @fileName",
                    new { fileName }).ToList();
            }
        }
        public void UpdateErrorExportFile(string fileName)
        {
            using (var _connection = new SqlConnection(connectionString))
            {
                _connection.Execute("UPDATE DataKnot SET ExportedToFile = NULL WHERE ExportedToFile = @fileName", new { fileName });
            }
        }
        public int? InsertKnotData(DataKnot currentKnotData)
        {
            using (var _connection = new SqlConnection(connectionString))
            {
                var insertCmd = @"INSERT INTO DataKnot
           (CycleID
           ,DataStepUID
           ,Barcode
           ,Model
           ,AmbientSensor
           ,Upperfoam
           ,LHFoam
           ,RHFoam
           ,AdaptorCord
           ,Torque1
           ,Angle1
           ,Code1
           ,Torque2
           ,Angle2
           ,Code2
           ,Torque3
           ,Angle3
           ,Code3
           ,Torque4
           ,Angle4
           ,Code4
           ,Torque5
           ,Angle5
           ,Code5
           ,Torque6
           ,Angle6
           ,Code6
           ,TransDateTime)
     VALUES
           (@CycleID
           ,@DataStepUID
           ,@Barcode
           ,@Model
           ,@AmbientSensor
           ,@Upperfoam
           ,@LHFoam
           ,@RHFoam
           ,@AdaptorCord
           ,@Torque1
           ,@Angle1
           ,@Code1
           ,@Torque2
           ,@Angle2
           ,@Code2
           ,@Torque3
           ,@Angle3
           ,@Code3
           ,@Torque4
           ,@Angle4
           ,@Code4
           ,@Torque5
           ,@Angle5
           ,@Code5
           ,@Torque6
           ,@Angle6
           ,@Code6
           ,@TransDateTime)";
                return _connection.Execute(insertCmd, currentKnotData);
            }
        }
    }
}

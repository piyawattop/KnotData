# KnotData

Using .NetFW 4.7.2 (https://dotnet.microsoft.com/download/visual-studio-sdks?utm_source=getdotnetsdk&utm_medium=referral)

USE Master
ALTER DATABASE FusionCore SET DISABLE_BROKER;
Go;

--- Create new Database and add new table
CREATE TABLE DataKnot  
(  
 DataKnotUID uniqueidentifier NOT NULL DEFAULT newid(),  
 CycleID nvarchar(50),  
 DataStepUID nvarchar(50),  
 Barcode nvarchar(512),  
 Model nvarchar(50),  
 AmbientSensor nvarchar(50),  
 Upperfoam nvarchar(50),  
 LHFoam nvarchar(50),  
 RHFoam nvarchar(50),  
 AdaptorCord nvarchar(50),  
 Torque1 FLOAT,  
 Angle1 FLOAT,  
 Code1 nvarchar(50),
 Torque2 FLOAT,  
 Angle2 FLOAT,  
 Code2 nvarchar(50),
 Torque3 FLOAT,  
 Angle3 FLOAT,  
 Code3 nvarchar(50),
 Torque4 FLOAT,  
 Angle4 FLOAT,  
 Code4 nvarchar(50),
 Torque5 FLOAT,  
 Angle5 FLOAT,  
 Code5 nvarchar(50),
 Torque6 FLOAT,  
 Angle6 FLOAT,  
 Code6 nvarchar(50),
 ExportedToFile nvarchar(512),  
 TransDateTime Datetime DEFAULT GETDATE()
);  
GO  

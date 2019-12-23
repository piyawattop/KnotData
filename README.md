# KnotData Service

### Prerequisites Using .NetFW 4.7.2 
If you dont have download at https://dotnet.microsoft.com/download/visual-studio-sdks?utm_source=getdotnetsdk&utm_medium=referral

### Enable Database Broker service
```
USE Master
GO;

ALTER DATABASE FusionCore SET DISABLE_BROKER;
Go;
```

### Create new Database and add new table
```
CREATE DATABASE KnotData
GO;

USE KnotData
GO; 

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
```

### How to install Service
1.Open CMD with Administrator right.
![cmd](https://clssp20.blob.core.windows.net/solconfig/1CMD.png)
2.Go to release folder and use InstallService.bat
![install](https://clssp20.blob.core.windows.net/solconfig/2InstallService.png)
Done...

### Setting in service
![appsetting](https://clssp20.blob.core.windows.net/solconfig/AppSetting.png)
1. COMPORT
2. FusionConnectionString
3. KnotConnectionString
4. ExportPath
5. ExportFileType allow only csv,xls,xlsx
6. Mode to let service schedule export file allow only Daily and Interval
7. IntervalMinutes set every xx minute 
8. ScheduledTime set for every day time ex. 18.30


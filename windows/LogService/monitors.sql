USE [master]
GO
/****** Object:  Database [Monitors]    Script Date: 2016/2/23 23:59:01 ******/
CREATE DATABASE [Monitors]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Monitors', FILENAME = N'E:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\Monitors.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'Monitors_log', FILENAME = N'E:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\Monitors_log.ldf' , SIZE = 2304KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [Monitors] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Monitors].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Monitors] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Monitors] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Monitors] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Monitors] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Monitors] SET ARITHABORT OFF 
GO
ALTER DATABASE [Monitors] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Monitors] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Monitors] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Monitors] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Monitors] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Monitors] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Monitors] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Monitors] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Monitors] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Monitors] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Monitors] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Monitors] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Monitors] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Monitors] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Monitors] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Monitors] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Monitors] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Monitors] SET RECOVERY FULL 
GO
ALTER DATABASE [Monitors] SET  MULTI_USER 
GO
ALTER DATABASE [Monitors] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Monitors] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Monitors] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Monitors] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [Monitors] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'Monitors', N'ON'
GO
USE [Monitors]
GO
/****** Object:  Table [dbo].[LogMinute]    Script Date: 2016/2/23 23:59:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[LogMinute](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[MonitorAction] [varchar](32) NULL,
	[MonitorTime] [datetime] NULL,
	[Host] [varchar](64) NULL,
	[IP] [varchar](64) NULL,
	[MAC] [varchar](64) NULL,
	[AppPath] [varchar](256) NULL,
	[ProcessName] [varchar](128) NULL,
	[AppName] [varchar](128) NULL,
	[FirstCag] [varchar](128) NULL,
	[SecondCag] [varchar](128) NULL,
	[ThirdCag] [varchar](128) NULL,
	[MonitorValue] [decimal](18, 0) NULL,
	[Total] [int] NOT NULL CONSTRAINT [DF__LogMinute__Total__1A14E395]  DEFAULT ((0)),
	[ForthCag] [varchar](128) NULL,
	[FifthCag] [varchar](128) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Logs]    Script Date: 2016/2/23 23:59:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Logs](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[MonitorAction] [varchar](32) NULL,
	[MonitorTime] [datetime] NULL,
	[Host] [varchar](64) NULL,
	[IP] [varchar](64) NULL,
	[MAC] [varchar](64) NULL,
	[AppPath] [varchar](256) NULL,
	[AppName] [varchar](128) NULL,
	[ProcessName] [varchar](128) NULL,
	[FirstCag] [varchar](128) NULL,
	[SecondCag] [varchar](128) NULL,
	[ThirdCag] [varchar](128) NULL,
	[MonitorValue] [decimal](18, 0) NULL,
	[Msg] [nvarchar](512) NULL,
	[ForthCag] [varchar](128) NULL,
	[FifthCag] [varchar](128) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[LogSecond]    Script Date: 2016/2/23 23:59:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[LogSecond](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[MonitorAction] [varchar](32) NULL,
	[MonitorTime] [datetime] NULL,
	[Host] [varchar](64) NULL,
	[IP] [varchar](64) NULL,
	[MAC] [varchar](64) NULL,
	[AppPath] [varchar](256) NULL,
	[AppName] [varchar](128) NULL,
	[ProcessName] [varchar](128) NULL,
	[FirstCag] [varchar](128) NULL,
	[SecondCag] [varchar](128) NULL,
	[ThirdCag] [varchar](128) NULL,
	[MonitorValue] [decimal](18, 0) NULL,
	[Total] [int] NOT NULL CONSTRAINT [DF__LogSecond__Total__173876EA]  DEFAULT ((0)),
	[ForthCag] [varchar](128) NULL,
	[FifthCag] [varchar](128) NULL,
 CONSTRAINT [PK_LogSecond] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Pointer]    Script Date: 2016/2/23 23:59:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Pointer](
	[TargetTable] [varchar](64) NULL,
	[StartTime] [datetime] NULL,
	[EndTime] [datetime] NULL,
	[StartID] [bigint] NULL,
	[EndID] [bigint] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[Logs] ADD  CONSTRAINT [DF__Logs__ProcessNam__1920BF5C]  DEFAULT ('') FOR [ProcessName]
GO
ALTER TABLE [dbo].[Logs] ADD  CONSTRAINT [DF__Logs__ForthCag__182C9B23]  DEFAULT ('') FOR [ForthCag]
GO
/****** Object:  StoredProcedure [dbo].[CleanTranLog]    Script Date: 2016/2/23 23:59:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[CleanTranLog]
AS 
    BEGIN
        DECLARE @num TINYINT --执行次数
        DECLARE @backLogName VARCHAR(100) ;--备份日志文件名称
        DECLARE @backLogPath VARCHAR(100) ; --备份日志文件的路径
        SET @num = 0 ;
        SET @backLogPath = N'E:\SQLBackup' ;--设定备份日志的路径
        --备份3次镜像日志文件，同时删除
        WHILE( @num < 3 )
            BEGIN
                DECLARE @LogPath VARCHAR(100)
                SET @backLogName = CAST(@num as VARCHAR(2)) + '.trn' ;
                SET @LogPath = @backLogPath + '\' + @backLogName
                BACKUP LOG Monitors  TO DISK = @LogPath WITH NOFORMAT, NOINIT,
                    NAME= @backLogName, SKIP, REWIND, NOUNLOAD,STATS = 10
                SET @num = @num + 1 
                --删除刚备份的trn日志文件结束的备份日志文件
                EXECUTE master.dbo.xp_delete_file 0, @LogPath ;
            end
         --收缩日志文件到200M
        DBCC SHRINKFILE (Monitors_log, 300) ; 	
        
        --注意
        --ICSONDB 这里指： 数据库名称
        --IAS_log 为日志逻辑名称
    END

GO
/****** Object:  StoredProcedure [dbo].[MinuteParseSP]    Script Date: 2016/2/23 23:59:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[MinuteParseSP] 
 (
	@Number INT=1,
	@SaveSecondLogDays INT=1
)
As

DECLARE @PointerTarget varchar(32)
SET @PointerTarget='LogSecond'

DECLARE @StartTime DATETIME
DECLARE @EndTime DATETIME
DECLARE @inited int
SELECT @inited=1,@EndTime=EndTime FROM Pointer where TargetTable=@PointerTarget 
if(@inited is null)
	insert Pointer (TargetTable) select @PointerTarget

IF(@EndTime IS NULL)
BEGIN
	select @StartTime= DATEADD(ss,-1,MIN(MonitorTime))   from LogSecond(nolock)	
END
else
begin
	set @StartTime=@EndTime 
end
	--set @EndTime=GETDATE() convert(varchar(16),getdate(),120)
	set @EndTime=convert(varchar(16),DATEADD(mi,@Number,@StartTime),120) 
	--set @EndTime=DATEADD(mi,@Number,@StartTime)
	--if(@StartTime>DATEADD(mi,-@Number,GETDATE())) return;
	if(@EndTime>DATEADD(mi,-@Number,convert(varchar(16),getdate(),120))) return;
	--set @EndTime=GETDATE() convert(varchar(16),getdate(),120)

INSERT INTO LogMinute 
(
	MonitorAction ,
	MonitorTime,
	Host,
	IP,
	MAC,
	AppPath,
	AppName,
	ProcessName,
	FirstCag,
	SecondCag,
	ThirdCag,
	ForthCag,
	MonitorValue,
	Total
)
SELECT 
	MonitorAction ,
	DATEADD(mi,(DATEPART(mi,MonitorTime)/@Number)*@Number,Convert(varchar(14),MonitorTime,120)+'00') ,
	Host,
	IP,
	MAC,
	AppPath,
	AppName,
	ProcessName,
	FirstCag,
	SecondCag,
	ThirdCag,
	ForthCag,
	avg(MonitorValue),
	SUM(1)
FROM LogSecond WITH(NOLOCK)
WHERE MonitorTime>@StartTime and MonitorTime<=@EndTime
GROUP BY 
	MonitorAction ,
	DATEADD(mi,(DATEPART(mi,MonitorTime)/@Number)*@Number,Convert(varchar(14),MonitorTime,120)+'00') ,
	Host,
	IP,
	MAC,
	AppPath,
	AppName,
	ProcessName,
	FirstCag,
	SecondCag,
	ThirdCag,
	ForthCag
 
 
 UPDATE [dbo].[Pointer] SET [EndTime]=@EndTime WHERE [TargetTable]=@PointerTarget
	 
 
DELETE FROM LogSecond  WHERE MonitorTime<DATEADD(dd, -@SaveSecondLogDays, @EndTime)

GO
/****** Object:  StoredProcedure [dbo].[SaveLogSP]    Script Date: 2016/2/23 23:59:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SaveLogSP]
(
	@MonitorAction VARCHAR(32) ,
	@MonitorTime    VARCHAR(32),
	@IP             VARCHAR(32),
	@Host           VARCHAR(32),
	@MAC            VARCHAR(32),
	@AppPath        VARCHAR(128),
	@AppName			 VARCHAR(128),
	@ProcessName		 VARCHAR(128),
	@FirstCag       VARCHAR(32),
	@SecondCag      VARCHAR(32),
	@MonitorValue   FLOAT,
	@Msg			VARCHAR(256)
)
AS 
INSERT Logs 
(
    MonitorAction  ,
    MonitorTime    ,
    IP             ,
    Host           ,
    MAC            ,
    AppPath        ,
	AppName			,
	ProcessName		,
    FirstCag       ,
    SecondCag      ,
    MonitorValue   ,
    Msg	 
)
SELECT 
    @MonitorAction  ,
    @MonitorTime    ,
    @IP             ,
    @Host           ,
    @MAC            ,
    @AppPath        ,
	@AppName			,
	@ProcessName		,
    @FirstCag       ,
    @SecondCag       ,
    @MonitorValue   ,
    @Msg

GO
/****** Object:  StoredProcedure [dbo].[SecondParseSP]    Script Date: 2016/2/23 23:59:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[SecondParseSP] 
(
	@Number INT=5
)
As
DECLARE @MaxMonitorTime DATETIME
DECLARE @MinMonitorTime DATETIME
SELECT @MaxMonitorTime=MAX(MonitorTime),@MinMonitorTime=MIN(MonitorTime) FROM Logs(nolock)
IF(@MaxMonitorTime IS NULL)
	RETURN
IF(DATEADD(ss,-@Number,@MaxMonitorTime)>=@MinMonitorTime)
	SET @MaxMonitorTime=DATEADD(ss,-@Number,@MaxMonitorTime)  
INSERT INTO Logsecond 
(
	MonitorAction ,
	MonitorTime,
	Host,
	IP,
	MAC,
	AppPath,
	AppName,
	ProcessName,
	FirstCag,
	SecondCag,
	ThirdCag,
	ForthCag,
	MonitorValue,	
	Total
)
SELECT 
	MonitorAction ,
	DATEADD(ss,(DATEPART(ss,MonitorTime)/@Number)*@Number,Convert(VARCHAR(16),MonitorTime,120)) ,
	Host,
	IP,
	MAC,
	AppPath,
	AppName,
	ProcessName,
	FirstCag,
	SecondCag,
	ThirdCag,
	ForthCag,
	avg(MonitorValue),
	COUNT(1)
FROM Logs(nolock)
WHERE MonitorTime<=@MaxMonitorTime -- and ThirdCag<>'SaveStatus' and ThirdCag<>'SaveDealStatus'
GROUP BY 
	MonitorAction ,
	DATEADD(ss,(DATEPART(ss,MonitorTime)/@Number)*@Number,Convert(VARCHAR(16),MonitorTime,120)),
	Host,
	IP,
	MAC,
	AppPath,
	AppName,
	ProcessName,
	FirstCag,
	SecondCag,	
	ThirdCag,
	ForthCag
	
DELETE FROM Logs WHERE MonitorTime<=@MaxMonitorTime

GO
USE [master]
GO
ALTER DATABASE [Monitors] SET  READ_WRITE 
GO

USE [TrainTasker]
GO

/****** Object:  Table [dbo].[TaskRecord]    Script Date: 05/19/2021 23:42:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[TaskRecord](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[RequesterID] [varchar](128) NULL,
	[TaskerID] [varchar](128) NULL,
	[TaskID] [varchar](128) NULL,
	[JobID] [varchar](128) NULL,
	[TaskType] [varchar](128) NULL,
	[ResourcePath] [varchar](128) NULL,
	[TimeStamp] [varchar](128) NULL,
	[Result] [varchar](128) NULL,
	[MaxTrains] [int] NULL,
	[TrainCount] [int] NULL,
	[guid] [varchar](36) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO



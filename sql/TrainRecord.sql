USE [TrainTasker]
GO

/****** Object:  Table [dbo].[TrainRecord]    Script Date: 05/19/2021 23:43:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[TrainRecord](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[X] [decimal](10, 2) NULL,
	[Y] [decimal](10, 2) NULL,
	[W] [decimal](10, 2) NULL,
	[H] [decimal](10, 2) NULL,
	[Result] [varchar](128) NULL,
	[ResourcePath] [varchar](128) NULL,
	[TaskRecordID] [int] NULL,
	[timeStamp] [datetime] NULL,
	[UserID] [varchar](128) NULL,
	[RequesterID] [varchar](128) NULL,
	[JobID] [varchar](128) NULL,
	[guid] [varchar](36) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO



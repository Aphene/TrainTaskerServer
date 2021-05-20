USE [TrainTasker]
GO

/****** Object:  Table [dbo].[JobRecord]    Script Date: 05/19/2021 23:40:29 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[JobRecord](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[userID] [varchar](128) NULL,
	[name] [varchar](128) NULL,
	[title] [varchar](128) NULL,
	[description] [varchar](128) NULL,
	[instructions] [varchar](128) NULL,
	[bounty] [varchar](128) NULL,
	[budget] [varchar](128) NULL,
	[budgetLeft] [varchar](128) NULL,
	[active] [bit] NULL,
	[startTime] [varchar](128) NULL,
	[endTime] [varchar](128) NULL,
	[guid] [varchar](36) NULL,
	[type] [varchar](128) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO



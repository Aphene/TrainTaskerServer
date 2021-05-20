USE [TrainTasker]
GO

/****** Object:  Table [dbo].[UserRecord]    Script Date: 05/19/2021 23:44:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[UserRecord](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[userID] [varchar](128) NULL,
	[screenName] [varchar](128) NULL,
	[email] [varchar](128) NULL,
	[password] [varchar](128) NULL,
	[balance] [varchar](1) NULL,
	[isRequester] [varchar](5) NULL,
	[guid] [varchar](36) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO



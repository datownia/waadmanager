USE [WindowsAzureAD_Example]
GO

/****** Object:  Table [dbo].[delta]    Script Date: 05/29/2013 15:50:12 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[delta](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[environment] [varchar](max) NOT NULL,
	[apiname] [varchar](max) NOT NULL,
	[delta] [int] NULL,
	[notes] [varchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

GO

SET ANSI_PADDING OFF
GO



USE [WindowsAzureAD_Example]
GO

/****** Object:  Table [dbo].[api_user]    Script Date: 05/29/2013 15:48:37 ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[api_user](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[environment] [nvarchar](max) NOT NULL,
	[username] [nvarchar](max) NOT NULL,
	[datowniaId] [nvarchar](max) NOT NULL,
	[adUsername] [nvarchar](max) NOT NULL,
 CONSTRAINT [PrimaryKey_Api_User] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

GO



USE [WindowsAzureAD_Example]
GO

/****** Object:  Table [dbo].[api_group]    Script Date: 05/29/2013 15:46:22 ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[api_group](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[environment] [nvarchar](max) NOT NULL,
	[groupname] [nvarchar](max) NOT NULL,
	[datowniaId] [nvarchar](max) NOT NULL,
 CONSTRAINT [PrimaryKey_Api_Group] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

GO



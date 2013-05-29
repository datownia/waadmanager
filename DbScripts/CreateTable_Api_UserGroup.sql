USE [WindowsAzureAD_Example]
GO

/****** Object:  Table [dbo].[api_usergroup]    Script Date: 05/29/2013 15:49:29 ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[api_usergroup](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[groupname] [nvarchar](max) NOT NULL,
	[username] [nvarchar](max) NOT NULL,
	[datowniaId] [nvarchar](max) NOT NULL,
	[environment] [nvarchar](max) NOT NULL,
 CONSTRAINT [PrimaryKey_793ebe51-7468-473e-a39e-14d3d5132f78] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

GO



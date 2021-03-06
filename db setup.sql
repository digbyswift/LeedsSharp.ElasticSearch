SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SocialChannel](
	[UserId] [uniqueidentifier] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Url] [nvarchar](250) NOT NULL,
	[Followers] [int] NULL
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[Id] [uniqueidentifier] NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
	[Latitude] [decimal](11, 7) NOT NULL,
	[Longitude] [decimal](11, 7) NOT NULL,
	[Address1] [varchar](50) NOT NULL,
	[Address2] [varchar](50) NULL,
	[City] [varchar](50) NOT NULL,
	[Country] [varchar](50) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserAttribute](
	[UserId] [uniqueidentifier] NOT NULL,
	[Key] [varchar](50) NOT NULL,
	[Value] [nvarchar](50) NULL,
 CONSTRAINT [PK_UserAttribute] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO
ALTER TABLE [dbo].[SocialChannel]  WITH CHECK ADD  CONSTRAINT [FK_SocialChannel_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[SocialChannel] CHECK CONSTRAINT [FK_SocialChannel_User]
GO
ALTER TABLE [dbo].[UserAttribute]  WITH CHECK ADD  CONSTRAINT [FK_UserAttribute_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[UserAttribute] CHECK CONSTRAINT [FK_UserAttribute_User]
GO

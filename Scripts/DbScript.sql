USE [WorkflowDb]
GO
/****** Object:  Table [dbo].[WorkflowInstances]    Script Date: 12/16/2018 1:48:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WorkflowInstances](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InstanceId] [uniqueidentifier] NOT NULL,
	[WorkflowName] [nvarchar](100) NOT NULL,
	[CreationDate] [date] NOT NULL,
	[InstanceStatus] [int] NOT NULL,
	[InstanceNextBookMarks] [nvarchar](200) NULL,
 CONSTRAINT [PK_WorkflowInstances] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[WorkflowInstances] ADD  CONSTRAINT [DF_WorkflowInstances_CreationDate]  DEFAULT (getdate()) FOR [CreationDate]
GO

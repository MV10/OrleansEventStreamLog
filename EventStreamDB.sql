
USE [OrleansESL]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- https://stackoverflow.com/questions/57867166/avoid-deadlocking-within-a-serializable-isolation-level-transaction

-- No clustered PK, all reads are by specific CustomerId ordered by ETag
CREATE TABLE [dbo].[CustomerEventStream] (
    [CustomerId] VARCHAR (20)  NOT NULL, -- arbitrary size, indexes don't allow MAX
	[ETag]       INT           NOT NULL,
    [Timestamp]  CHAR (33)     NOT NULL, -- DateTimeOffset format "O" size, ex. 2019-11-27T05:41:12.1320053-05:00
    [EventType]  VARCHAR (MAX) NOT NULL,
    [Payload]    VARCHAR (MAX) NOT NULL
);
GO

CREATE UNIQUE NONCLUSTERED INDEX IX_CustomerEventStream ON
[dbo].[CustomerEventStream] ([CustomerId] ASC, [ETag] ASC);
GO

-- No clustered PK, all reads are for a specific CustomerId
CREATE TABLE [dbo].[CustomerSnapshot] (
    [CustomerId] VARCHAR (20)  NOT NULL,
	[ETag]       INT           NOT NULL,
    [Snapshot]   VARCHAR (MAX) NOT NULL
);
GO

CREATE UNIQUE NONCLUSTERED INDEX IX_CustomerSnapshot ON
[dbo].[CustomerSnapshot] ([CustomerId] ASC);
GO

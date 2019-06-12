CREATE TABLE [dbo].[Movies] (
    [id]          BIGINT         NOT NULL IDENTITY,
    [name]        NVARCHAR (200) NOT NULL,
    [description] NVARCHAR (MAX) NULL,
    [year]        INT            NOT NULL,
    [director]    NVARCHAR (200) NOT NULL,
    [poster]      NVARCHAR (200) NULL,
    [ts]          DATETIME       DEFAULT (getdate()) NOT NULL,
    [UserId]      NVARCHAR (128) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC)
);


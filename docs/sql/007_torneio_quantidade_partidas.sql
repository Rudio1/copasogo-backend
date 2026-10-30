SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF COL_LENGTH(N'dbo.torneios', N'quantidade_partidas') IS NULL
    ALTER TABLE [dbo].[torneios] ADD [quantidade_partidas] int NOT NULL CONSTRAINT [DF_torneios_quantidade_partidas] DEFAULT 31;
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- Vitoria por W.O.: a partida fica Completed com vencedor, sem placar, e wo = 1.
IF COL_LENGTH(N'dbo.partidas', N'wo') IS NULL
    ALTER TABLE [dbo].[partidas] ADD [wo] bit NOT NULL CONSTRAINT [DF_partidas_wo] DEFAULT CAST(0 AS bit);
GO

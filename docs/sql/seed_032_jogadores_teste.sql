SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- 32 jogadores ficticios para teste de tela. Pode rodar de novo: nao duplica o steam_id.

INSERT INTO [dbo].[jogadores] ([id], [nome], [steam_id], [created_at], [is_deleted])
SELECT
    CAST(v.[id] AS uniqueidentifier),
    v.[nome],
    v.[steam_id],
    SYSUTCDATETIME(),
    CAST(0 AS bit)
FROM (VALUES
    ('a1000001-0000-4000-8000-000000000001', N'Teste 01 Aurora',   N'https://steamcommunity.com/id/copasogo-teste-01'),
    ('a1000001-0000-4000-8000-000000000002', N'Teste 02 Bruno',    N'https://steamcommunity.com/id/copasogo-teste-02'),
    ('a1000001-0000-4000-8000-000000000003', N'Teste 03 Caio',     N'https://steamcommunity.com/id/copasogo-teste-03'),
    ('a1000001-0000-4000-8000-000000000004', N'Teste 04 Duda',     N'https://steamcommunity.com/id/copasogo-teste-04'),
    ('a1000001-0000-4000-8000-000000000005', N'Teste 05 Elias',    N'https://steamcommunity.com/id/copasogo-teste-05'),
    ('a1000001-0000-4000-8000-000000000006', N'Teste 06 Fernanda', N'https://steamcommunity.com/id/copasogo-teste-06'),
    ('a1000001-0000-4000-8000-000000000007', N'Teste 07 Gael',     N'https://steamcommunity.com/id/copasogo-teste-07'),
    ('a1000001-0000-4000-8000-000000000008', N'Teste 08 Helena',   N'https://steamcommunity.com/id/copasogo-teste-08'),
    ('a1000001-0000-4000-8000-000000000009', N'Teste 09 Igor',     N'https://steamcommunity.com/id/copasogo-teste-09'),
    ('a1000001-0000-4000-8000-000000000010', N'Teste 10 Julia',    N'https://steamcommunity.com/id/copasogo-teste-10'),
    ('a1000001-0000-4000-8000-000000000011', N'Teste 11 Kaio',     N'https://steamcommunity.com/id/copasogo-teste-11'),
    ('a1000001-0000-4000-8000-000000000012', N'Teste 12 Lara',     N'https://steamcommunity.com/id/copasogo-teste-12'),
    ('a1000001-0000-4000-8000-000000000013', N'Teste 13 Miguel',   N'https://steamcommunity.com/id/copasogo-teste-13'),
    ('a1000001-0000-4000-8000-000000000014', N'Teste 14 Nina',     N'https://steamcommunity.com/id/copasogo-teste-14'),
    ('a1000001-0000-4000-8000-000000000015', N'Teste 15 Otto',     N'https://steamcommunity.com/id/copasogo-teste-15'),
    ('a1000001-0000-4000-8000-000000000016', N'Teste 16 Paula',    N'https://steamcommunity.com/id/copasogo-teste-16'),
    ('a1000001-0000-4000-8000-000000000017', N'Teste 17 Rafael',   N'https://steamcommunity.com/id/copasogo-teste-17'),
    ('a1000001-0000-4000-8000-000000000018', N'Teste 18 Sofia',    N'https://steamcommunity.com/id/copasogo-teste-18'),
    ('a1000001-0000-4000-8000-000000000019', N'Teste 19 Theo',     N'https://steamcommunity.com/id/copasogo-teste-19'),
    ('a1000001-0000-4000-8000-000000000020', N'Teste 20 Ursula',   N'https://steamcommunity.com/id/copasogo-teste-20'),
    ('a1000001-0000-4000-8000-000000000021', N'Teste 21 Vitor',    N'https://steamcommunity.com/id/copasogo-teste-21'),
    ('a1000001-0000-4000-8000-000000000022', N'Teste 22 Wanda',    N'https://steamcommunity.com/id/copasogo-teste-22'),
    ('a1000001-0000-4000-8000-000000000023', N'Teste 23 Xavier',   N'https://steamcommunity.com/id/copasogo-teste-23'),
    ('a1000001-0000-4000-8000-000000000024', N'Teste 24 Yasmin',   N'https://steamcommunity.com/id/copasogo-teste-24'),
    ('a1000001-0000-4000-8000-000000000025', N'Teste 25 Zeca',     N'https://steamcommunity.com/id/copasogo-teste-25'),
    ('a1000001-0000-4000-8000-000000000026', N'Teste 26 Aline',    N'https://steamcommunity.com/id/copasogo-teste-26'),
    ('a1000001-0000-4000-8000-000000000027', N'Teste 27 Bento',    N'https://steamcommunity.com/id/copasogo-teste-27'),
    ('a1000001-0000-4000-8000-000000000028', N'Teste 28 Celina',   N'https://steamcommunity.com/id/copasogo-teste-28'),
    ('a1000001-0000-4000-8000-000000000029', N'Teste 29 Davi',     N'https://steamcommunity.com/id/copasogo-teste-29'),
    ('a1000001-0000-4000-8000-000000000030', N'Teste 30 Eva',      N'https://steamcommunity.com/id/copasogo-teste-30'),
    ('a1000001-0000-4000-8000-000000000031', N'Teste 31 Fabio',    N'https://steamcommunity.com/id/copasogo-teste-31'),
    ('a1000001-0000-4000-8000-000000000032', N'Teste 32 Gabi',     N'https://steamcommunity.com/id/copasogo-teste-32')
) AS v ([id], [nome], [steam_id])
WHERE NOT EXISTS (
    SELECT 1
    FROM [dbo].[jogadores] AS j
    WHERE j.[steam_id] = v.[steam_id]
      AND j.[is_deleted] = 0
);
GO

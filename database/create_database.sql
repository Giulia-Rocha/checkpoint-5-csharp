IF DB_ID(N'CadastroProdutosDb') IS NULL
BEGIN
    CREATE DATABASE CadastroProdutosDb;
END;
GO

USE CadastroProdutosDb;
GO

IF OBJECT_ID(N'dbo.Produtos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Produtos
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Produtos PRIMARY KEY,
        Nome NVARCHAR(120) NOT NULL,
        Preco DECIMAL(18,2) NOT NULL,
        Estoque INT NOT NULL,
        Categoria NVARCHAR(80) NOT NULL,
        CONSTRAINT CK_Produtos_Preco_NaoNegativo CHECK (Preco >= 0),
        CONSTRAINT CK_Produtos_Estoque_NaoNegativo CHECK (Estoque >= 0)
    );
END;
GO

PRAGMA foreign_keys = ON;

CREATE TABLE IF NOT EXISTS Clientes (
    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    NomeCompleto TEXT NOT NULL CHECK (length(NomeCompleto) <= 150),
    Cpf TEXT NOT NULL UNIQUE CHECK (length(Cpf) = 11),
    DataNascimento TEXT NOT NULL,
    Email TEXT NULL CHECK (Email IS NULL OR length(Email) <= 200)
);

CREATE TABLE IF NOT EXISTS Dividas (
    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    ClienteId INTEGER NOT NULL,
    Valor NUMERIC NOT NULL CHECK (Valor > 0),
    Paga INTEGER NOT NULL DEFAULT 0 CHECK (Paga IN (0, 1)),
    DataCriacao TEXT NOT NULL,
    DataPagamento TEXT NULL,
    CONSTRAINT FK_Dividas_Clientes FOREIGN KEY (ClienteId)
        REFERENCES Clientes (Id) ON DELETE CASCADE
);

CREATE UNIQUE INDEX IF NOT EXISTS IX_Dividas_UmaAbertaPorCliente
    ON Dividas (ClienteId) WHERE Paga = 0;

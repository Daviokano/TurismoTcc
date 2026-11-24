-- ATENÇÃO:
-- Este script apaga e recria o banco dbHorizon_Travel.

DROP DATABASE IF EXISTS dbHorizon_Travel;

CREATE DATABASE dbHorizon_Travel;

USE dbHorizon_Travel;

CREATE TABLE Usuario (
    IDUsu INT AUTO_INCREMENT PRIMARY KEY,
    CPFUsu CHAR(11) NOT NULL UNIQUE,
    nomeUsu VARCHAR(100) NOT NULL,
    emailUsu VARCHAR(100) NOT NULL UNIQUE,
    telefoneUsu VARCHAR(20) NOT NULL,
    dataNascimentoUsu DATE NOT NULL,
    senhaUsu VARCHAR(255) NOT NULL,
    situacaoUsu CHAR(2) NOT NULL DEFAULT 'A'
);

CREATE TABLE Funcionario (
    IDFun INT AUTO_INCREMENT PRIMARY KEY,
    CPFFun CHAR(11) NOT NULL UNIQUE,
    nomeFun VARCHAR(100) NOT NULL,
    telefoneFun VARCHAR(20) NOT NULL,
    emailFun VARCHAR(100) NOT NULL UNIQUE,
    senhaFun VARCHAR(255) NOT NULL,
    dataNascimentoFun DATE NOT NULL,
    permissaoFun VARCHAR(25),
    situacaoFun CHAR(1) NOT NULL DEFAULT 'A'
);

CREATE TABLE Parceiro (
    CNPJFor CHAR(14) PRIMARY KEY,
    nomeFor VARCHAR(100) NOT NULL,
    emailFor VARCHAR(100) NOT NULL UNIQUE,
    situacaoFor CHAR(1) NOT NULL DEFAULT 'A'
);

CREATE TABLE Promocao (
    IDPromo INT AUTO_INCREMENT PRIMARY KEY,
    nomePromocao VARCHAR(100) NOT NULL,
    porcentagemDesconto DECIMAL(5,2) NOT NULL,
    dataValidade DATE NOT NULL,

    CHECK (porcentagemDesconto >= 0),
    CHECK (porcentagemDesconto <= 100)
);

CREATE TABLE Pacote (
    IDPac INT AUTO_INCREMENT PRIMARY KEY,
    IDFun INT NOT NULL,
    CNPJFor CHAR(14) NOT NULL,
    IDPromo INT NULL,
    precoPac DECIMAL(10,2) NOT NULL,
    quantidadePac INT NOT NULL,
    descricaoPac TEXT NOT NULL,
    avaliacaoPac DECIMAL(2,1),
    fotoPac VARCHAR(255) NOT NULL,
    origemPac VARCHAR(200) NOT NULL,
    destinoPac VARCHAR(100) NOT NULL,
    dataPac DATE NOT NULL,
    situacaoPac CHAR(1) NOT NULL DEFAULT 'A',

    CONSTRAINT FK_PacoteFuncionario
        FOREIGN KEY (IDFun)
        REFERENCES Funcionario(IDFun),

    CONSTRAINT FK_PacoteParceiro
        FOREIGN KEY (CNPJFor)
        REFERENCES Parceiro(CNPJFor),

    CONSTRAINT FK_PacotePromocao
        FOREIGN KEY (IDPromo)
        REFERENCES Promocao(IDPromo),

    CHECK (precoPac > 0),
    CHECK (quantidadePac >= 0),
    CHECK (avaliacaoPac BETWEEN 0 AND 5)
);

CREATE TABLE Reserva (
    IDRes INT AUTO_INCREMENT PRIMARY KEY,
    IDUsu INT NOT NULL,
    IDPac INT NOT NULL,
    dataRes DATE NOT NULL,
    statusRes VARCHAR(40) NOT NULL,

    CONSTRAINT FK_ReservaUsuario
        FOREIGN KEY (IDUsu)
        REFERENCES Usuario(IDUsu),

    CONSTRAINT FK_ReservaPacote
        FOREIGN KEY (IDPac)
        REFERENCES Pacote(IDPac)
);

CREATE TABLE Pagamento (
    IDPag INT AUTO_INCREMENT PRIMARY KEY,
    IDRes INT NOT NULL UNIQUE,
    valorPag DECIMAL(10,2) NOT NULL,
    dataPag DATE NOT NULL,
    metodoPag VARCHAR(50) NOT NULL,
    statusPag VARCHAR(30) NOT NULL,

    CONSTRAINT FK_PagamentoReserva
        FOREIGN KEY (IDRes)
        REFERENCES Reserva(IDRes),

    CHECK (valorPag > 0)
);

CREATE TABLE Historico (
    IDHist INT AUTO_INCREMENT PRIMARY KEY,
    IDRes INT NOT NULL,
    IDFun INT NULL,
    statusAnterior VARCHAR(40),
    statusNovo VARCHAR(40),
    dataModificacao DATETIME NOT NULL,

    CONSTRAINT FK_HistoricoReserva
        FOREIGN KEY (IDRes)
        REFERENCES Reserva(IDRes),

    CONSTRAINT FK_HistoricoFuncionario
        FOREIGN KEY (IDFun)
        REFERENCES Funcionario(IDFun)
);

CREATE TABLE Passageiro (
    IDPassageiro INT AUTO_INCREMENT PRIMARY KEY,
    IDRes INT NOT NULL,
    nomeCompleto VARCHAR(150) NOT NULL,
    CPF CHAR(11) NOT NULL,
    dataNascimento DATE NOT NULL,
    passaporte VARCHAR(30),

    CONSTRAINT FK_PassageiroReserva
        FOREIGN KEY (IDRes)
        REFERENCES Reserva(IDRes)
);

CREATE TABLE Avaliacao (
    IDAvaliacao INT AUTO_INCREMENT PRIMARY KEY,
    IDUsu INT NOT NULL,
    IDPac INT NOT NULL,
    notaAva DECIMAL(2,1) NOT NULL,
    comentarioAva TEXT,
    dataAva DATE NOT NULL,

    CONSTRAINT FK_AvaliacaoUsuario
        FOREIGN KEY (IDUsu)
        REFERENCES Usuario(IDUsu),

    CONSTRAINT FK_AvaliacaoPacote
        FOREIGN KEY (IDPac)
        REFERENCES Pacote(IDPac),

    CHECK (notaAva BETWEEN 0 AND 5)
);

INSERT INTO Funcionario (
    CPFFun,
    nomeFun,
    telefoneFun,
    emailFun,
    senhaFun,
    permissaoFun,
    situacaoFun,
    dataNascimentoFun
)
VALUES (
    '12345678901',
    'Carlos Silva',
    '(11) 98765-4321',
    'carlos@exemplo.com',
    '23456789',
    'Administrador',
    'A',
    '1992-08-15'
);


DELIMITER $$

CREATE PROCEDURE ht_CadastrarUsuario (
    IN pCPF CHAR(11),
    IN pNome VARCHAR(100),
    IN pEmail VARCHAR(100),
    IN pTelefone VARCHAR(20),
    IN pDataNascimento DATE,
    IN pSenha VARCHAR(255)
)
BEGIN
    INSERT INTO Usuario (
        CPFUsu,
        nomeUsu,
        emailUsu,
        telefoneUsu,
        dataNascimentoUsu,
        senhaUsu,
        situacaoUsu
    )
    VALUES (
        pCPF,
        pNome,
        pEmail,
        pTelefone,
        pDataNascimento,
        pSenha,
        'A'
    );
END $$

CREATE PROCEDURE ht_RealizarReserva (
    IN pIDUsu INT,
    IN pIDPac INT,
    IN pData DATE
)
BEGIN
    DECLARE vQuantidade INT;

    SELECT quantidadePac
    INTO vQuantidade
    FROM Pacote
    WHERE IDPac = pIDPac;

    IF vQuantidade > 0 THEN

        INSERT INTO Reserva (
            IDUsu,
            IDPac,
            dataRes,
            statusRes
        )
        VALUES (
            pIDUsu,
            pIDPac,
            pData,
            'Pendente'
        );

        UPDATE Pacote
        SET quantidadePac = quantidadePac - 1
        WHERE IDPac = pIDPac;

    ELSE

        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Pacote esgotado.';

    END IF;
END $$

CREATE TRIGGER trg_HistoricoReserva
AFTER UPDATE ON Reserva
FOR EACH ROW
BEGIN
    IF OLD.statusRes <> NEW.statusRes THEN
        INSERT INTO Historico (
            IDRes,
            statusAnterior,
            statusNovo,
            dataModificacao
        )
        VALUES (
            OLD.IDRes,
            OLD.statusRes,
            NEW.statusRes,
            NOW()
        );
    END IF;
END $$

CREATE TRIGGER trg_AtualizarMedia
AFTER INSERT ON Avaliacao
FOR EACH ROW
BEGIN
    UPDATE Pacote
    SET avaliacaoPac = (
        SELECT ROUND(AVG(notaAva), 1)
        FROM Avaliacao
        WHERE IDPac = NEW.IDPac
    )
    WHERE IDPac = NEW.IDPac;
END $$

CREATE TRIGGER trg_AtualizarMediaUpdate
AFTER UPDATE ON Avaliacao
FOR EACH ROW
BEGIN
    UPDATE Pacote
    SET avaliacaoPac = (
        SELECT ROUND(AVG(notaAva), 1)
        FROM Avaliacao
        WHERE IDPac = NEW.IDPac
    )
    WHERE IDPac = NEW.IDPac;
END $$

CREATE TRIGGER trg_AtualizarMediaDelete
AFTER DELETE ON Avaliacao
FOR EACH ROW
BEGIN
    UPDATE Pacote
    SET avaliacaoPac = (
        SELECT ROUND(AVG(notaAva), 1)
        FROM Avaliacao
        WHERE IDPac = OLD.IDPac
    )
    WHERE IDPac = OLD.IDPac;
END $$

DELIMITER ;


ALTER TABLE Pacote
ADD COLUMN quantidadePassageirosPac INT NOT NULL DEFAULT 1
AFTER quantidadePac;

ALTER TABLE Pacote
ADD CONSTRAINT CK_PacoteQuantidadePassageiros
CHECK (quantidadePassageirosPac > 0);

ALTER TABLE Promocao
ADD COLUMN abrangenciaPromocao VARCHAR(100) NOT NULL DEFAULT 'Todos'
AFTER nomePromocao;

ALTER TABLE Promocao
ADD COLUMN situacaoPromocao CHAR(1) NOT NULL DEFAULT 'A'
AFTER dataValidade;

DESCRIBE Usuario;
DESCRIBE Funcionario;
DESCRIBE Parceiro;
DESCRIBE Promocao;
DESCRIBE Pacote;
DESCRIBE Reserva;
DESCRIBE Pagamento;
DESCRIBE Historico;
DESCRIBE Passageiro;
DESCRIBE Avaliacao;

SELECT * FROM Usuario;
SELECT * FROM Funcionario;
SELECT * FROM Parceiro;
SELECT * FROM Promocao;
SELECT * FROM Pacote;
SELECT * FROM Reserva;
SELECT * FROM Pagamento;
SELECT * FROM Historico;
SELECT * FROM Passageiro;
SELECT * FROM Avaliacao;
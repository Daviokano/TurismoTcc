CREATE DATABASE dbHorizon_Travel;
-- DROP DATABASE dbHorizon_travel;
USE dbHorizon_Travel;

CREATE TABLE Endereco(

    IDEnd INT AUTO_INCREMENT PRIMARY KEY,

    ruaEnd VARCHAR(200) NOT NULL,

    numEnd VARCHAR(10) NOT NULL,

    ruaEnd VARCHAR(100) NOT NULL UNIQUE,

    bairroEnd VARCHAR(20) NOT NULL,

    cidEnd VARCHAR(200) NOT NULL,

    estEnd CHAR(2) NOT NULL,

    CEPEnd CHAR(8) NOT NULL

);


CREATE TABLE Usuario(

    IDUsu INT AUTO_INCREMENT PRIMARY KEY,

    CPFUsu CHAR(11) NOT NULL UNIQUE,

    nomeUsu VARCHAR(100) NOT NULL,

    emailUsu VARCHAR(100) NOT NULL UNIQUE,

    telefoneUsu VARCHAR(20) NOT NULL,

    IDEnd INT NOT NULL,

    dataNascimentoUsu DATETIME NOT NULL,

    senhaUsu CHAR(8) NOT NULL,
    
    CONSTRAINT FK_UsuarioEndereco
        FOREIGN KEY (IDEnd)
        REFERENCES Endereco(IDEnd)

);


CREATE TABLE Funcionario(

    IDFun INT AUTO_INCREMENT PRIMARY KEY,

    CPFFun CHAR(11) NOT NULL UNIQUE,

    nomeFun VARCHAR(100) NOT NULL,

    telefoneFun VARCHAR(20) NOT NULL,

    emailFun VARCHAR(100) NOT NULL UNIQUE,

    senhaFun CHAR(8) NOT NULL,

    dataNascimentoFun DATETIME NOT NULL

);

CREATE TABLE Parceiro(

    CNPJFor CHAR(14) PRIMARY KEY,

    nomeFor VARCHAR(100) NOT NULL,

    enderecoFor VARCHAR(200) NOT NULL,

    emailFor VARCHAR(100) NOT NULL UNIQUE,
    
    IDEnd INT NOT NULL,
    
    CONSTRAINT FK_ParceiroEndereco
        FOREIGN KEY (IDEnd)
        REFERENCES Endereco(IDEnd)

);

CREATE TABLE Promocao(

    IDPromo INT AUTO_INCREMENT PRIMARY KEY,

    porcentagemDesconto DECIMAL(5,2) NOT NULL,

    dataValidade DATETIME NOT NULL,

    CHECK(porcentagemDesconto >= 0),

    CHECK(porcentagemDesconto <=100)

);

CREATE TABLE Pacote(

    IDPac INT AUTO_INCREMENT PRIMARY KEY,

    IDFun INT NOT NULL,

    CNPJFor CHAR(14) NOT NULL,

    IDPromo INT,

    precoPac DECIMAL(10,2) NOT NULL,

    quantidadePac INT NOT NULL,

    descricaoPac TEXT NOT NULL,

    avaliacaoPac DECIMAL(2,1),

    fotoPac VARCHAR(255),

    destinoPac VARCHAR(100) NOT NULL,

    dataPac DATETIME NOT NULL,

    CONSTRAINT FK_PacoteFuncionario
        FOREIGN KEY(IDFun)
        REFERENCES Funcionario(IDFun),

    CONSTRAINT FK_PacoteParceiro
        FOREIGN KEY(CNPJFor)
        REFERENCES Parceiro(CNPJFor),

    CONSTRAINT FK_PacotePromocao
        FOREIGN KEY(IDPromo)
        REFERENCES Promocao(IDPromo),

    CHECK(precoPac > 0),

    CHECK(quantidadePac >=0),

    CHECK(avaliacaoPac BETWEEN 0 AND 5)

);

CREATE TABLE Reserva(

    IDRes INT AUTO_INCREMENT PRIMARY KEY,

    IDUsu INT NOT NULL,

    IDPac INT NOT NULL,

    dataRes DATETIME NOT NULL,

    statusRes VARCHAR(40) NOT NULL,

    CONSTRAINT FK_ReservaUsuario
        FOREIGN KEY(IDUsu)
        REFERENCES Usuario(IDUsu),

    CONSTRAINT FK_ReservaPacote
        FOREIGN KEY(IDPac)
        REFERENCES Pacote(IDPac)

);

CREATE TABLE Pagamento(

    IDPag INT AUTO_INCREMENT PRIMARY KEY,

    IDRes INT NOT NULL UNIQUE,

    valorPag DECIMAL(10,2) NOT NULL,

    dataPag DATETIME NOT NULL,

    metodoPag VARCHAR(50) NOT NULL,

    statusPag VARCHAR(30) NOT NULL,

    CONSTRAINT FK_PagamentoReserva
        FOREIGN KEY(IDRes)
        REFERENCES Reserva(IDRes),

    CHECK(valorPag >0)

);



CREATE TABLE Historico(

    IDHist INT AUTO_INCREMENT PRIMARY KEY,

    IDRes INT NOT NULL,

    statusAnterior VARCHAR(40),

    dataModificacao DATETIME NOT NULL,

    CONSTRAINT FK_HistoricoReserva
        FOREIGN KEY(IDRes)
        REFERENCES Reserva(IDRes)

);


CREATE TABLE Avaliacao(

    IDAvaliacao INT AUTO_INCREMENT PRIMARY KEY,

    IDUsu INT NOT NULL,

    IDPac INT NOT NULL,

    notaAva DECIMAL(2,1) NOT NULL,

    comentarioAva TEXT,

    dataAva DATETIME NOT NULL,

    CONSTRAINT FK_AvaliacaoUsuario
        FOREIGN KEY(IDUsu)
        REFERENCES Usuario(IDUsu),

    CONSTRAINT FK_AvaliacaoPacote
        FOREIGN KEY(IDPac)
        REFERENCES Pacote(IDPac),

       CHECK(notaAva BETWEEN 0 AND 5)

);

-- Cadastrar um novo usuário

DELIMITER $$

CREATE PROCEDURE ht_CadastrarUsuario(

IN pCPF CHAR(11),
IN pNome VARCHAR(100),
IN pEmail VARCHAR(100),
IN pTelefone VARCHAR(20),
IN pEndereco VARCHAR(200),
IN pDataNascimento DATE,
IN pSenha VARCHAR(255)

)

BEGIN

INSERT INTO Usuario
(CPFUsu,nomeUsu,emailUsu,telefoneUsu,enderecoUsu,dataNascimentoUsu,senhaUsu)

VALUES
(pCPF,pNome,pEmail,pTelefone,pEndereco,pDataNascimento,pSenha);

END $$

DELIMITER ;

-- realizar uma reserva

DELIMITER $$

CREATE PROCEDURE ht_RealizarReserva(

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

        INSERT INTO Reserva
        (IDUsu, IDPac, dataRes, statusRes)

        VALUES
        (pIDUsu, pIDPac, pData, 'Pendente');

        UPDATE Pacote
        SET quantidadePac = quantidadePac - 1
        WHERE IDPac = pIDPac;

    ELSE

        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Pacote esgotado.';

    END IF;

END $$

DELIMITER ;

-- Serve par guardar o histórico quando tiver alguma alteração
DELIMITER $$

CREATE TRIGGER trg_HistoricoReserva

AFTER UPDATE ON Reserva

FOR EACH ROW

BEGIN

IF OLD.statusRes <> NEW.statusRes THEN

INSERT INTO Historico
(IDRes,statusAnterior,dataModificacao)

VALUES
(
OLD.IDRes,
OLD.statusRes,
NOW()
);

END IF;

END$$

DELIMITER ;


-- atualizar automaticamente quando mudar a media da nota da empresa
DELIMITER $$

CREATE TRIGGER trg_AtualizarMedia

AFTER INSERT ON Avaliacao

FOR EACH ROW

BEGIN

    UPDATE Pacote

    SET avaliacaoPac = (

        SELECT ROUND(AVG(notaAva),1)

        FROM Avaliacao

        WHERE IDPac = NEW.IDPac

    )

    WHERE IDPac = NEW.IDPac;

END $$

DELIMITER ;

-- Update da média

DELIMITER $$

CREATE TRIGGER trg_AtualizarMediaUpdate

AFTER UPDATE ON Avaliacao

FOR EACH ROW

BEGIN

    UPDATE Pacote

    SET avaliacaoPac = (

        SELECT ROUND(AVG(notaAva),1)

        FROM Avaliacao

        WHERE IDPac = NEW.IDPac

    )

    WHERE IDPac = NEW.IDPac;

END $$

DELIMITER ;

-- Caso seja apagada a notaa, tem q atualizar a média

DELIMITER $$

CREATE TRIGGER trg_AtualizarMediaDelete

AFTER DELETE ON Avaliacao

FOR EACH ROW

BEGIN

    UPDATE Pacote

    SET avaliacaoPac = (

        SELECT ROUND(AVG(notaAva),1)

        FROM Avaliacao

        WHERE IDPac = OLD.IDPac

    )

    WHERE IDPac = OLD.IDPac;

END $$

DELIMITER ;
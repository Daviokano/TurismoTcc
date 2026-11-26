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

INSERT INTO Pacote (
    IDFun,
    CNPJFor,
    IDPromo,
    precoPac,
    quantidadePac,
    quantidadePassageirosPac,
    descricaoPac,
    avaliacaoPac,
    fotoPac,
    origemPac,
    destinoPac,
    dataPac,
    situacaoPac
)
VALUES

(1, '11222333000181', NULL, 3500.00, 20, 4,
'Pacote turístico para conhecer as principais atrações do Brasil, incluindo passeios guiados, 
visitas a pontos turísticos, alimentação durante os passeios, acompanhamento de guia especializado e tempo livre para conhecer a cultura,
 a gastronomia e as paisagens brasileiras.',
NULL, 'brasil.jpeg', 'São Paulo', 'Brasil', '2027-07-15', 'A'),

(1, '11222333000181', NULL, 6200.00, 12, 4,
'Viagem especial para conhecer a França e suas atrações culturais e históricas, 
incluindo passeios turísticos por Paris, visitas a monumentos famosos, acompanhamento de guia, alimentação e ingressos para atrações selecionadas. 
Aproveite para conhecer a arquitetura francesa e experimentar a gastronomia local.',
NULL, 'franca.jpeg', 'São Paulo', 'França', '2027-07-20', 'A'),

(1, '11222333000181', NULL, 5800.00, 18, 3,
'Pacote turístico pelo Japão para conhecer sua cultura, tradição e modernidade. 
Inclui visitas a pontos turísticos famosos, passeios pelos templos tradicionais, acompanhamento de guia turístico, alimentação e ingressos para atrações selecionadas. 
Uma experiência para descobrir paisagens incríveis e costumes japoneses.',
NULL, 'japao.jpeg', 'São Paulo', 'Japão', '2027-09-05', 'A'),

(1, '11222333000181', NULL, 4900.00, 25, 2,
'Experiência completa no México para conhecer sua história, arquitetura e cultura. 
O pacote inclui passeios guiados por atrações históricas, visitas a monumentos tradicionais, alimentação, transporte turístico e acompanhamento profissional. 
Aproveite também o tempo livre para conhecer a gastronomia e os costumes mexicanos.',
NULL, 'mexico.jpeg', 'Brasília', 'México', '2027-10-12', 'A'),

(1, '11222333000181', NULL, 4500.00, 19, 4,
'Viagem turística pela Inglaterra para conhecer seus principais pontos históricos e culturais. 
Inclui passeios guiados, visitas a monumentos famosos, acompanhamento de guia turístico, alimentação e tempo livre para explorar as ruas, lojas e construções 
tradicionais. Uma oportunidade de conhecer melhor a cultura inglesa.',
NULL, 'inglaterra.jpeg', 'Brasília', 'Inglaterra', '2027-09-25', 'A'),

(1, '11222333000181', NULL, 6800.00, 10, 2,
'Pacote especial para aproveitar a magia da Disney, incluindo ingressos para o parque,
 acesso às principais atrações disponíveis, passeios pelas áreas temáticas, alimentação e acompanhamento turístico. 
 Uma experiência inesquecível para aproveitar brinquedos, apresentações, personagens e momentos especiais em família.',
NULL, 'disney.jpeg', 'São Paulo', 'Disney', '2027-11-20', 'A'),

(1, '11222333000181', NULL, 7200.00, 15, 4,
'Experiência completa no Parque da Disney para aproveitar atrações temáticas, brinquedos, espetáculos
 e encontros com personagens. O pacote inclui ingressos de acesso, alimentação, orientações turísticas e tempo livre para explorar diferentes áreas do parque. 
 Ideal para famílias e visitantes que desejam viver momentos de diversão e entretenimento.',
NULL, 'parque-disney.jpeg', 'Rio de Janeiro', 'Parque da Disney', '2027-12-05', 'A'),

(1, '11222333000181', NULL, 5500.00, 16, 2,
'Pacote cultural em Paris para conhecer o famoso Museu do Louvre, incluindo ingresso para visitação, 
acompanhamento de guia turístico, alimentação e passeio pelas principais áreas culturais da região. Conheça importantes obras de arte, exposições históricas e a 
arquitetura de um dos museus mais conhecidos do mundo.',
NULL, 'museu-louvre.jpeg', 'Curitiba', 'Museu do Louvre', '2027-11-08', 'A'),

(1, '11222333000181', NULL, 2800.00, 30, 4,
'Pacote turístico para aproveitar a famosa praia de Copacabana, no Rio de Janeiro. 
Inclui passeios pela orla, acompanhamento de guia turístico, alimentação durante as atividades, visitas a pontos turísticos próximos e tempo livre para aproveitar a
 praia, conhecer restaurantes e registrar belas fotografias.',
NULL, 'copacabana.jpeg', 'São Paulo', 'Copacabana', '2027-07-25', 'A'),

(1, '11222333000181', NULL, 3200.00, 22, 3,
'Viagem para conhecer o Cristo Redentor, um dos maiores símbolos do Brasil.
 O pacote inclui ingresso de acesso, transporte até o ponto turístico, acompanhamento de guia, alimentação e passeio panorâmico pela região. 
 Aproveite para admirar uma das vistas mais bonitas do Rio de Janeiro e conhecer a história desse importante monumento.',
NULL, 'cristo-redentor.jpeg', 'Campinas', 'Cristo Redentor', '2027-10-15', 'A'),

(1, '11222333000181', NULL, 6200.00, 12, 4,
'Pacote completo para conhecer a Torre Eiffel, um dos monumentos mais famosos do mundo. 
Inclui ingresso para visitação, passeio panorâmico por Paris, acompanhamento de guia turístico, alimentação e tempo livre para conhecer os arredores, tirar fotografias e
 aproveitar as paisagens da capital francesa.',
NULL, 'torre-eiffel.jpeg', 'São Paulo', 'Torre Eiffel', '2027-08-20', 'A'),

(1, '11222333000181', NULL, 5800.00, 18, 3,
'Pacote especial para conhecer o famoso Monte Fuji, com transporte até a região turística, 
acompanhamento de guia, alimentação, visitas aos mirantes naturais e oportunidades para fotografar as belas paisagens japonesas.
 Uma experiência ideal para quem deseja conhecer a natureza e a cultura do Japão.',
NULL, 'monte-fuji.jpeg', 'São Paulo', 'Monte Fuji', '2027-09-05', 'A'),

(1, '11222333000181', NULL, 5100.00, 14, 2,
'Viagem cultural para conhecer o tradicional Templo Senso-ji, localizado em Tóquio. 
O pacote inclui ingresso para visitação, acompanhamento de guia turístico, alimentação e passeio pelos arredores do templo. Conheça a arquitetura tradicional japonesa, 
os costumes locais e a história de um dos templos mais importantes do Japão.',
NULL, 'templo-senso-ji.jpeg', 'Rio de Janeiro', 'Templo Senso-ji', '2027-09-18', 'A'),

(1, '11222333000181', NULL, 5300.00, 13, 3,
'Pacote turístico para conhecer Chichén Itzá, um dos mais importantes sítios arqueológicos da civilização maia.
 Inclui ingresso para visitação, transporte até o complexo histórico, acompanhamento de guia especializado, alimentação e passeio pelas construções antigas.
  Uma oportunidade de conhecer a história, a arquitetura e a cultura maia.',
NULL, 'chichen-itza.jpeg', 'São Paulo', 'Chichén Itzá', '2027-10-05', 'A'),

(1, '11222333000181', NULL, 4500.00, 19, 4,
'Viagem turística para conhecer o famoso Big Ben e os principais pontos históricos de Londres. 
O pacote inclui passeios guiados, acompanhamento turístico, alimentação e tempo livre para explorar a região, conhecer construções históricas e registrar fotografias. 
Uma experiência para aproveitar a cultura e os encantos da capital inglesa.',
NULL, 'big-ben.jpeg', 'Brasília', 'Big Ben', '2027-09-25', 'A');


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
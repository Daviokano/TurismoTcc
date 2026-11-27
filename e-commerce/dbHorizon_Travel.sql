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

INSERT INTO Parceiro
    (nomeFor, CNPJFor, emailFor, situacaoFor)
VALUES
    ('Okano Pacote', '11222333000181', 'okano@pacotes.com.br', 'A');

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
'Embarque em uma expedição extraordinária e profundamente enriquecedora pelas múltiplas facetas do Brasil, partindo de São Paulo e meticulosamente planejada para grupos de até quatro viajantes que desejam absorver a grandiosidade cultural, histórica e ecológica do país. O pacote inclui passagens aéreas domésticas em trechos estratégicos, hospedagem de categoria superior em hotéis bem localizados com café da manhã regional completo, traslados executivos climatizados entre aeroportos, hotéis e atrações, além de plano de alimentação com almoços e jantares típicos selecionados. Durante todo o itinerário, você contará com o acompanhamento integral de um guia de turismo bilíngue e historiador local, conduzindo visitas guiadas por centros históricos coloniais tombados como Patrimônio Mundial, reservas naturais exuberantes, mirantes panorâmicos e polos gastronômicos autênticos. Inclui ainda ingressos prioritários para parques nacionais e museus, seguro viagem nacional completo e suporte operacional 24 horas para garantir conforto, segurança e tranquilidade absolutos em cada etapa do roteiro.',
NULL, 'img/destino-brasil.jpg', 'São Paulo', 'Brasil', '2027-07-15', 'A'),

(1, '11222333000181', NULL, 6200.00, 12, 4,
'Vivencie uma imersão inesquecível e sofisticada pela França, com saída de São Paulo e estrutura completa voltada para famílias ou grupos de até quatro passageiros que buscam explorar a arte, a história e a alta gastronomia europeia. O pacote contempla passagens aéreas internacionais de ida e volta, acomodação em hotel parisiense charmoso próximo às principais linhas de metrô, café da manhã francês servido diariamente e traslados privativos aeroporto-hotel-aeroporto. A programação detalhada oferece transporte turístico exclusivo para passeios panorâmicos pela Cidade Luz, ingressos agendados sem fila para os monumentos e museus mais emblemáticos — incluindo Arco do Triunfo, Champs-Élysées e galerias renomadas —, além de acompanhamento de guia especializado em português. Você também desfrutará de experiências gastronômicas em bistrôs tradicionais, almoços harmonizados durante os passeios, passeio comentado pelas margens do Rio Sena, tempo livre planejado para compras e exploração independente, seguro viagem internacional com ampla cobertura e assistência dedicada em tempo integral.',
NULL, 'img/destino-franca.png', 'São Paulo', 'França', '2027-07-20', 'A'),

(1, '11222333000181', NULL, 5800.00, 18, 3,
'Descubra os contrastes fascinantes entre tradições milenares e tecnologia de ponta em um roteiro completo pelo Japão, partindo de São Paulo e desenhado sob medida para grupos de três viajantes. O pacote engloba voos internacionais, hospedagem selecionada combinando conforto contemporâneo e hospitalidade tradicional japonesa (ryokan), café da manhã diário e passes de transporte ferroviário e turístico para deslocamentos ágeis entre cidades e santuários. Acompanhado por um guia bilíngue especialista em cultura nipônica, você explorará templos históricos cercados por jardins zen, santuários xintoístas, bairros tradicionais preservados e paisagens naturais icônicas, com todos os ingressos inclusos. O itinerário abrange ainda alimentação selecionada durante as excursões, participação em cerimônia tradicional do chá, degustação da autêntica culinária local, seguro viagem internacional abrangente e suporte 24 horas em português, proporcionando uma jornada fluida, segura e culturalmente transformadora.',
NULL, 'img/destino-japao.jpg', 'São Paulo', 'Japão', '2027-09-05', 'A'),

(1, '11222333000181', NULL, 4900.00, 25, 2,
'Mergulhe na vibrante herança pré-hispânica, colonial e gastronômica do México em uma viagem completa partindo de Brasília, idealizada especialmente para casais ou duplas que buscam autenticidade e conforto. O pacote inclui passagens aéreas de ida e volta, hospedagem em hotel boutique ou resort de padrão superior com café da manhã completo, traslados executivos climatizados e plano de alimentação com refeições típicas durante as atividades programadas. Guiados por um profissional credenciado especialista em história mesoamericana, vocês visitarão complexos arqueológicos monumentais, praças históricas vibrantes, museus antropológicos e mercados tradicionais repletos de artesanato local. O roteiro equilibra perfeitamente excursões estruturadas com períodos livres estratégicos para explorar a renomada culinária mexicana reconhecida pela UNESCO, apreciar apresentações folclóricas, relaxar e absorver os costumes locais com respaldo total de seguro viagem internacional e assistência contínua.',
NULL, 'img/destino-mexico.jpg', 'Brasília', 'México', '2027-10-12', 'A'),

(1, '11222333000181', NULL, 4500.00, 19, 4,
'Explore séculos de tradição real, arquitetura icônica e efervescência cultural em um pacote turístico completo pela Inglaterra, saindo de Brasília e planejado para grupos de até quatro pessoas. O pacote contempla passagens aéreas internacionais, hospedagem estrategicamente localizada no coração de Londres com fácil acesso ao transporte público, tradicional café da manhã inglês diário e traslados privativos de chegada e partida. Durante a viagem, um guia turístico fluente em português conduzirá passeios detalhados pelos principais marcos históricos, palácios reais, praças monumentais e bairros emblemáticos, acompanhados de transporte turístico confortável e alimentação inclusa durante os passeios principais. O itinerário reserva ainda períodos livres bem distribuídos para você percorrer ruas comerciais famosas, visitar museus de renome mundial, apreciar pubs históricos e registrar fotografias inesquecíveis, contando sempre com seguro viagem internacional completo e suporte local dedicado.',
NULL, 'img/destino-inglaterra.jpg', 'Brasília', 'Inglaterra', '2027-09-25', 'A'),

(1, '11222333000181', NULL, 6800.00, 10, 2,
'Realize o sonho de vivenciar a magia dos parques da Disney nos Estados Unidos em um pacote especial saindo de São Paulo, criado exclusivamente para duplas ou casais que desejam máximo aproveitamento sem filas ou estresse logístico. O pacote inclui voos internacionais de ida e volta, hospedagem em hotel parceiro selecionado nas proximidades do complexo com café da manhã completo, traslados privativos aeroporto-hotel-parques e ingressos oficiais com acesso integral às principais áreas temáticas. Você contará com acompanhamento de guia turístico especialista em roteiros Disney, oferecendo consultoria prévia para agendamento de atrações concorridas, posicionamento estratégico para desfiles diurnos e shows noturnos de fogos de artifício, além de reservas para encontros com personagens. Inclui também plano de alimentação durante os dias de parque, seguro viagem internacional completo e suporte 24 horas em português para tornar cada momento mágico.',
NULL, 'img/disneyimg.jpg', 'São Paulo', 'Estados Unidos', '2027-11-20', 'A'),

(1, '11222333000181', NULL, 7200.00, 15, 4,
'Experiência definitiva e supercompleta nos Estados Unidos partindo do Rio de Janeiro, projetada para famílias ou grupos de até quatro viajantes que desejam explorar toda a grandiosidade do complexo da Disney com total comodidade. O pacote abrange passagens aéreas internacionais, acomodação em resort amplo e confortável com café da manhã diário, transporte turístico exclusivo diário até os portões dos parques e ingressos completos para múltiplas atrações temáticas, montanhas-russas radicais e espetáculos teatrais ao vivo. Contempla ainda alimentação completa dentro do complexo, acompanhamento de coordenador turístico bilíngue para orientar horários de shows, sessões de fotos com personagens e otimização de trajetos, além de tempo livre programado para compras nos centros comerciais locais e exploração independente. Tudo protegido por seguro viagem internacional de alta cobertura e assistência integral da nossa equipe.',
NULL, 'img/disneyimg.jpg', 'Rio de Janeiro', 'Estados Unidos', '2027-12-05', 'A'),

(1, '11222333000181', NULL, 5500.00, 16, 2,
'Imersão artística, histórica e arquitetônica pela França partindo de Curitiba, desenvolvida para duplas apaixonadas por cultura que desejam conhecer o lendário Museu do Louvre e os tesouros de Paris em profundidade. O pacote inclui passagens aéreas internacionais, hospedagem selecionada em bairro histórico parisiense com café da manhã completo, traslados executivos e transporte turístico exclusivo durante as atividades. Com ingressos prioritários de horário marcado para evitar longas filas na pirâmide de vidro, você percorrerá as principais galerias acompanhado por um guia historiador especializado em arte europeia, admirando obras-primas como a Mona Lisa, a Vênus de Milo e antiguidades egípcias e greco-romanas. O pacote inclui ainda almoços gastronômicos durante os passeios, caminhada cultural pelos Jardins das Tulherias e bairros artísticos de Paris, seguro viagem internacional completo e suporte dedicado em português.',
NULL, 'img/card paris.jpg', 'Curitiba', 'França', '2027-11-08', 'A'),

(1, '11222333000181', NULL, 2800.00, 30, 4,
'Aproveite o sol, a brisa do mar e a energia contagiante do Brasil em um pacote turístico completo para o Rio de Janeiro, saindo de São Paulo e estruturado para grupos de até quatro pessoas. O pacote inclui transporte aéreo ou rodoviário executivo de ida e volta, hospedagem confortável a poucos metros da famosa orla de Copacabana com café da manhã tropical completo, traslados e serviço de praia exclusivo com cadeiras e guarda-sol. Acompanhado por um guia turístico local credenciado, o roteiro contempla caminhadas guiadas pelo calçadão histórico, visita ao Forte de Copacabana com vista panorâmica da enseada, excursões aos pontos turísticos vizinhos de Ipanema e Arpoador, além de alimentação inclusa em restaurantes tradicionais cariocas. Você também terá tempo livre para banhos de mar, esportes na areia e experiências gastronômicas locais, com seguro viagem nacional e assistência 24 horas.',
NULL, 'img/card copacabana.jpeg', 'São Paulo', 'Brasil', '2027-07-25', 'A'),

(1, '11222333000181', NULL, 3200.00, 22, 3,
'Embarque em uma viagem inesquecível pelo Brasil partindo de Campinas rumo à Cidade Maravilhosa, com foco na visitação ao icônico Cristo Redentor, eleito uma das Sete Maravilhas do Mundo Moderno. O pacote contempla transporte completo de ida e volta, hospedagem bem localizada na Zona Sul do Rio de Janeiro com café da manhã diário, traslados climatizados até a estação do Trem do Corcovado e ingressos oficiais para a subida panorâmica através da exuberante Floresta da Tijuca. No topo, um guia turístico especializado apresentará toda a história, engenharia e curiosidades do monumento, garantindo tempo livre amplo para contemplar e fotografar a vista de 360 graus da Baía de Guanabara e do Pão de Açúcar. O roteiro inclui ainda almoço típico, city tour panorâmico pelos principais marcos históricos cariocas, seguro viagem nacional completo e suporte integral.',
NULL, 'img/brasil-procurados.jpg', 'Campinas', 'Brasil', '2027-10-15', 'A'),

(1, '11222333000181', NULL, 6200.00, 12, 4,
'Pacote completo e encantador pela França com partida de São Paulo, criado para famílias ou grupos de até quatro viajantes que desejam viver a magia de Paris com destaque para a mundialmente famosa Torre Eiffel. O pacote inclui passagens aéreas internacionais, acomodação confortável com café da manhã francês diário, traslados privativos aeroporto-hotel e transporte turístico exclusivo para city tours panorâmicos. Contempla ingressos agendados com acesso de elevador aos andares panorâmicos da Dama de Ferro, acompanhamento de guia turístico bilíngue durante toda a visita e passeio detalhado pelos arredores do Campo de Marte e Trocadéro. A experiência inclui ainda refeições selecionadas, tempo livre estratégico para sessões fotográficas, passeios culturais por bairros boêmios e cafés parisienses, além de seguro viagem internacional completo e assistência 24 horas em português.',
NULL, 'img/destino-franca.png', 'São Paulo', 'França', '2027-08-20', 'A'),

(1, '11222333000181', NULL, 5800.00, 18, 3,
'Jornada especial e contemplativa pelo Japão partindo de São Paulo, planejada para grupos de três viajantes que buscam conectar-se com a natureza sagrada e as tradições autênticas ao redor do majestoso Monte Fuji. O pacote inclui voos internacionais de ida e volta, hospedagem confortável com café da manhã diário e acesso a banhos termais tradicionais (onsen), além de transporte turístico privativo pela região dos Cinco Lagos. Acompanhado por um guia especializado em cultura e geografia japonesa, o roteiro leva você aos mirantes naturais mais deslumbrantes — como o Pagode Chureito e as margens do Lago Kawaguchiko —, inclui visitas a vilarejos históricos preservados, atividades culturais imersivas e alimentação típica durante os passeios. Conta ainda com tempo reservado para fotografias panorâmicas, seguro viagem internacional completo e suporte operacional contínuo.',
NULL, 'img/destino-japao.jpg', 'São Paulo', 'Japão', '2027-09-05', 'A'),

(1, '11222333000181', NULL, 5100.00, 14, 2,
'Viagem cultural envolvente pelo Japão com saída do Rio de Janeiro, desenhada especialmente para casais ou duplas que desejam explorar a espiritualidade e a história milenar de Tóquio a partir do tradicional Templo Senso-ji. O pacote contempla passagens aéreas internacionais, hospedagem estrategicamente situada no histórico bairro de Asakusa com café da manhã completo, traslados executivos, transporte turístico e ingressos para visitação. Acompanhado por um guia historiador especialista em arquitetura e costumes japoneses, você atravessará o imponente Portão Kaminarimon, percorrerá a tradicional rua comercial Nakamise-dori, conhecerá os pavilhões sagrados e participará de experiências culturais autênticas. O pacote inclui ainda refeições selecionadas em restaurantes tradicionais, seguro viagem internacional abrangente e suporte 24 horas em português para uma experiência impecável.',
NULL, 'img/destino-japao.jpg', 'Rio de Janeiro', 'Japão', '2027-09-18', 'A'),

(1, '11222333000181', NULL, 5300.00, 13, 3,
'Expedição arqueológica e histórica fascinante pelo México saindo de São Paulo, voltada para grupos de três viajantes que desejam desvendar os segredos de Chichén Itzá, um dos mais grandiosos sítios da civilização maia e Maravilha do Mundo Moderno. O pacote inclui passagens aéreas internacionais, hospedagem confortável com café da manhã completo, transporte executivo climatizado até o complexo arqueológico e ingressos de acesso antecipado. Durante a visita, um guia arqueólogo credenciado apresentará explicações aprofundadas sobre a astronomia, arquitetura e história maia diante da Pirâmide de Kukulcán, do Grande Campo de Jogo de Pelota e do Observatório El Caracol. O roteiro contempla ainda alimentação típica regional durante o passeio, parada refrescante em um cenote sagrado, seguro viagem internacional completo e assistência dedicada.',
NULL, 'img/destino-mexico.jpg', 'São Paulo', 'México', '2027-10-05', 'A'),

(1, '11222333000181', NULL, 4500.00, 19, 4,
'Viagem turística clássica pela Inglaterra partindo de Brasília, idealizada para grupos de até quatro pessoas que desejam vivenciar a história viva e o charme cosmopolita de Londres com destaque para o icônico Big Ben. O pacote inclui passagens aéreas internacionais, hospedagem bem localizada no centro londrino com tradicional café da manhã inglês diário, traslados privativos e transporte turístico confortável. Acompanhado por um guia turístico em português, você explorará o complexo de Westminster, contemplará a Torre Elizabeth e a Abadia de Westminster, caminhará pelas margens do Rio Tâmisa e conhecerá os principais marcos históricos da capital britânica. Inclui ainda alimentação selecionada durante as atividades, tempo livre para fotografias, compras e experiências culturais, além de seguro viagem internacional completo e suporte 24 horas.',
NULL, 'img/destino-inglaterra.jpg', 'Brasília', 'Inglaterra', '2027-09-25', 'A');

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
(1, '11222333000181', NULL, 6800.00, 10, 2,
'Embarque em uma jornada mágica e inesquecível saindo de São Paulo rumo ao universo encantado da Disney, um pacote especialmente concebido para casais ou duplas que desejam aproveitar cada segundo sem preocupações logísticas. O pacote contempla passagens aéreas de ida e volta, hospedagem selecionada nas imediações do complexo com café da manhã completo, traslados privativos aeroporto-hotel-parques e ingressos oficiais com acesso integral às principais atrações temáticas. Durante toda a estadia, você contará com o acompanhamento de um guia turístico bilíngue especializado em otimização de filas, cronograma de desfiles diurnos, espetáculos noturnos de fogos de artifício e reservas estratégicas para encontros com personagens clássicos. Inclui ainda plano de alimentação completo durante os dias de parque, seguro viagem internacional abrangente e suporte 24 horas em português, garantindo conforto, praticidade e memórias eternas em família ou a dois.',
NULL, 'img/disneyimg.jpg', 'São Paulo', 'Disney', '2027-11-20', 'A'),

(1, '11222333000181', NULL, 7200.00, 15, 4,
'Experiência completa e imersiva no Parque da Disney, desenhada especialmente para famílias ou grupos de até quatro pessoas que buscam diversão intensa com máxima comodidade saindo do Rio de Janeiro. O pacote inclui voos diretos ou com conexões otimizadas, hospedagem em resort parceiro com quartos amplos e café da manhã diário, transporte diário gratuito e climatizado até os portões dos parques e ingressos para múltiplas atrações temáticas, montanhas-russas radicais e espetáculos musicais ao vivo. Contempla ainda plano de alimentação completo dentro do complexo, consultoria prévia para agendamento de atrações prioritárias, acompanhamento de coordenador turístico durante os passeios principais e tempo livre estrategicamente distribuído para explorar diferentes áreas do parque e realizar compras. Tudo respaldado por seguro viagem internacional de ampla cobertura e assistência integral da nossa equipe local.',
NULL, 'img/disneyimg.jpg', 'Rio de Janeiro', 'Parque da Disney', '2027-12-05', 'A'),

(1, '11222333000181', NULL, 5500.00, 16, 2,
'Imersão artística e histórica no coração de Paris com partida de Curitiba, criada para duplas apaixonadas por cultura que desejam explorar o lendário Museu do Louvre em profundidade. O pacote oferece passagens aéreas internacionais, hospedagem charmosa em bairro histórico com fácil acesso ao metrô e café da manhã francês diário, traslados executivos e ingressos com horário agendado para evitar filas na icônica pirâmide de vidro. Você será acompanhado por um guia historiador especializado em arte europeia, percorrendo galerias imperdíveis para admirar obras-primas como a Mona Lisa, a Vênus de Milo, a Vitória de Samotrácia e coleções egípcias e renascentistas. A experiência inclui ainda almoço gastronômico em bistrô parisiense tradicional, passeio guiado pelas principais áreas culturais da região — incluindo Jardins das Tulherias e margens do Rio Sena —, seguro viagem internacional completo e suporte dedicado.',
NULL, 'img/card paris.jpg', 'Curitiba', 'Museu do Louvre', '2027-11-08', 'A'),

(1, '11222333000181', NULL, 2800.00, 30, 4,
'Roteiro ensolarado, vibrante e completo para aproveitar o melhor de Copacabana, a praia mais emblemática do Rio de Janeiro, com saída de São Paulo e estrutura ideal para grupos de até quatro viajantes. O pacote contempla transporte aéreo ou rodoviário executivo de ida e volta, hospedagem em hotel confortável a poucos passos do calçadão de pedras portuguesas com café da manhã tropical completo e serviço de praia exclusivo com cadeiras e guarda-sol. A programação inclui caminhada guiada pela orla histórica, visita ao Forte de Copacabana com vista panorâmica para a enseada, passeios culturais a pontos turísticos próximos como Ipanema e Arpoador, além de alimentação incluída em restaurantes tradicionais cariocas. Você também terá períodos livres para banho de mar, esportes na areia, compras em feiras locais e registros fotográficos inesquecíveis ao pôr do sol, com seguro viagem nacional.',
NULL, 'img/card copacabana.jpeg', 'São Paulo', 'Copacabana', '2027-07-25', 'A'),

(1, '11222333000181', NULL, 3200.00, 22, 3,
'Expedição turística inesquecível saindo de Campinas para conhecer de perto o Cristo Redentor, uma das Sete Maravilhas do Mundo Moderno e maior símbolo do Brasil. O pacote inclui transporte completo de ida e volta, hospedagem bem localizada na Zona Sul carioca com café da manhã diário, traslados climatizados até a estação do Trem do Corcovado e ingressos prioritários para a subida através da exuberante Floresta da Tijuca. No topo, um guia turístico credenciado apresentará detalhes históricos, arquitetônicos e curiosidades sobre a construção do monumento, garantindo tempo amplo para admirar e fotografar uma das vistas mais bonitas do Rio de Janeiro — abrangendo a Baía de Guanabara, o Pão de Açúcar e a Lagoa Rodrigo de Freitas. O roteiro abrange ainda almoço típico, passeio panorâmico pela região, seguro viagem nacional completo e assistência 24 horas.',
NULL, 'img/brasil-procurados.jpg', 'Campinas', 'Cristo Redentor', '2027-10-15', 'A'),

(1, '11222333000181', NULL, 6200.00, 12, 4,
'Pacote completo e sofisticado para vivenciar a magia da Torre Eiffel, em Paris, partindo de São Paulo e estruturado para famílias ou grupos de até quatro pessoas. O roteiro inclui passagens aéreas internacionais, acomodação confortável com café da manhã francês completo, traslados privativos aeroporto-hotel e ingressos com acesso de elevador até os andares superiores da Dama de Ferro, proporcionando vistas deslumbrantes de toda a capital francesa. A programação conta com acompanhamento de guia turístico bilíngue, passeio panorâmico pelos principais pontos da Cidade Luz — incluindo Arco do Triunfo, Champs-Élysées e Trocadéro —, jantar especial próximo ao Campo de Marte e cruzeiro comentado pelo Rio Sena ao entardecer. Inclui ainda alimentação durante os passeios, tempo livre para explorar os arredores, tirar fotografias e seguro viagem internacional completo.',
NULL, 'img/destino-franca.png', 'São Paulo', 'Torre Eiffel', '2027-08-20', 'A'),

(1, '11222333000181', NULL, 5800.00, 18, 3,
'Jornada fascinante rumo ao majestoso Monte Fuji, o pico mais alto e sagrado do Japão, com saída de São Paulo e roteiro pensado para grupos de três viajantes que buscam contato profundo com a natureza e as tradições nipônicas. O pacote inclui passagens aéreas internacionais, hospedagem em hotel tradicional com café da manhã completo e acesso a águas termais (onsen), transporte turístico confortável pela região dos Cinco Lagos e acompanhamento de guia bilíngue especializado. Durante a viagem, você visitará mirantes naturais icônicos como o Pagode Chureito e o Lago Kawaguchiko, fará passeios por vilarejos históricos preservados e desfrutará de alimentação típica da culinária regional japonesa. O pacote contempla ainda ingressos para atrações naturais, oportunidades exclusivas para fotografar as paisagens japonesas, seguro viagem internacional completo e suporte integral.',
NULL, 'img/destino-japao.jpg', 'São Paulo', 'Monte Fuji', '2027-09-05', 'A'),

(1, '11222333000181', NULL, 5100.00, 14, 2,
'Viagem cultural envolvente partindo do Rio de Janeiro com destino a Tóquio para explorar o histórico Templo Senso-ji, o santuário budista mais antigo e venerado da capital japonesa. O pacote contempla voos internacionais, hospedagem estratégica no bairro tradicional de Asakusa com café da manhã diário, traslados eficientes, passes de transporte urbano, ingressos para visitação e acompanhamento de guia especializado na história, arquitetura e espiritualidade do Japão. A experiência inclui passagem pelo imponente Portão Kaminarimon, caminhada guiada pela movimentada rua comercial Nakamise-dori para degustação de quitutes típicos e compra de artesanatos, cerimônia tradicional do chá, visita aos jardins internos do templo e alimentação selecionada em restaurantes locais. Ideal para duplas que desejam mergulhar nos costumes locais e nos contrastes do Japão com total segurança.',
NULL, 'img/destino-japao.jpg', 'Rio de Janeiro', 'Templo Senso-ji', '2027-09-18', 'A'),

(1, '11222333000181', NULL, 5300.00, 13, 3,
'Expedição arqueológica e cultural saindo de São Paulo para desvendar os mistérios de Chichén Itzá, na Península de Yucatán, um dos mais grandiosos sítios arqueológicos da antiga civilização maia e Patrimônio Mundial da UNESCO. O pacote inclui passagens aéreas internacionais, hospedagem confortável com café da manhã completo, transporte executivo climatizado até o complexo histórico, ingressos de acesso antecipado e acompanhamento de guia arqueólogo especializado. Você conhecerá de perto a imponente Pirâmide de Kukulcán, o Grande Campo de Jogo de Pelota, o Observatório El Caracol e o Templo dos Guerreiros, compreendendo a fundo os avanços astronômicos, arquitetônicos e culturais desse povo milenar. O roteiro inclui ainda banho refrescante em um cenote sagrado de águas cristalinas, alimentação típica da culinária yucateca, seguro viagem internacional e suporte dedicado.',
NULL, 'img/destino-mexico.jpg', 'São Paulo', 'Chichén Itzá', '2027-10-05', 'A'),

(1, '11222333000181', NULL, 6000.00, 15, 2,
'Vivencie uma jornada inesquecível pelo berço do Renascimento e do Império Romano em um pacote turístico completo pela Itália, partindo de São Paulo e meticulosamente planejado para casais ou duplas que buscam arte, história e alta gastronomia. O pacote contempla passagens aéreas internacionais de ida e volta, hospedagem selecionada em hotéis charmosos bem localizados com café da manhã italiano diário, traslados executivos aeroporto-hotel e deslocamentos confortáveis em trens de alta velocidade entre as principais cidades históricas. Acompanhado por um guia historiador fluente em português, você explorará monumentos milenares, praças icônicas, basílicas monumentais e museus mundialmente famosos, com ingressos prioritários agendados sem fila. A experiência inclui ainda degustações gastronômicas autênticas, refeições selecionadas em trattorias tradicionais, passeios guiados por centros históricos tombados como Patrimônio Mundial da UNESCO, tempo livre para compras e exploração independente, seguro viagem internacional completo e suporte 24 horas.',
NULL, 'img/destino-italia.png', 'São Paulo', 'Itália', '2027-09-12', 'A'),

(1, '11222333000181', NULL, 4500.00, 19, 4,
'Roteiro clássico pela realeza e história britânica com saída de Brasília para conhecer o icônico Big Ben e os monumentos mais emblemáticos de Londres, estruturado para grupos de até quatro passageiros. O pacote inclui passagens aéreas internacionais, hospedagem bem localizada próxima ao centro histórico, tradicional café da manhã inglês diário, traslados aeroporto-hotel e acompanhamento de guia turístico bilíngue. A programação abrange caminhada guiada pela região de Westminster, contemplação externa do Palácio de Westminster e da Torre Elizabeth, visita aos arredores da Abadia de Westminster, travessia pela Ponte de Westminster sobre o Rio Tâmisa e acompanhamento da tradicional cerimônia da Troca da Guarda no Palácio de Buckingham. Conta ainda com alimentação incluída durante os passeios, tempo livre para explorar construções históricas, registrar fotografias e seguro viagem internacional completo.',
NULL, 'img/destino-inglaterra.jpg', 'Brasília', 'Big Ben', '2027-09-25', 'A');
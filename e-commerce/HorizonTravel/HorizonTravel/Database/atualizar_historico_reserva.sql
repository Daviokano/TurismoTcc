USE dbHorizon_Travel;

DROP TRIGGER IF EXISTS trg_HistoricoReserva;

ALTER TABLE Historico
    ADD COLUMN statusNovo VARCHAR(40) NULL AFTER statusAnterior,
    ADD COLUMN IDFun INT NULL AFTER IDRes,
    ADD CONSTRAINT FK_HistoricoFuncionario
        FOREIGN KEY (IDFun) REFERENCES Funcionario(IDFun);

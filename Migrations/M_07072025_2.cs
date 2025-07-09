using FluentMigrator;

[Migration(202507072000)]
public class CreateLoanProcedures : Migration
{
    public override void Up()
    {
        Execute.Sql(@"
            DROP PROCEDURE IF EXISTS sp_CreateLoan;
            CREATE PROCEDURE sp_CreateLoan(
                IN p_UserId INT,
                IN p_LoanTypeId INT,
                IN p_StatusId INT,
                IN p_Amount DECIMAL(10,2),
                IN p_CurrencyId INT,
                IN p_MonthsTerm INT,
                IN p_CreatedAt DATETIME
            )
            BEGIN
                INSERT INTO Loans (UserId, LoanTypeId, StatusId, Amount, CurrencyId, MonthsTerm, CreatedAt)
                VALUES (p_UserId, p_LoanTypeId, p_StatusId, p_Amount, p_CurrencyId, p_MonthsTerm, p_CreatedAt);
                SELECT LAST_INSERT_ID() AS NewLoanId;
            END;
        ");

        Execute.Sql(@"
            DROP PROCEDURE IF EXISTS sp_GetLoansByUserId;
            CREATE PROCEDURE sp_GetLoansByUserId(IN p_UserId INT)
            BEGIN
                SELECT 
                    l.*, 
                    t.Id AS LoanTypeId, t.Name AS Name, 
                    s.Id AS StatusId, s.Name AS Name,
                    c.Id AS CurrencyId, c.Code AS Code, c.Name AS Name 
                FROM Loans l
                JOIN LoanTypes t ON l.LoanTypeId = t.Id
                JOIN LoanStatuses s ON l.StatusId = s.Id
                JOIN Currencies c ON l.CurrencyId = c.Id
                WHERE l.UserId = p_UserId
                ORDER BY l.Id DESC;
            END;
        ");

        Execute.Sql(@"
            DROP PROCEDURE IF EXISTS sp_GetLoanById;
            CREATE PROCEDURE sp_GetLoanById(IN p_LoanId INT)
            BEGIN
                SELECT 
                    l.*, 
                    t.Id AS LoanTypeId, t.Name AS Name, 
                    s.Id AS StatusId, s.Name AS Name,
                    c.Id AS CurrencyId, c.Code AS Code, c.Name AS Name 
                FROM Loans l
                JOIN LoanTypes t ON l.LoanTypeId = t.Id
                JOIN LoanStatuses s ON l.StatusId = s.Id
                JOIN Currencies c ON l.CurrencyId = c.Id
                WHERE l.Id = p_LoanId;
            END;
        ");

        Execute.Sql(@"
            DROP PROCEDURE IF EXISTS sp_UpdateLoanStatus;
            CREATE PROCEDURE sp_UpdateLoanStatus(IN p_LoanId INT, IN p_StatusId INT)
            BEGIN
                UPDATE Loans SET StatusId = p_StatusId WHERE Id = p_LoanId;
            END;
        ");

        Execute.Sql(@"
            DROP PROCEDURE IF EXISTS sp_GetLoanTypes;
            CREATE PROCEDURE sp_GetLoanTypes()
            BEGIN
                SELECT * FROM LoanTypes ORDER BY Id ASC;
            END;
        ");

        Execute.Sql(@"
            DROP PROCEDURE IF EXISTS sp_GetLoanStatuses;
            CREATE PROCEDURE sp_GetLoanStatuses()
            BEGIN
                SELECT * FROM LoanStatuses ORDER BY Id ASC;
            END;
        ");

        Execute.Sql(@"
            DROP PROCEDURE IF EXISTS sp_GetLoansByStatus;
            CREATE PROCEDURE sp_GetLoansByStatus(IN p_StatusId INT)
            BEGIN
                SELECT 
                    l.*, 
                    t.Id AS LoanTypeId, t.Name AS Name, 
                    s.Id AS StatusId, s.Name AS Name,
                    c.Id AS CurrencyId, c.Code AS Code, c.Name AS Name 
                FROM Loans l
                JOIN LoanTypes t ON l.LoanTypeId = t.Id
                JOIN LoanStatuses s ON l.StatusId = s.Id
                JOIN Currencies c ON l.CurrencyId = c.Id
                WHERE StatusId = p_StatusId;
            END;
        ");

        Execute.Sql(@"
            DROP PROCEDURE IF EXISTS sp_GetCurrencies;
            CREATE PROCEDURE sp_GetCurrencies()
            BEGIN
                SELECT * FROM Currencies ORDER BY Id ASC;
            END;
        ");

        Execute.Sql(@"
            DROP PROCEDURE IF EXISTS sp_UpdateLoan;
            CREATE PROCEDURE sp_UpdateLoan(
                IN p_Id INT,
                IN p_LoanTypeId INT,
                IN p_Amount DECIMAL(10,2),
                IN p_CurrencyId INT,
                IN p_MonthsTerm INT
            )
            BEGIN
                UPDATE Loans
                SET 
                    LoanTypeId = p_LoanTypeId,
                    Amount = p_Amount,
                    CurrencyId = p_CurrencyId,
                    MonthsTerm = p_MonthsTerm
                WHERE Id = p_Id;

            SELECT p_Id AS UpdatedLoanId;
            END;
         ");


        Execute.Sql(@"
            DROP PROCEDURE IF EXISTS sp_DeleteLoan;
            CREATE PROCEDURE sp_DeleteLoan(IN p_LoanId INT)
            BEGIN
                DELETE FROM Loans WHERE Id = p_LoanId;
            END;
        ");

    }

    public override void Down()
    {
        Execute.Sql("DROP PROCEDURE IF EXISTS sp_CreateLoan;");
        Execute.Sql("DROP PROCEDURE IF EXISTS sp_GetLoansByUserId;");
        Execute.Sql("DROP PROCEDURE IF EXISTS sp_GetLoanById;");
        Execute.Sql("DROP PROCEDURE IF EXISTS sp_UpdateLoanStatus;");
        Execute.Sql("DROP PROCEDURE IF EXISTS sp_GetLoanTypes;");
        Execute.Sql("DROP PROCEDURE IF EXISTS sp_GetLoanStatuses;");
        Execute.Sql("DROP PROCEDURE IF EXISTS sp_GetCurrencies;");
        Execute.Sql("DROP PROCEDURE IF EXISTS sp_UpdateLoan;");
        Execute.Sql("DROP PROCEDURE IF EXISTS sp_DeleteLoan;");
    }
}


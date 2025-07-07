using FluentMigrator;

[Migration(202507081200)]
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
                IN p_Currency VARCHAR(10),
                IN p_MonthlyPeriod INT,
                IN p_CreatedAt DATETIME
            )
            BEGIN
                INSERT INTO Loans (UserId, LoanTypeId, StatusId, Amount, Currency, MonthlyPeriod, CreatedAt)
                VALUES (p_UserId, p_LoanTypeId, p_StatusId, p_Amount, p_Currency, p_MonthlyPeriod, p_CreatedAt);
                SELECT LAST_INSERT_ID() AS NewLoanId;
            END;
        ");

        Execute.Sql(@"
            DROP PROCEDURE IF EXISTS sp_GetLoansByUserId;
            CREATE PROCEDURE sp_GetLoansByUserId(IN p_UserId INT)
            BEGIN
                SELECT l.*, t.Id AS LoanTypeId, t.Name AS LoanTypeName, s.Id AS StatusId, s.Name AS StatusName
                FROM Loans l
                JOIN LoanTypes t ON l.LoanTypeId = t.Id
                JOIN LoanStatuses s ON l.StatusId = s.Id
                WHERE l.UserId = p_UserId;
            END;
        ");

        Execute.Sql(@"
            DROP PROCEDURE IF EXISTS sp_GetLoanById;
            CREATE PROCEDURE sp_GetLoanById(IN p_LoanId INT)
            BEGIN
                SELECT l.*, t.Id AS LoanTypeId, t.Name AS LoanTypeName, s.Id AS StatusId, s.Name AS StatusName
                FROM Loans l
                JOIN LoanTypes t ON l.LoanTypeId = t.Id
                JOIN LoanStatuses s ON l.StatusId = s.Id
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
                SELECT * FROM LoanTypes;
            END;
        ");

        Execute.Sql(@"
            DROP PROCEDURE IF EXISTS sp_GetLoanStatuses;
            CREATE PROCEDURE sp_GetLoanStatuses()
            BEGIN
                SELECT * FROM LoanStatuses;
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
    }
}


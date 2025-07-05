using FluentMigrator;

[Migration(202507062000)]
public class CreateUserProcedures : Migration
{
    public override void Up()
    {
        Execute.Sql(@"
            DROP PROCEDURE IF EXISTS sp_GetUserByEmail;
            CREATE PROCEDURE sp_GetUserByEmail(IN userEmail VARCHAR(255))
            BEGIN
                SELECT * FROM Users WHERE Email = userEmail LIMIT 1;
            END;
        ");

        Execute.Sql(@"
            DROP PROCEDURE IF EXISTS sp_EmailExists;
            CREATE PROCEDURE sp_EmailExists(IN userEmail VARCHAR(255))
            BEGIN
                SELECT COUNT(1) FROM Users WHERE Email = userEmail;
            END;
        ");

        Execute.Sql(@"
            DROP PROCEDURE IF EXISTS sp_CreateUser;
            CREATE PROCEDURE sp_CreateUser(
                IN firstName VARCHAR(100),
                IN lastName VARCHAR(100),
                IN idNumber VARCHAR(50),
                IN dateOfBirth DATE,
                IN email VARCHAR(255),
                IN passwordHash VARCHAR(255),
                IN role VARCHAR(50),
                IN createdAt DATETIME
            )
            BEGIN
                INSERT INTO Users (FirstName, LastName, IdNumber, DateOfBirth, Email, PasswordHash, Role, CreatedAt)
                VALUES (firstName, lastName, idNumber, dateOfBirth, email, passwordHash, role, createdAt);
                SELECT LAST_INSERT_ID() AS NewId;
            END;
        ");
    }

    public override void Down()
    {
        Execute.Sql("DROP PROCEDURE IF EXISTS sp_GetUserByEmail;");
        Execute.Sql("DROP PROCEDURE IF EXISTS sp_EmailExists;");
        Execute.Sql("DROP PROCEDURE IF EXISTS sp_CreateUser;");
    }
}


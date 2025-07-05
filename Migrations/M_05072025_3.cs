using FluentMigrator;

[Migration(202507063000)]
public class SeedAdminUser : Migration
{
    public override void Up()
    {
        // Hash the admin password using BCrypt
        var hash = BCrypt.Net.BCrypt.HashPassword("admin123");


        Execute.Sql($@"
        CALL sp_CreateUser(
          'Admin',
          'User',
          'ADMIN-001',
          '1980-01-01',
          'admin@example.com',
          '{hash}',
          'Admin',
          NOW()
          );
        ");

    }

    public override void Down()
    {
        Execute.Sql("DELETE FROM Users WHERE Email = 'admin@example.com';");
    }
}


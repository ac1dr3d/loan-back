using FluentMigrator;

[Migration(202507071000)]
public class CreateLoanTables : Migration
{
    public override void Up()
    {
        // LoanTypes
        Create.Table("LoanTypes")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("Name").AsString(50).NotNullable().Unique();

        // LoanStatuses
        Create.Table("LoanStatuses")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("Name").AsString(50).NotNullable().Unique();

        // Currencies
        Create.Table("Currencies")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("Code").AsString(3).NotNullable().Unique()
            .WithColumn("Name").AsString(50).NotNullable();

        // Loans
        Create.Table("Loans")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("UserId").AsInt32().NotNullable()
            .WithColumn("LoanTypeId").AsInt32().NotNullable()
            .WithColumn("StatusId").AsInt32().NotNullable()
            .WithColumn("CurrencyId").AsInt32().NotNullable()
            .WithColumn("Amount").AsDecimal(10, 2).NotNullable()
            .WithColumn("MonthsTerm").AsInt32().NotNullable() // ← renamed here
            .WithColumn("CreatedAt").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);

        // Foreign Keys
        Create.ForeignKey("FK_Loans_User")
            .FromTable("Loans").ForeignColumn("UserId")
            .ToTable("Users").PrimaryColumn("Id");

        Create.ForeignKey("FK_Loans_LoanType")
            .FromTable("Loans").ForeignColumn("LoanTypeId")
            .ToTable("LoanTypes").PrimaryColumn("Id");

        Create.ForeignKey("FK_Loans_LoanStatus")
            .FromTable("Loans").ForeignColumn("StatusId")
            .ToTable("LoanStatuses").PrimaryColumn("Id");

        Create.ForeignKey("FK_Loans_Currency")
            .FromTable("Loans").ForeignColumn("CurrencyId")
            .ToTable("Currencies").PrimaryColumn("Id");

        // Seed Loan Types
        Insert.IntoTable("LoanTypes").Row(new { Name = "სწრაფი სესხი" });
        Insert.IntoTable("LoanTypes").Row(new { Name = "ავტო სესხი" });
        Insert.IntoTable("LoanTypes").Row(new { Name = "განვადება" });

        // Seed Loan Statuses
        Insert.IntoTable("LoanStatuses").Row(new { Name = "ახალი" });
        Insert.IntoTable("LoanStatuses").Row(new { Name = "გადაგზავნილი" });
        Insert.IntoTable("LoanStatuses").Row(new { Name = "დამუშავების პროცესში" });
        Insert.IntoTable("LoanStatuses").Row(new { Name = "დამტკიცებული" });
        Insert.IntoTable("LoanStatuses").Row(new { Name = "უარყოფილი" });

        // Seed Currencies
        Insert.IntoTable("Currencies").Row(new { Code = "GEL", Name = "ლარი" });
        Insert.IntoTable("Currencies").Row(new { Code = "USD", Name = "აშშ დოლარი" });
        Insert.IntoTable("Currencies").Row(new { Code = "EUR", Name = "ევრო" });
    }

    public override void Down()
    {
        Delete.Table("Loans");
        Delete.Table("LoanTypes");
        Delete.Table("LoanStatuses");
        Delete.Table("Currencies");
    }
}


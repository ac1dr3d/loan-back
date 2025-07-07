using FluentMigrator;

[Migration(202507080000)]
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

        // Loans
        Create.Table("Loans")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("UserId").AsInt32().NotNullable()
            .WithColumn("LoanTypeId").AsInt32().NotNullable()
            .WithColumn("StatusId").AsInt32().NotNullable()
            .WithColumn("Amount").AsDecimal(10, 2).NotNullable()
            .WithColumn("Currency").AsString(10).NotNullable()
            .WithColumn("MonthlyPeriod").AsInt32().NotNullable()
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

        // Seed Types
        Insert.IntoTable("LoanTypes").Row(new { Name = "სწრაფი სესხი" });
        Insert.IntoTable("LoanTypes").Row(new { Name = "ავტო სესხი" });
        Insert.IntoTable("LoanTypes").Row(new { Name = "განვადება" });

        // Seed Statuses
        Insert.IntoTable("LoanStatuses").Row(new { Name = "ახალი" });
        Insert.IntoTable("LoanStatuses").Row(new { Name = "გადაგზავნილი" });
        Insert.IntoTable("LoanStatuses").Row(new { Name = "დამტკიცებული" });
        Insert.IntoTable("LoanStatuses").Row(new { Name = "უარყოფილი" });
    }

    public override void Down()
    {
        Delete.Table("Loans");
        Delete.Table("LoanTypes");
        Delete.Table("LoanStatuses");
    }
}


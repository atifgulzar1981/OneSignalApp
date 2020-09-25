using FluentMigrator;

namespace OneSignalApp.Migrations
{
  [Migration(20200925222325)]
  public class M_20200925222325_InitialSchema : Migration
  {
    public override void Up()
    {
      Create.Table("Users")
        .WithColumn("Id").AsInt32().PrimaryKey().Identity()
        .WithColumn("FullName").AsString(150).NotNullable()
        .WithColumn("Email").AsString(150).NotNullable()
        .WithColumn("Password").AsString(150).NotNullable()
        .WithColumn("UserType").AsInt32().NotNullable()
        ;
    }

    public override void Down()
    {
      throw new System.NotImplementedException();
    }
  }
}
namespace SecureParcel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class up3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Parcels", "Comment", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Parcels", "Comment");
        }
    }
}

namespace SecureParcel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class up1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Messages",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    Date = c.DateTime(nullable: false, precision: 0),
                    Text = c.String(unicode: false),
                    Parcel_ID = c.Int(),
                    User_Id = c.String(maxLength: 128, storeType: "nvarchar"),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Parcels", t => t.Parcel_ID)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id);
            Sql("CREATE index `IX_Parcel_ID` on `Messages` (`Parcel_ID`)");
            Sql("CREATE index `IX_User_Id` on `Messages` (`User_Id`)");

            CreateTable(
                "dbo.Parcels",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    GUID = c.String(unicode: false),
                    CreatedAt = c.DateTime(nullable: false, precision: 0),
                    Name = c.String(unicode: false),
                    SenderAddress = c.String(unicode: false),
                    SenderName = c.String(unicode: false),
                    RecipientPublicKey = c.String(unicode: false),
                    RecipientAddress = c.String(unicode: false),
                    RecipientName = c.String(unicode: false),
                    SafeAccount = c.String(unicode: false),
                    PaymentAmount = c.String(unicode: false),
                    PaymentDate = c.String(unicode: false),
                    ShipmentDate = c.String(unicode: false),
                    TrackNumber = c.String(unicode: false),
                    DeliveryStatus = c.Int(nullable: false),
                    Sender_Id = c.String(maxLength: 128, storeType: "nvarchar"),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.Sender_Id);
            Sql("CREATE index `IX_Sender_Id` on `Parcels` (`Sender_Id`)");

            CreateTable(
                "dbo.AspNetUsers",
                c => new
                {
                    Id = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    FirstName = c.String(nullable: false, unicode: false),
                    LastName = c.String(nullable: false, unicode: false),
                    IsActivated = c.Boolean(nullable: false),
                    FullName = c.String(unicode: false),
                    PublicKey = c.String(unicode: false),
                    PrivateKey = c.String(unicode: false),
                    Address = c.String(unicode: false),
                    Email = c.String(maxLength: 256, storeType: "nvarchar"),
                    EmailConfirmed = c.Boolean(nullable: false),
                    PasswordHash = c.String(unicode: false),
                    SecurityStamp = c.String(unicode: false),
                    PhoneNumber = c.String(unicode: false),
                    PhoneNumberConfirmed = c.Boolean(nullable: false),
                    TwoFactorEnabled = c.Boolean(nullable: false),
                    LockoutEndDateUtc = c.DateTime(precision: 0),
                    LockoutEnabled = c.Boolean(nullable: false),
                    AccessFailedCount = c.Int(nullable: false),
                    UserName = c.String(nullable: false, maxLength: 256, storeType: "nvarchar"),
                })
                .PrimaryKey(t => t.Id);
            Sql("CREATE index `IX_UserName` on `AspNetUsers` (`UserName`)");

            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    UserId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    ClaimType = c.String(unicode: false),
                    ClaimValue = c.String(unicode: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true);
            Sql("CREATE index `IX_UserId` on `AspNetUserClaims` (`UserId`)");

            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                {
                    LoginProvider = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    ProviderKey = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    UserId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true);
            Sql("CREATE index `IX_UserId` on `AspNetUserLogins` (`UserId`)");

            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                {
                    UserId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    RoleId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true);
            Sql("CREATE index `IX_UserId` on `AspNetUserRoles` (`UserId`)");
            Sql("CREATE index `IX_RoleId` on `AspNetUserRoles` (`RoleId`)");

            CreateTable(
                "dbo.TrackEvents",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    Date = c.DateTime(nullable: false, precision: 0),
                    Text = c.String(unicode: false),
                    Parcel_ID = c.Int(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Parcels", t => t.Parcel_ID);
            Sql("CREATE index `IX_Parcel_ID` on `TrackEvents` (`Parcel_ID`)");

            CreateTable(
                "dbo.AspNetRoles",
                c => new
                {
                    Id = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    Name = c.String(nullable: false, maxLength: 256, storeType: "nvarchar"),
                })
                .PrimaryKey(t => t.Id);
            Sql("CREATE index `IX_Name` on `AspNetRoles` (`Name`)");

        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Messages", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.TrackEvents", "Parcel_ID", "dbo.Parcels");
            DropForeignKey("dbo.Parcels", "Sender_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Messages", "Parcel_ID", "dbo.Parcels");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.TrackEvents", new[] { "Parcel_ID" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Parcels", new[] { "Sender_Id" });
            DropIndex("dbo.Messages", new[] { "User_Id" });
            DropIndex("dbo.Messages", new[] { "Parcel_ID" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.TrackEvents");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Parcels");
            DropTable("dbo.Messages");
        }
    }
}

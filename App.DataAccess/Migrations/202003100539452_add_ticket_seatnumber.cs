namespace App.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_ticket_seatnumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("public.Tickets", "SeatNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("public.Tickets", "SeatNumber");
        }
    }
}

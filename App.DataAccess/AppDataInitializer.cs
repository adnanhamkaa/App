using System.Data.Entity;

namespace App.DataAccess
{
    public class AppDataInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext> {
        protected override void Seed(ApplicationDbContext context) {
            //AsalPerubahan(context);
            //CaType(context);

            //context.SaveChanges();

        }

        //private void CaType(ApplicationDbContext context) {
        //    var catype = new List<CorporateActionType>();

        //    catype.Add(new CorporateActionType() {
        //        Id = "1",
        //        Name = "Deviden Tunai",
        //        CreatedBy = "SYSTEM",
        //        CreatedDate = DateTime.Now
        //    });

        //    catype.Add(new CorporateActionType() {
        //        Id = "2",
        //        Name = "Stock Split",
        //        CreatedBy = "SYSTEM",
        //        CreatedDate = DateTime.Now
        //    });

        //    catype.Add(new CorporateActionType() {
        //        Id = "3",
        //        Name = "Saham Bonus",
        //        CreatedBy = "SYSTEM",
        //        CreatedDate = DateTime.Now
        //    });

        //    catype.Add(new CorporateActionType() {
        //        Id = "4",
        //        Name = "Deviden Saham",
        //        CreatedBy = "SYSTEM",
        //        CreatedDate = DateTime.Now
        //    });

        //    catype.Add(new CorporateActionType() {
        //        Id = "5",
        //        Name = "Reverse Stock",
        //        CreatedBy = "SYSTEM",
        //        CreatedDate = DateTime.Now
        //    });

        //    catype.Add(new CorporateActionType() {
        //        Id = "6",
        //        Name = "Deviden Interim",
        //        CreatedBy = "SYSTEM",
        //        CreatedDate = DateTime.Now
        //    });

        //    catype.Add(new CorporateActionType() {
        //        Id = "7",
        //        Name = "HMETD",
        //        CreatedBy = "SYSTEM",
        //        CreatedDate = DateTime.Now
        //    });
            
        //    foreach (var item in catype) {
        //        if (!context.CorporateActionTypes.Any(t => t.Id == item.Id)) {
        //            context.CorporateActionTypes.Add(item);
        //        }
        //    }
        //}

        //private void Margin(ApplicationDbContext context) {
        //    var catype = new List<Margin>();

        //    catype.Add(new Margin() {
        //        Id = "1",
        //        MarginName = "Margin 1",
        //        CreatedBy = "SYSTEM",
        //        CreatedDate = DateTime.Now
        //    });

        //    catype.Add(new Margin() {
        //        Id = "2",
        //        MarginName = "Margin 2",
        //        CreatedBy = "SYSTEM",
        //        CreatedDate = DateTime.Now
        //    });

        //    catype.Add(new Margin() {
        //        Id = "3",
        //        MarginName = "Shortsell",
        //        CreatedBy = "SYSTEM",
        //        CreatedDate = DateTime.Now
        //    });

        //    catype.Add(new Margin() {
        //        Id = "4",
        //        MarginName = "Unmargin",
        //        CreatedBy = "SYSTEM",
        //        CreatedDate = DateTime.Now
        //    });

        //    catype.Add(new Margin() {
        //        Id = "5",
        //        MarginName = "ETP",
        //        CreatedBy = "SYSTEM",
        //        CreatedDate = DateTime.Now
        //    });

        //    foreach (var item in catype) {
        //        if (!context.Margin.Any(t => t.Id == item.Id) && !context.Margin.Any(t => t.MarginName == item.MarginName)) {
        //            context.Margin.Add(item);
        //        }
        //    }
        //}


        //private void AsalPerubahan(ApplicationDbContext context) {

        //    var asalperubahans = new List<AsalPerubahan>();
        //    asalperubahans.Add(new AsalPerubahan() {
        //        Id = "37e4eca9-f068-466b-a348-746170b65625",
        //        AsalPerubahanName = "Saham Sendiri",
        //        CreatedBy = "SYSTEM",
        //        CreatedDate = DateTime.Now
        //    });

        //    asalperubahans.Add(new AsalPerubahan() {
        //        Id = "e8ec53b3-c39c-46ab-8ab5-a90249e902f0",
        //        AsalPerubahanName = "Prefered Stok",
        //        CreatedBy = "SYSTEM",
        //        CreatedDate = DateTime.Now
        //    });

        //    asalperubahans.Add(new AsalPerubahan() {
        //        Id = "a8e640cf-3875-4e99-9d1f-b77452cbf59b",
        //        AsalPerubahanName = "Konversi HMETD",
        //        CreatedBy = "SYSTEM",
        //        CreatedDate = DateTime.Now
        //    });

        //    asalperubahans.Add(new AsalPerubahan() {
        //        Id = "37e4eca9-f068-466b-a348-746170b65625",
        //        AsalPerubahanName = "Saham Sendiri",
        //        CreatedBy = "SYSTEM",
        //        CreatedDate = DateTime.Now
        //    });

        //    asalperubahans.Add(new AsalPerubahan() {
        //        Id = "e285c5d5-b532-4e3c-822f-ff2d22aac038",
        //        AsalPerubahanName = "Konversi HMS",
        //        CreatedBy = "SYSTEM",
        //        CreatedDate = DateTime.Now
        //    });

        //    asalperubahans.Add(new AsalPerubahan() {
        //        Id = "9f7269ab-0f7d-4b30-8f74-c662b04b8f74",
        //        AsalPerubahanName = "Konversi Waran",
        //        CreatedBy = "SYSTEM",
        //        CreatedDate = DateTime.Now
        //    });

        //    asalperubahans.Add(new AsalPerubahan() {
        //        Id = "3bc43d3b-92cb-47d1-ac60-ee3c12ae3c8f",
        //        AsalPerubahanName = "Konversi Opsi",
        //        CreatedBy = "SYSTEM",
        //        CreatedDate = DateTime.Now
        //    });

        //    asalperubahans.Add(new AsalPerubahan() {
        //        Id = "0509b279-0829-47eb-b37c-e7f7fd0494e2",
        //        AsalPerubahanName = "Konversi Obligasi",
        //        CreatedBy = "SYSTEM",
        //        CreatedDate = DateTime.Now
        //    });


        //    foreach (var item in asalperubahans) {
        //        if(!context.AsalPerubahans.Any(t => t.Id == item.Id)) {
        //            context.AsalPerubahans.Add(item);
        //        }
        //    }
        //}
    }
}

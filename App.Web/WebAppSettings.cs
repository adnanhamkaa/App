using App.Web.Utilities;
using System;
using System.Configuration;

namespace App.Web {
    public static class WebAppSettings {
        public static string DbMigrationSecret => ConfigurationManager.AppSettings["App:DbMigrationSecret"] ?? "";
        public static bool DbMigrationOnStart => bool.Parse(ConfigurationManager.AppSettings["App:DbMigrationOnStart"]);

        //SMTP Setting
        public static string SMTPClientHost => ConfigurationManager.AppSettings["App:SMTPHostname"] ?? "";       
        public static int SMTPClientPort => Int32.Parse(ConfigurationManager.AppSettings["App:SMTPClientPort"]);
        public static string SMTPUsername => ConfigurationManager.AppSettings["App:SMTPUsername"] ?? "";
        public static string SMTPPassword => ConfigurationManager.AppSettings["App:SMTPPassword"] ?? "";
        public static string ICSStorage => ConfigurationManager.AppSettings["App:ICSStorage"] ?? "";
        public static bool UseSSL => ConfigurationManager.AppSettings["SMPTUseSSL"].SafeConvert<bool>();

        public static string EquityInstrumentName => ConfigurationManager.AppSettings["App:EquityInstrumentName"] ?? "";
        public static string NotasiKhususJobSetting => ConfigurationManager.AppSettings["App:NotasiKhususToWebsiteCron"] ?? "0 8 * * *";
        public static string EquityBackupJobSetting => ConfigurationManager.AppSettings["App:EquityBackupService"] ?? "0 23 * * *";

        public static string AppReminderJobSetting => ConfigurationManager.AppSettings["App:ReminderJobCron"] ?? "1 0 * * *";

        public static string APPDomain => ConfigurationManager.AppSettings["App:Domain"];

        public static string DocStorage => ConfigurationManager.AppSettings["docstorage"];

        public static int SessionTimeout => ConfigurationManager.AppSettings["App:SessionTimeout"].SafeConvert<int>();
    }
}
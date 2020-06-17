using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.Utilities {
    public enum AppReminderTypes {
        Reminder_Equity_Corporate_Action,

        Reminder_Equity_Formulir_CA_Cum_Tunai,
        Reminder_Equity_Formulir_CA_Cum_Reguler,
        Reminder_Equity_Formulir_CA_Baru,
        Reminder_Equity_Formulir_CA_Delete_Remarks,
        Reminder_Equity_Pengumuman_Harga_Teoritis_CA,
        Reminder_Equity_Pengumuman_Mengingatkan_CA,
        Reminder_Equity_Pengumuman_Peniadaan_Perdagangan_CA_SS_dan_RS,
        Reminder_Equity_Formulir_Pencatatan_Efek_HMETD,
        Reminder_Equity_Formulir_Suspend_di_Pasar_Tunai,
        Reminder_Equity_Formulir_Unsuspend_di_Pasar_Tunai,

        Reminder_Equity_Lockup,
        Reminder_Equity_PJE_From_IDXNET,
        Reminder_Equity_CA_From_IDXNET,
        Reminder_Equity_Maturity_Date,
        Reminder_Equity_Suspend_Active,
        Reminder_Equity_Suspend_Waran,
        Reminder_Equity_Inisiasi_Remarks,
        Reminder_Derivative_KOS,
        Reminder_Derivative_Future,
        Reminder_Fixed_Income_Jatuh_Tempo_EBUS,
        Reminder_Fixed_Income_Coupon_Period,
        Reminder_Fixed_Income_ListingFITS,
        Reminder_Fixed_Income_ListingPLTE,
        Reminder_Fixed_Income_Suspend_Active,
        Reminder_Fixed_Income_Coupon_Update,
        Reminder_Fixed_Income_Nominal_Adjustment_PLTE,
        Reminder_Fixed_Income_Nominal_Adjustment_FITS
    }

    public static class AppReminderHelper {
        public static string GetReminderGroup(string typestr) {
            var result = "######";

            AppReminderTypes? type = null;

            try {
                type = (AppReminderTypes)Enum.Parse(typeof(AppReminderTypes), typestr);
            } catch (Exception) {
                
            }

            switch (type) {
                
                default:
                    break;
            }
            return result;
        }

        public static bool IsTypeExistInGroup(string group, string type) {

            if (string.IsNullOrEmpty(type)) return true;

            var groupDefinition = new Dictionary<string, string>();


                

            return groupDefinition.Where(t => t.Value == group).Any(t => type.Contains(t.Key));


        }

    }
}
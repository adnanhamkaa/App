using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.Utilities {
    public class AppActions {
        //masukan action sesuai region masing2, menghindari conflict
        //didepannya dikasih nama modulnya dulu supaya gampang searchnya

        public const string Samples_masterdata = nameof(Samples_masterdata);
        public const string Samples_ticket = nameof(Samples_ticket);

        public const string User_Management = nameof(User_Management);
        public const string User_Management_Roles = nameof(User_Management_Roles);
               
        public const string Activity_Log = nameof(Activity_Log);
               
        public const string Background_Job = nameof(Background_Job);
        public const string Error_Log = nameof(Error_Log);

        //#region User Management
        public const string usermanagement_registrasi = nameof(usermanagement_registrasi);
        //#endregion User Management

        public const string Restore_Delete = nameof(Restore_Delete);

    }
}
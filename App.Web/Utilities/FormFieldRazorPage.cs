using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace App.Web.Utilities {
    public abstract class FormFieldRazorPage<TModel, TFormFieldSetting> : WebViewPage<TModel>
        where TFormFieldSetting : FormFieldSetting {
        public TFormFieldSetting FormFieldSetting =>
            (TFormFieldSetting)ViewData[FormFieldGlobalViewModel.FORM_FIELD_SETTING_VDD_KEY];
    }

    public class FormFieldGlobalViewModel {
        public static readonly string FORM_FIELD_SETTING_VDD_KEY = $"{typeof(FormFieldSetting).FullName}:Instance";
        public FormFieldSetting FormFieldSetting { get; set; }  
        public dynamic Model { get; set; }
    }

    public static class FormFieldExtensions {
        public static MvcHtmlString FormFieldPartial<T>(this HtmlHelper<T> html, object formFieldModel,
            FormFieldSetting setting) {
            return html.Partial("~/Views/Shared/Metronic/_FormFields/_global.cshtml", new FormFieldGlobalViewModel {
                FormFieldSetting = setting,
                Model = formFieldModel
            });
        }
    }
}
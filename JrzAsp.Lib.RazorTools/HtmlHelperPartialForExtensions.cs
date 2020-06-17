using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace JrzAsp.Lib.RazorTools {
    public static class HtmlHelperPartialForExtensions {
        public static MvcHtmlString PartialFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            string partialViewName,
            Expression<Func<TModel, TProperty>> modelExpression,
            string customHtmlFieldPrefix = null,
            bool clearHtmlFieldPrefixBeforeUsingCustom = false,
            IDictionary<string, object> extraViewData = null
        ) {
            var vd = CloneAndDeriveViewDataDictionary(
                htmlHelper, modelExpression, customHtmlFieldPrefix,
                clearHtmlFieldPrefixBeforeUsingCustom, extraViewData);
            return htmlHelper.Partial(partialViewName, vd);
        }       

        public static MvcHtmlString PartialFor<TModel, TProperty>(
            this HtmlHelper htmlHelper,
            TModel model,
            string partialViewName,
            Expression<Func<TModel, TProperty>> modelExpression,
            string customHtmlFieldPrefix = null,
            bool clearHtmlFieldPrefixBeforeUsingCustom = false,
            IDictionary<string, object> extraViewData = null
        ) {
            var hh = htmlHelper.HtmlHelperFor(model);
            var vd = CloneAndDeriveViewDataDictionary(
                hh, modelExpression, customHtmlFieldPrefix,
                clearHtmlFieldPrefixBeforeUsingCustom, extraViewData);
            return hh.Partial(partialViewName, vd);
        }

        public static void RenderPartialFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            string partialViewName,
            Expression<Func<TModel, TProperty>> modelExpression,
            string customHtmlFieldPrefix = null,
            bool clearHtmlFieldPrefixBeforeUsingCustom = false,
            IDictionary<string, object> extraViewData = null
        ) {
            var vd = CloneAndDeriveViewDataDictionary(
                htmlHelper, modelExpression, customHtmlFieldPrefix,
                clearHtmlFieldPrefixBeforeUsingCustom, extraViewData);
            htmlHelper.RenderPartial(partialViewName, vd);
        }

        public static void RenderPartialFor<TModel, TProperty>(
            this HtmlHelper htmlHelper,
            TModel model,
            string partialViewName,
            Expression<Func<TModel, TProperty>> modelExpression,
            string customHtmlFieldPrefix = null,
            bool clearHtmlFieldPrefixBeforeUsingCustom = false,
            IDictionary<string, object> extraViewData = null
        ) {
            var hh = htmlHelper.HtmlHelperFor(model);
            var vd = CloneAndDeriveViewDataDictionary(
                hh, modelExpression, customHtmlFieldPrefix,
                clearHtmlFieldPrefixBeforeUsingCustom, extraViewData);
            hh.RenderPartial(partialViewName, vd);
        }

        public static ViewDataDictionary<TProperty> CloneAndDeriveViewDataDictionary<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> modelExpression,
            string customHtmlFieldPrefix = null,
            bool clearHtmlFieldPrefixBeforeUsingCustom = false,
            IDictionary<string, object> extraViewData = null
        ) {
            var prop = default(TProperty);
            if (typeof(TModel).IsValueType || htmlHelper.ViewData.Model != null) {
                prop = modelExpression.Compile()(htmlHelper.ViewData.Model);
            }

            var vd = new ViewDataDictionary<TProperty>(prop);
            foreach (var hvdkvp in htmlHelper.ViewData) vd.Add(hvdkvp);

            // chain model metadata
            vd.ModelMetadata = ModelMetadata.FromLambdaExpression(modelExpression, htmlHelper.ViewData);
            vd.Model = prop;

            // html field prefix
            var newVdHtmlFieldPrefix = htmlHelper.NameFor(modelExpression).ToString()
                .Substring(htmlHelper.NameFor(m => m).ToString().Length);
            vd.TemplateInfo.HtmlFieldPrefix = (customHtmlFieldPrefix ?? newVdHtmlFieldPrefix).Trim('.');

            if (!clearHtmlFieldPrefixBeforeUsingCustom) {
                var oldPrefix = htmlHelper.ViewData.TemplateInfo?.HtmlFieldPrefix?.Trim('.');
                if (vd.TemplateInfo.HtmlFieldPrefix.StartsWith("[")) {
                    vd.TemplateInfo.HtmlFieldPrefix = oldPrefix + vd.TemplateInfo.HtmlFieldPrefix;
                } else {
                    if (!string.IsNullOrEmpty(oldPrefix)) oldPrefix = oldPrefix + ".";
                    vd.TemplateInfo.HtmlFieldPrefix = oldPrefix + vd.TemplateInfo.HtmlFieldPrefix;
                }
            }

            if (!string.IsNullOrEmpty(vd.TemplateInfo.HtmlFieldPrefix)) {
                vd.TemplateInfo.HtmlFieldPrefix = vd.TemplateInfo.HtmlFieldPrefix.Trim('.');
            }

            // chain model state
            foreach (var mskvp in htmlHelper.ViewData.ModelState) vd.ModelState.Add(mskvp);

            if (extraViewData != null) foreach (var ekv in extraViewData) vd.Add(ekv.Key, ekv.Value);

            return vd;
        }
    }
}
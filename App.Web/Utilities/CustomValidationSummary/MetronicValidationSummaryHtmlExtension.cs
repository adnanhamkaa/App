using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using JrzAsp.Lib.RazorTools;

namespace App.Web.Utilities.CustomValidationSummary
{
    public static class MetronicValidationSummaryHtmlExtension {
        public static MvcHtmlString MetronicValidationSummary(this HtmlHelper html) {
            return html.PartialFor(html.ViewData.Model, "~/Views/Shared/Metronic/_ValidationSummary.cshtml", x => x);
        }
    }
    public static class CustomLabel {
        public static MvcHtmlString LabelForRequired<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string labelText = "") {
            return LabelHelper(html,
                ModelMetadata.FromLambdaExpression(expression, html.ViewData),
                ExpressionHelper.GetExpressionText(expression), labelText);
        }

        private static MvcHtmlString LabelHelper(HtmlHelper html,
            ModelMetadata metadata, string htmlFieldName, string labelText) {

            if (string.IsNullOrEmpty(labelText)) {
                labelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
            }
            if (string.IsNullOrEmpty(labelText)) {
                return MvcHtmlString.Empty;
            }
            bool isRequired = false;
            if (metadata.ContainerType != null) {
                isRequired = metadata.ContainerType.GetProperty(metadata.PropertyName)
                                .GetCustomAttributes(typeof(RequiredAttribute), false)
                                .Length == 1;
            }
            TagBuilder tag = new TagBuilder("label");
            tag.Attributes.Add(
                "for",
                TagBuilder.CreateSanitizedId(
                    html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName)
                )
            );

            tag.SetInnerText(labelText);

            if (isRequired)
            {
                var asteriskTag = new TagBuilder("b");
                asteriskTag.Attributes.Add("class", "text-danger");
                asteriskTag.SetInnerText("*");
                tag.InnerHtml += asteriskTag.ToString(TagRenderMode.Normal);
            }

            var output = tag.ToString(TagRenderMode.Normal);

            //if (isRequired) {
            //    var asteriskTag = new TagBuilder("b");
            //    asteriskTag.Attributes.Add("class", "text-danger");
            //    asteriskTag.SetInnerText("*");
            //    output += asteriskTag.ToString(TagRenderMode.Normal);
            //}
            return MvcHtmlString.Create(output);
        }
    }

}
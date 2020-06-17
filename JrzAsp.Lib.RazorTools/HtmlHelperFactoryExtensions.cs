using System.Web.Mvc;

namespace JrzAsp.Lib.RazorTools {
    /// <summary>
    ///     http://stackoverflow.com/a/9354081
    /// </summary>
    public static class HtmlHelperFactoryExtensions {
        public static HtmlHelper<TModel> HtmlHelperFor<TModel>(this HtmlHelper htmlHelper) {
            return HtmlHelperFor(htmlHelper, default(TModel));
        }

        public static HtmlHelper<TModel> HtmlHelperFor<TModel>(this HtmlHelper htmlHelper, TModel model) {
            return HtmlHelperFor(htmlHelper, model, null);
        }

        public static HtmlHelper<TModel> HtmlHelperFor<TModel>(this HtmlHelper htmlHelper, TModel model,
            string htmlFieldPrefix) {
            var viewDataContainer = CreateViewDataContainer(htmlHelper.ViewData, model);

            var templateInfo = viewDataContainer.ViewData.TemplateInfo;

            if (!string.IsNullOrEmpty(htmlFieldPrefix)) {
                templateInfo.HtmlFieldPrefix = templateInfo.GetFullHtmlFieldName(htmlFieldPrefix);
            }

            var viewContext = htmlHelper.ViewContext;
            var newViewContext = new ViewContext(viewContext.Controller.ControllerContext, viewContext.View,
                viewDataContainer.ViewData, viewContext.TempData, viewContext.Writer);

            return new HtmlHelper<TModel>(newViewContext, viewDataContainer, htmlHelper.RouteCollection);
        }

        private static IViewDataContainer CreateViewDataContainer(ViewDataDictionary viewData, object model) {
            var newViewData = new ViewDataDictionary(viewData) {
                Model = model
            };

            newViewData.TemplateInfo = new TemplateInfo {
                HtmlFieldPrefix = newViewData.TemplateInfo.HtmlFieldPrefix
            };

            return new ViewDataContainer {
                ViewData = newViewData
            };
        }

        private class ViewDataContainer : IViewDataContainer {
            public ViewDataDictionary ViewData { get; set; }
        }
    }
}
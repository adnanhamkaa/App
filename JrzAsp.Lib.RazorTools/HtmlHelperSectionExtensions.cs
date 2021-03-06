﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Mvc;

namespace JrzAsp.Lib.RazorTools {
    /// <summary>
    ///     This class is used to extend HtmlHelper so that it have extra methods that can
    ///     be used to render into layout section from any cshtml be it a view or even a partial.
    ///     This is needed because normal ASP MVC way of writing into a section cannot be done
    ///     when inside a partial.
    ///     Inspired by http://stackoverflow.com/a/18790222/426000
    /// </summary>
    public static class HtmlHelperSectionExtensions {

        /// <summary>
        ///     <para>
        ///         Start a delayed-execution block of output -- this will be rendered/printed on the next call to
        ///         <see cref="RenderSection" />.
        ///     </para>
        ///     <para>
        ///         <example>
        ///             Print once in "default block" (usually rendered at end via <code>@Html.RenderSection()</code>).  Code:
        ///             <code>
        /// @using (Html.Section()) {
        ///     <b>show at later</b>
        ///     <span>@Model.Name</span>
        ///     etc
        /// }
        /// </code>
        ///         </example>
        ///     </para>
        ///     <para>
        ///         <example>
        ///             Print once (i.e. if within a looped partial), using identified block via
        ///             <code>@Html.RenderSection("one-time")</code>.  Code:
        ///             <code>
        /// @using (Html.Section("one-time", isOnlyOne: "one-time")) {
        ///     <b>show me once</b>
        ///     <span>@Model.First().Value</span>
        /// }
        /// </code>
        ///         </example>
        ///     </para>
        /// </summary>
        /// <param name="helper">the helper from which we use the context</param>
        /// <param name="injectionBlockId">optional unique identifier to specify one or many injection blocks</param>
        /// <param name="isOnlyOne">
        ///     extra identifier used to ensure that this item is only added once; if provided, content should
        ///     only appear once in the page (i.e. only the first block called for this identifier is used)
        /// </param>
        /// <returns>using block to wrap delayed output</returns>
        public static IDisposable Section(this HtmlHelper helper, string injectionBlockId = null,
            string isOnlyOne = null) {
            return new DelayedSectionInjectionBlock(helper, injectionBlockId, isOnlyOne);
        }

        /// <summary>
        ///     Render all queued output blocks injected via <see cref="Section" />.
        ///     <para>
        ///         <example>
        ///             Print all delayed blocks using default identifier (i.e. not provided)
        ///             <code>
        /// @using (Html.Section()) {
        ///     <b>show me later</b>
        ///     <span>@Model.Name</span>
        ///     etc
        /// }
        /// </code>
        ///             -- then later --
        ///             <code>
        /// @using (Html.Section()) {
        ///     <b>more for later</b>
        ///     etc
        /// }
        /// </code>
        ///             -- then later --
        ///             <code>
        /// @Html.RenderSection() // will print both delayed blocks
        /// </code>
        ///         </example>
        ///     </para>
        ///     <para>
        ///         <example>
        ///             Allow multiple repetitions of rendered blocks, using same <code>@Html.Section()...</code> as before.  Code:
        ///             <code>
        /// @Html.RenderSection(removeAfterRendering: false); /* will print */
        /// @Html.RenderSection() /* will print again because not removed before */
        /// </code>
        ///         </example>
        ///     </para>
        /// </summary>
        /// <param name="helper">the helper from which we use the context</param>
        /// <param name="injectionBlockId">optional unique identifier to specify one or many injection blocks</param>
        /// <param name="removeAfterRendering">only render this once</param>
        /// <returns>rendered output content</returns>
        public static MvcHtmlString RenderSection(this HtmlHelper helper, string injectionBlockId = null,
            bool removeAfterRendering = true) {
            var stack = DelayedSectionInjectionBlock.GetQueue(helper, injectionBlockId);

            if (removeAfterRendering) {
                var sb = new StringBuilder(
#if DEBUG
                    $"<!-- delayed-block: {injectionBlockId} -->"
#endif
                );
                // .count faster than .any
                while (stack.Count > 0) sb.AppendLine(stack.Dequeue());
                return MvcHtmlString.Create(sb.ToString());
            }

            return MvcHtmlString.Create(
#if DEBUG
                $"<!-- delayed-block: {injectionBlockId} -->" +
#endif
                string.Join(Environment.NewLine, stack));
        }

        /// <summary>
        ///     Delegate script/resource/etc injection until the end of the page
        ///     <para>
        ///         @via http://stackoverflow.com/a/14127332/1037948 and
        ///         http://jadnb.wordpress.com/2011/02/16/rendering-scripts-from-partial-views-at-the-end-in-mvc/
        ///     </para>
        /// </summary>
        private class DelayedSectionInjectionBlock : IDisposable {
            /// <summary>
            ///     Unique internal storage key
            /// </summary>
            private const string CACHE_KEY = "DCCF8C78-2E36-4567-B0CF-FE052ACCE309"; // "DelayedInjectionBlocks";

            /// <summary>
            ///     Internal storage identifier for remembering unique/isOnlyOne items
            /// </summary>
            private const string UNIQUE_IDENTIFIER_KEY = CACHE_KEY;

            /// <summary>
            ///     What to use as internal storage identifier if no identifier provided (since we can't use null as key)
            /// </summary>
            private const string EMPTY_IDENTIFIER = "";

            private readonly HtmlHelper _helper;
            private readonly string _identifier;
            private readonly string _isOnlyOne;

            /// <summary>
            ///     Create a new using block from the given helper (used for trapping appropriate context)
            /// </summary>
            /// <param name="helper">the helper from which we use the context</param>
            /// <param name="identifier">optional unique identifier to specify one or many injection blocks</param>
            /// <param name="isOnlyOne">
            ///     extra identifier used to ensure that this item is only added once; if provided, content should
            ///     only appear once in the page (i.e. only the first block called for this identifier is used)
            /// </param>
            public DelayedSectionInjectionBlock(HtmlHelper helper, string identifier = null, string isOnlyOne = null) {
                _helper = helper;

                // start a new writing context
                ((WebViewPage) _helper.ViewDataContainer).OutputStack.Push(new StringWriter());

                _identifier = identifier ?? EMPTY_IDENTIFIER;
                _isOnlyOne = isOnlyOne;
            }

            /// <summary>
            ///     Append the internal content to the context's cached list of output delegates
            /// </summary>
            public void Dispose() {
                // render the internal content of the injection block helper
                // make sure to pop from the stack rather than just render from the Writer
                // so it will remove it from regular rendering
                var content = ((WebViewPage) _helper.ViewDataContainer).OutputStack;
                var renderedContent = content.Count == 0 ? string.Empty : content.Pop().ToString();

                // if we only want one, remove the existing
                var queue = GetQueue(_helper, _identifier);

                // get the index of the existing item from the alternate storage
                var existingIdentifiers = _GetOrSet(_helper, new Dictionary<string, int>(), UNIQUE_IDENTIFIER_KEY);

                // only save the result if this isn't meant to be unique, or
                // if it's supposed to be unique and we haven't encountered this identifier before
                if (null == _isOnlyOne || !existingIdentifiers.ContainsKey(_isOnlyOne)) {
                    // remove the new writing context we created for this block
                    // and save the output to the queue for later
                    queue.Enqueue(renderedContent);

                    // only remember this if supposed to
                    if (null != _isOnlyOne) {
                        existingIdentifiers[_isOnlyOne] =
                            queue.Count; // save the index, so we could remove it directly (if we want to use the last instance of the block rather than the first)
                    }
                }
            }

            /// <summary>
            ///     Retrieve a context-aware list of cached output delegates from the given helper; uses the helper's context rather
            ///     than singleton HttpContext.Current.Items
            /// </summary>
            /// <param name="helper">the helper from which we use the context</param>
            /// <param name="identifier">optional unique sub-identifier for a given injection block</param>
            /// <returns>list of delayed-execution callbacks to render internal content</returns>
            public static Queue<string> GetQueue(HtmlHelper helper, string identifier = null) {
                return _GetOrSet(helper, new Queue<string>(), identifier ?? EMPTY_IDENTIFIER);
            }

            /// <summary>
            ///     Retrieve a context-aware list of cached output delegates from the given helper; uses the helper's context rather
            ///     than singleton HttpContext.Current.Items
            /// </summary>
            /// <param name="helper">the helper from which we use the context</param>
            /// <param name="defaultValue">
            ///     the default value to return if the cached item isn't found or isn't the expected type; can
            ///     also be used to set with an arbitrary value
            /// </param>
            /// <param name="identifier">optional unique sub-identifier for a given injection block</param>
            /// <returns>list of delayed-execution callbacks to render internal content</returns>
            private static T _GetOrSet<T>(HtmlHelper helper, T defaultValue, string identifier = EMPTY_IDENTIFIER)
                where T : class {
                var storage = GetStorage(helper);

                // return the stored item, or set it if it does not exist
                return (T) (storage.ContainsKey(identifier)
                    ? storage[identifier]
                    : (storage[identifier] = defaultValue));
            }

            /// <summary>
            ///     Get the storage, but if it doesn't exist or isn't the expected type, then create a new "bucket"
            /// </summary>
            /// <param name="helper"></param>
            /// <returns></returns>
            public static Dictionary<string, object> GetStorage(HtmlHelper helper) {
                var storage = helper.ViewContext.HttpContext.Items[CACHE_KEY] as Dictionary<string, object>;
                if (storage == null) {
                    helper.ViewContext.HttpContext.Items[CACHE_KEY] = storage = new Dictionary<string, object>();
                }
                return storage;
            }
        }
    }
}
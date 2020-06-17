using App.DataAccess;
using App.Web.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace App.Web.Utilities
{
    public class CommonHelper {
        
        public static dynamic Cast(dynamic source, Type dest) {

            if (source == null) return source;

            if (dest == typeof(decimal?)) {
                return Convert.ToDecimal(source);
            }

            if (dest == typeof(int?)) {
                return Convert.ToInt32(source);
            }

            if (dest == typeof(DateTime?)) {
                return Convert.ToDateTime(source);
            }

            return Convert.ChangeType(source, dest);
        }

        public static string GetVersion() {
            return "v1.0.0";
        }

        public static string GetExcelColumnName(int columnNumber) {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0) {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }

        public static IEnumerable<T> GetEnumValues<T>() {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static string GetBaseUrl() {
            var request = HttpContext.Current.Request;
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;

            if (appUrl != "/")
                appUrl = "/" + appUrl;

            var baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);

            return baseUrl;
        }

       

    }

    public static class Randomer
    {

        static Randomer() {
            GlobalSelf = new Random();
        }

        public static Random Self => PerRequestSelf ?? GlobalSelf;
        public static Random GlobalSelf { get; }
        public static Random PerRequestSelf {
            get {
                var ctx = HttpContext.Current;
                if (ctx == null) return null;
                var key = "App.Utilities.Randomer";
                //var rdm = new Random();
                if (ctx.Items[key] is Random rdm) return rdm;
                rdm = new Random();
                ctx.Items[key] = rdm;
                return rdm;

            }
        }
    }

    public enum Separator {
        Koma,
        Titik
    }

    public static class CommonExtention {
        /// <summary>
        ///     Returns "(T) obj"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T DirectCastTo<T>(this object obj) {
            return (T)obj;
        }

        /// <summary>
        ///     Returns "obj as T"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T ChangeTypeTo<T>(this object obj) where T : class {
            return obj as T;
        }

        public static object GetPropValue(this object src, string propName) {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

        public static void SetPropValue(this object src, string propName, object value) {
            src.GetType().GetProperty(propName).SetValue(src, value, null);
        }

        public static void CopyStream(this Stream source, Stream destination) {
            byte[] buffer = new byte[32768];
            int bytesRead;
            do {
                bytesRead = source.Read(buffer, 0, buffer.Length);
                destination.Write(buffer, 0, bytesRead);
            } while (bytesRead != 0);
        }

        /// <summary>
        /// Flattens an object hierarchy.
        /// </summary>
        /// <param name="rootLevel">The root level in the hierarchy.</param>
        /// <param name="nextLevel">A function that returns the next level below a given item.</param>
        /// <returns><![CDATA[An IEnumerable<T> containing every item from every level in the hierarchy.]]></returns>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> rootLevel, Func<T, IEnumerable<T>> nextLevel) {
            List<T> accumulation = new List<T>();
            accumulation.AddRange(rootLevel);
            flattenLevel<T>(accumulation, rootLevel, nextLevel);
            return accumulation;
        }

        /// <summary>
        /// Recursive helper method that traverses a hierarchy, accumulating items along the way.
        /// </summary>
        /// <param name="accumulation">A collection in which to accumulate items.</param>
        /// <param name="currentLevel">The current level we are traversing.</param>
        /// <param name="nextLevel">A function that returns the next level below a given item.</param>
        private static void flattenLevel<T>(List<T> accumulation, IEnumerable<T> currentLevel, Func<T, IEnumerable<T>> nextLevel) {
            foreach (T item in currentLevel) {
                accumulation.AddRange(currentLevel);
                flattenLevel<T>(accumulation, nextLevel(item), nextLevel);
            }
        }

        public static decimal? SafeRound(this decimal val, int decimalplace) {
            try {
                return Math.Round(val, decimalplace);
            } catch (Exception exc) {
                return null;
            }

        }

        public static DateTime ChangeTime(this DateTime obj, int? hours = null, int? minutes = null, int? seconds = null, int? milliseconds = null) {

            return new DateTime(
                obj.Year,
                obj.Month,
                obj.Day,
                hours ?? obj.Hour,
                minutes ?? obj.Minute,
                seconds ?? obj.Second,
                milliseconds ?? obj.Millisecond,
                obj.Kind);
        }

        public static T SafeGetElementAt<T>(this T[] array, int index) {

            if (index < array.Length) {
                return array[index];
            }

            return default(T);

        }

        public static T SafeConvert<T>(this string s, T defaultValue) {
            if (string.IsNullOrEmpty(s))
                return defaultValue;
            try {
                return (T)Convert.ChangeType(s, typeof(T));
            } catch (Exception exc) {
                return defaultValue;
            }

        }

        public static decimal? NormalizeDecimalPlaces(this decimal? value) {
            return value.HasValue ? value % 1 == 0 ? Math.Round(value.Value, 0) : value : null;
        }

        public static decimal NormalizeDecimalPlaces(this decimal value) {
            return value % 1 == 0 ? Math.Round(value, 0) : value;
        }

        public static T SafeConvert<T>(this string s) {
            try {
                return (T)Convert.ChangeType(s, typeof(T));
            } catch (Exception exc) {
                return (T)typeof(T).GetDefaultValue();
            }

        }

        public static void AddDuplicateErrorState(this ModelStateDictionary modelState) {
            modelState.AddModelError("", "Duplicate Data");
        }

        public static string ToKMB(this decimal number) {

            SortedDictionary<decimal, string> abbrevations = new SortedDictionary<decimal, string>
             {
                {1000m,"K"},
                {1000000m, "Jt" },
                {1000000000m, "M" },
                {1000000000000m, "T" }
             };

            for (int i = abbrevations.Count - 1; i >= 0; i--) {
                KeyValuePair<decimal, string> pair = abbrevations.ElementAt(i);
                if (Math.Abs(number) >= pair.Key) {
                    var roundedNumber = Math.Round(number / pair.Key, 1);
                    return roundedNumber.ToFormatedString() + pair.Value;
                }
            }
            return number.ToFormatedString();
        }

        public static void AddDuplicateErrorState(this ModelStateDictionary modelState, string message) {
            modelState.AddModelError("", message);
        }

        public static object GetDefaultValue(this Type t) {
            if (t.IsValueType) {
                return Activator.CreateInstance(t);
            } else {
                return null;
            }
        }

        public static string ToPascalCase(this string s, char separator = ' ') {
            var words = s.Split(separator)
                         .Select(word => word.Substring(0, 1).ToUpper() +
                                         word.Substring(1).ToLower()).ToArray();

            var result = string.Join(separator.ToString(), words);
            return result;
        }

        public static DateTime? SafeDateConvert(this string s, string format = null) {
            if (string.IsNullOrEmpty(s))
                return null;

            try {
                if (format == null) return Convert.ToDateTime(s);

                return DateTime.ParseExact(s, format, null);

            } catch (Exception exc) {

                return null;
            }
        }

        public static DateTime? SafeOADateConvert(this string s) {
            if (string.IsNullOrEmpty(s))
                return null;

            try {

                return DateTime.FromOADate(long.Parse(s));

            } catch (Exception exc) {

                return null;
            }
        }

        public static decimal? SafeDecimalConvert(this string s) {
            if (string.IsNullOrEmpty(s))
                return null;

            try {

                return Convert.ToDecimal(s);

            } catch (Exception e) {

                return null;
            }
        }

        public static IQueryable<T> BindDbFunctions<T>(this IQueryable<T> source) {
            var expression = new DbFunctionsBinder().Visit(source.Expression);
            if (expression == source.Expression) return source;
            return source.Provider.CreateQuery<T>(expression);
        }

        class DbFunctionsBinder : ExpressionVisitor {
            protected override Expression VisitMethodCall(MethodCallExpression node) {
                if (node.Object != null && node.Object.Type == typeof(DateTime)) {
                    if (node.Method.Name == "AddHours") {
                        var timeValue = Visit(node.Object);
                        var addValue = Visit(node.Arguments.Single());
                        if (timeValue.Type != typeof(DateTime?)) timeValue = Expression.Convert(timeValue, typeof(DateTime?));
                        if (addValue.Type != typeof(int?)) addValue = Expression.Convert(addValue, typeof(int?));
                        var methodCall = Expression.Call(
                            typeof(DbFunctions), "AddHours", Type.EmptyTypes,
                            timeValue, addValue);
                        return Expression.Convert(methodCall, typeof(DateTime));
                    }
                }
                return base.VisitMethodCall(node);
            }

            protected override Expression VisitMember(MemberExpression node) {
                if (node.Expression != null && node.Expression.Type == typeof(DateTime) && node.Member.Name == "Date") {
                    var dateValue = Expression.Convert(Visit(node.Expression), typeof(DateTime?));
                    var methodCall = Expression.Call(typeof(DbFunctions), "TruncateTime", Type.EmptyTypes, dateValue);
                    return Expression.Convert(methodCall, typeof(DateTime));
                }
                return base.VisitMember(node);
            }
        }

        static bool IsNullable<T>(this T obj) {
            if (obj == null) return true; // obvious
            Type type = typeof(T);
            if (!type.IsValueType) return true; // ref-type
            if (Nullable.GetUnderlyingType(type) != null) return true; // Nullable<T>
            return false; // value-type
        }

        public static PropertyInfo GetPropertyFromPath(this Type type, string path) {
            Type currentType = type;
            PropertyInfo currentProp = null;
            foreach (string propertyName in path.Split('.')) {
                currentProp = currentType.GetProperty(propertyName);
                if (currentProp == null) return null;
                currentType = currentProp.PropertyType;
            }
            return currentProp;
        }

        public static bool IsBasicType(this Type type) {
            return (type.IsPrimitive ||
                type == typeof(String) ||
                type == typeof(decimal) ||
                type == typeof(decimal?) ||
                type == typeof(int) ||
                type == typeof(int?) ||
                type == typeof(DateTime) ||
                type == typeof(DateTime?));

        }

        public static string ToFormatedString(this decimal number, bool addRupiah = false, Separator separator = Separator.Titik, int? decimalPoint = null, string zeroReplacement = null) {
            var result = "";

            if (number == 0 && !string.IsNullOrEmpty(zeroReplacement)) return zeroReplacement;

            number = number / 1.000000000000000000000000000000000m;

            if (number % 1 != 0) {
                switch (separator) {
                    case Separator.Koma:
                        result = number.ToString("N");
                        break;
                    case Separator.Titik:
                        result = number.ToString("N").Replace('.', '#').Replace(',', '.').Replace('#', ',');
                        break;
                    default:
                        result = number.ToString("N").Replace('.', '#').Replace(',', '.').Replace('#', ',');
                        break;
                }
            }

            int count = BitConverter.GetBytes(decimal.GetBits(number)[3])[2];

            count = count > 3 && decimalPoint == null ? 3 : (decimalPoint ?? count);

            switch (separator) {
                case Separator.Koma:
                    result = number.ToString("N" + count);
                    break;
                case Separator.Titik:
                    result = number.ToString("N" + count).Replace('.', '#').Replace(',', '.').Replace('#', ',');
                    break;
                default:
                    result = number.ToString("N" + count).Replace('.', '#').Replace(',', '.').Replace('#', ',');
                    break;
            }

            if (!string.IsNullOrEmpty(result) && addRupiah) {
                result = "Rp" + result + ",-";
            }

            return result;

        }

        public static string ToFormatedString(this int number) {
            return number.ToString("N0").Replace(',', '.');
        }

        public static decimal? Round(this decimal? number) {
            return Math.Round(Convert.ToDecimal(number), 3);
        }

        public static decimal? Round(this double number) {
            return Math.Round(Convert.ToDecimal(number), 3);
        }

        public static decimal Round(this decimal number, int decimalplace = 3) {
            return Math.Round(Convert.ToDecimal(number), decimalplace);
        }

        public static DateTime[] GetDayRange(this DateTime date) {
            var result = new List<DateTime>();
            result.Add(date.Date);
            result.Add(date.Date.AddDays(1));

            return result.ToArray();
        }

        public static DateTime AddWorkDays(this DateTime originalDate, int workDays) {

            if (workDays == 0) return originalDate;

            DateTime tmpDate = originalDate;

            while (tmpDate.IsHoliday()) {
                if (workDays > 0) {
                    tmpDate = tmpDate.AddDays(-1);
                } else {
                    tmpDate = tmpDate.AddDays(1);
                }
            }

            for (int i = 1; i <= Math.Abs(workDays); i++) {

                while (tmpDate.IsHoliday()) {
                    if (workDays > 0) {
                        tmpDate = tmpDate.AddDays(1);
                    } else {
                        tmpDate = tmpDate.AddDays(-1);
                    }
                }

                if (workDays > 0) {
                    tmpDate = tmpDate.AddDays(1);
                } else {
                    tmpDate = tmpDate.AddDays(-1);
                }


            }

            while (tmpDate.IsHoliday()) {
                if (workDays > 0) {
                    tmpDate = tmpDate.AddDays(1);
                } else {
                    tmpDate = tmpDate.AddDays(-1);
                }
            }

            return tmpDate;
        }


        public static IQueryable<T> IncludeAll<T>(this IQueryable<T> queryable) where T : class {
            var type = typeof(T);
            var properties = type.GetProperties();
            foreach (var property in properties) {
                var isChild = property.PropertyType.BaseType == typeof(ModelBase) || typeof(IEnumerable<ModelBase>).IsAssignableFrom(property.PropertyType);
                if (isChild) {
                    queryable = queryable.Include(property.Name);
                }
            }
            return queryable;
        }

        public static bool IsHoliday(this DateTime originalDate, string module = null) {
            originalDate = originalDate.StartOfDay();
            var result = HttpContext.Current?.Cache["workday"] as IEnumerable<Holiday>;

            if (originalDate.DayOfWeek == DayOfWeek.Sunday || originalDate.DayOfWeek == DayOfWeek.Saturday) return true;

            if (result == null || result.Count() == 0) {
                using (ApplicationDbContext context = new ApplicationDbContext()) {

                    //insert your get holidays from db here

                    IEnumerable<Holiday> holidays = new List<Holiday>();

                    if (HttpContext.Current != null)
                        HttpContext.Current.Cache["workday"] = holidays;

                    result = holidays;
                }
            }

            return result.Any(t => t.HolidayDate.HasValue && t.HolidayDate.Value.Date == originalDate);

        }

        public static DateTime GetStartOfMonthWorkingDay(this DateTime originalDate) {
            DateTime tmpDate = new DateTime(originalDate.Year, originalDate.Month, 1);
            tmpDate = tmpDate.AddDays(-1).AddWorkDays(1);

            return tmpDate;
        }

        public static DateTime GetEndOfMonthWorkingDay(this DateTime originalDate) {
            var lastdate = DateTime.DaysInMonth(originalDate.Year, originalDate.Month);
            DateTime tmpDate = new DateTime(originalDate.Year, originalDate.Month, lastdate, 23, 59, 59);
            tmpDate = tmpDate.AddDays(1).AddWorkDays(-1);

            return tmpDate;
        }

        public static DateTime GetEndOfMonth(this DateTime originalDate) {

            var lastdate = DateTime.DaysInMonth(originalDate.Year, originalDate.Month);
            DateTime tmpDate = new DateTime(originalDate.Year, originalDate.Month, lastdate, 23, 59, 59);

            return tmpDate;

        }

        public static DateTime GetStartOfMonth(this DateTime originalDate) {
            DateTime tmpDate = new DateTime(originalDate.Year, originalDate.Month, 1);

            return tmpDate;

        }

        

        public static DateTime EndOfDay(this DateTime date) {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999);
        }

        public static DateTime StartOfDay(this DateTime date) {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
        }

        /// <summary>
        /// Method extend untuk konversi tipe datetime ke string dengan bahasa indonesia
        /// </summary>
        /// <param name="format">format string contoh DD/MMM/YYYY </param>
        /// <returns>string datetime sesuai dengan format yang diinputkan</returns>
        public static string ToStringIndonesia(this DateTime date, string format) {



            format = format.Replace("yyyy", date.Year.ToString());
            format = (date.Year.ToString().Length >= 4) ? format.Replace("yyy", date.Year.ToString().Substring(1, 3)) : format.Replace("yyyy", date.Year.ToString());
            format = (date.Year.ToString().Length >= 4) ? format.Replace("yy", date.Year.ToString().Substring(2, 2)) : format.Replace("yyyy", date.Year.ToString());

            format = format.Replace("YYYY", date.Year.ToString());
            format = (date.Year.ToString().Length >= 4) ? format.Replace("YYY", date.Year.ToString().Substring(1, 3)) : format.Replace("yyyy", date.Year.ToString());
            format = (date.Year.ToString().Length >= 4) ? format.Replace("YY", date.Year.ToString().Substring(2, 2)) : format.Replace("yyyy", date.Year.ToString());

            format = format.Replace("tt", date.ToString("tt"));
            format = format.Replace("t", date.ToString("t"));

            format = format.Replace("mm", date.ToString("mm"));
            format = format.Replace("ss", date.ToString("ss"));

            format = format.Replace("HH", date.ToString("HH"));
            format = format.Replace("hh", date.ToString("hh"));

            format = format.Replace("MMMM", Bulan(date.Month));
            format = format.Replace("MMM", Bulan(date.Month).Substring(0, 3));
            format = format.Replace("MM", date.Month.ToString());
            format = format.Replace("dddd", Hari(date.ToString("dddd")));
            format = format.Replace("ddd", Hari(date.ToString("dddd")).Substring(0, 3));
            format = format.Replace("dd", date.ToString("dd"));
            format = format.Replace("d", date.ToString("%d"));

            return format;
        }

        private static string Hari(string hari) {
            string terbilang = "";
            hari = hari.ToLower();

            switch (hari) {
                case "monday":
                    terbilang = "Senin";
                    break;
                case "tuesday":
                    terbilang = "Selasa";
                    break;
                case "wednesday":
                    terbilang = "Rabu";
                    break;
                case "thursday":
                    terbilang = "Kamis";
                    break;
                case "friday":
                    terbilang = "Jumat";
                    break;
                case "saturday":
                    terbilang = "Sabtu";
                    break;
                case "sunday":
                    terbilang = "Minggu";
                    break;
                default:
                    break;
            }

            return terbilang;
        }

        private static string Bulan(int bulan) {
            string terbilang = "";

            switch (bulan) {
                case 1:
                    terbilang = "Januari";
                    break;
                case 2:
                    terbilang = "Februari";
                    break;
                case 3:
                    terbilang = "Maret";
                    break;
                case 4:
                    terbilang = "April";
                    break;
                case 5:
                    terbilang = "Mei";
                    break;
                case 6:
                    terbilang = "Juni";
                    break;
                case 7:
                    terbilang = "Juli";
                    break;
                case 8:
                    terbilang = "Agustus";
                    break;
                case 9:
                    terbilang = "September";
                    break;
                case 10:
                    terbilang = "Oktober";
                    break;
                case 11:
                    terbilang = "November";
                    break;
                case 12:
                    terbilang = "Desember";
                    break;
                default:
                    break;
            }

            return terbilang;

        }

        

        public static IQueryable<T> IncludeAll<T>(this IQueryable<T> queryable, params string[] includes) where T : class {

            foreach (var include in includes) {
                queryable = queryable.Include(include);
            }
            return queryable;
        }

        public static string GetDoublePrecision(this double? source, bool useIdCulture = false) {
            if (source == null) {
                return null;
            }

            if (source == 0) {
                return "0";
            }

            var removal = Math.Pow(10, 10);
            double current = (double)source * removal;
            current = Math.Floor(current);
            int counter = 0;
            while (current % 10 == 0) {
                current /= 10;
                counter++;
            }

            string format = "#,###";
            int diff = 10 - counter;
            if (diff == 0) {
                return source?.ToString(format);
            }
            format += ".";
            for (int i = 0; i < (10 - counter); i++) {
                format += "0";
            }

            if (useIdCulture) {
                return source?.ToString(format, CultureInfo.CreateSpecificCulture("id-ID"));
            }

            return source?.ToString(format);
        }

        public static string GetDecimalPrecision(this decimal? source, bool useIdCulture = false)
        {
            if (source == null)
            {
                return null;
            }

            var result = ((decimal)source).ToString("###,###.000");

            var separated = result.Split('.');

            if (separated.Count() > 1)
            {
                var tmp = Int16.Parse(separated[1]);
                if (tmp == 0) {
                    result = separated[0];
                }
                else
                {
                    while(tmp % 10 == 0)
                    {
                        tmp /= 10;
                    }

                    result = $"{separated[0]}.{tmp.ToString()}";
                }
            }

            return result;
        }
        public static string StripHtmlTags(this string text) {
            if (string.IsNullOrWhiteSpace(text)) return text;
            var doc = new HtmlDocument();
            doc.LoadHtml(text);
            return doc.DocumentNode.InnerText;
        }
    }
}
namespace carenirvana.bre.engine.rulefunction
{
    public partial class RuleFunctionInternal
    {
        // Static global variables
        public static DateTime CURRDATE = DateTime.Now.Date;
        public static DateTime NOW = DateTime.Now;

        // Text functions
        public static string CONCAT(params string[] strings)
        {
            return string.Join(" ", strings.Select(TRIM).Where(s => !string.IsNullOrEmpty(s)));
        }

        public static string LOWERCASE(string str)
        {
            return str.ToLower();
        }

        public static string UPPERCASE(string str)
        {
            return str.ToUpper();
        }

        public static string TRIM(string str)
        {
            return str.Trim();
        }

        public static string TRIMSTART(string str)
        {
            return str.TrimStart();
        }

        public static string TRIMEND(string str)
        {
            return str.TrimEnd();
        }

        public static bool ISNULL(object obj)
        {
            if (obj == null)
            {
                return true;
            }

            if (obj is DateTime dateTime)
            {
                return dateTime == default;
            }

            return false;
        }

        public static bool ISNOTNULL(object obj)
        {
            return obj != null;
        }

        // Date functions
        public static int DATEDIFF(DateTime date1, DateTime date2, string difftype)
        {
            var totalDays = (int)date2.Subtract(date1).TotalDays;
            return difftype switch
            {
                "D" => totalDays,
                "M" => (int)(totalDays / (365.2425 / 12)),
                "Y" => (int)(totalDays / 365.2425),
                _ => totalDays
            };
        }

        public static int DAY(DateTime date)
        {
            return date.Day;
        }

        public static int MONTH(DateTime date)
        {
            return date.Month;
        }
        public static int YEAR(DateTime date)
        {
            return date.Year;
        }
    }
}
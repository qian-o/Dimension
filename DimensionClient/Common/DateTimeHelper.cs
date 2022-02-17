namespace DimensionClient.Common
{
    public static class DateTimeHelper
    {
        public static int GetAge(this DateTime dateTime)
        {
            DateTime now = DateTime.Now;
            int age = now.Year - dateTime.Year;
            if (now.Month < dateTime.Month || (now.Month == dateTime.Month && now.Day < dateTime.Day))
            {
                age--;
            }
            return age > 0 ? age : 0;
        }

        public static string GetConstellation(this DateTime dateTime)
        {
            string data;
            switch (dateTime.Month)
            {
                case 12 when dateTime.Day >= 21:
                case 1 when dateTime.Day <= 19:
                    data = "摩羯座";
                    break;
                case 1 when dateTime.Day >= 19:
                case 2 when dateTime.Day <= 18:
                    data = "水瓶座";
                    break;
                case 2 when dateTime.Day >= 18:
                case 3 when dateTime.Day <= 20:
                    data = "双鱼座";
                    break;
                case 3 when dateTime.Day >= 21:
                case 4 when dateTime.Day <= 19:
                    data = "白羊座";
                    break;
                case 4 when dateTime.Day >= 20:
                case 5 when dateTime.Day <= 20:
                    data = "金牛座";
                    break;
                case 5 when dateTime.Day >= 21:
                case 6 when dateTime.Day <= 21:
                    data = "双子座";
                    break;
                case 6 when dateTime.Day >= 22:
                case 7 when dateTime.Day <= 22:
                    data = "巨蟹座";
                    break;
                case 7 when dateTime.Day >= 23:
                case 8 when dateTime.Day <= 22:
                    data = "狮子座";
                    break;
                case 8 when dateTime.Day >= 23:
                case 9 when dateTime.Day <= 22:
                    data = "处女座";
                    break;
                case 9 when dateTime.Day >= 23:
                case 10 when dateTime.Day <= 23:
                    data = "天秤座";
                    break;
                case 10 when dateTime.Day >= 24:
                case 11 when dateTime.Day <= 22:
                    data = "天蝎座";
                    break;
                default:
                    data = "射手座";
                    break;
            }
            return data;
        }
    }
}

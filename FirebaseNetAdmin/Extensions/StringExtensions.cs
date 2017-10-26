namespace FirebaseNetAdmin.Extensions
{
    public static class StringExtensions
    {
        public static string TrimSlashes(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return "";
            }
            char[] trimedCharacters = { '/', '\\' };
            return str.Trim(trimedCharacters);
        }
    }
}

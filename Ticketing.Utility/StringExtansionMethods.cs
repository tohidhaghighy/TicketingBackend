namespace Ticketing.Utility
{
    public static class StringExtansionMethods
    {
        public static List<int> ConvertStringToListIntiger (this string str)
        {
            List<int> output = new List<int>();
            var input = str.Split(",");
            foreach (var item in input)
            { 
                output.Add(int.Parse(item));
            }
            return output;
        }
    }
}

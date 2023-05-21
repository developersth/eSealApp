using System.Globalization;
using System.Data;
using Microsoft.Data.SqlClient;

namespace backend
{
    public class Utilities
    {
        public static string GennerateId(string prefix,int id)
        {
            string currentDate = DateTime.Now.ToString("yyyyMMdd",new CultureInfo("en-us")); // Get the current date in the format "yyyyMMdd"

            string paddedNumericPortion = id.ToString().PadLeft(5, '0'); // Pad the numeric portion with leading zeros

            string newId = $"{prefix}{currentDate}-{paddedNumericPortion}"; // Combine the components to form the new ID

            return newId;
        }
    }
}
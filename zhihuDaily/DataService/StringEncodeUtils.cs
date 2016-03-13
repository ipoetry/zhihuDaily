using System;
using System.Text;

namespace zhihuDaily.DataService
{
    public class StringEncodeUtils
    {
        static public string EncodeTo64(string toEncode)
        {

            byte[] toEncodeAsBytes = Encoding.ASCII.GetBytes(toEncode);

            string returnValue= Convert.ToBase64String(toEncodeAsBytes);

            return returnValue;

        }
        static public string DecodeFrom64(string encodedData)
        {

            byte[] encodedDataAsBytes = Convert.FromBase64String(encodedData);

            string returnValue = Encoding.ASCII.GetString(encodedDataAsBytes);

            return returnValue;

        }
    }
}

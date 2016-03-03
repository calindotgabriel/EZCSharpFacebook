// Created by Călin Gabriel
// at 20:53 on 27/02/2016.
//  

using System;

namespace EZCSharpFacebook
{
    public static class EZFacebookExtensions
    {
        /**
         * C# equivalent method to Java's String.substring() method
         * Returns the substring for a given start point to an endpoint.
         */
        public static string JavaSubString(this string s, int start, int end)
        {
            return s.Substring(start, end - start + 1);
        }

        /**
         * Extract a given parameter from a string that is usually a query.
         * Query format : ..param1=value1&param2=valu2...
         */
        public static string ExtractFacebookParameter(this string s, string parameter)
        {
            var parameterIndex = s.IndexOf(parameter, StringComparison.Ordinal);
            var parameterEqualsValue = s.Substring(parameterIndex);

            var start = parameterEqualsValue.IndexOf("=", StringComparison.Ordinal) + 1;
            var end = parameterEqualsValue.IndexOf("&", StringComparison.Ordinal) - 1;

            bool parameterIsLastInList = end == -1 - 1;
            return parameterIsLastInList ? parameterEqualsValue.Substring(start) : parameterEqualsValue.JavaSubString(start, end);
        }
    }
}
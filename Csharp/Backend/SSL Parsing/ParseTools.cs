using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace SSLParser
{
    public class ParsingTools
    {
        public static readonly string TAB = "    ";

        /// <summary>
        /// Parses functions()
        /// </summary>
        /// <param name="line">the line to parse</param>
        /// <param name="method">the name of the method</param>
        /// <returns>all arguments inside the parenthases of the method</returns>
        public static string[] GetInParenthases(string line, out string method)
        {
            int index = line.IndexOf('(');
            method = line.Substring(0, index);

            int starts = 1;
            int ends = 0;
            int lastCut = index + 1;
            bool stringWrap = false;
            string[] output = new string[0];

            for (int i = index + 1; i < line.Length; i++)
            {
                // check if current character is the start or end of a string

                if (line[i] == '"')
                {
                    stringWrap = !stringWrap;
                }

                // skip string

                else if (!stringWrap)
                {
                    if (line[i] == '(' || line[i] == '{')
                    {
                        starts++;
                    }
                    else if (line[i] == ')' || line[i] == '}')
                    {
                        ends++;

                        // end

                        if (starts == ends)
                        {
                            Tools.AddToArray(ref output, line.Substring(lastCut, i - lastCut).Trim());
                            break;
                        }
                    }
                    else if ((line[i] == ',') && ((starts - ends) < 2))
                    {
                        // finish argument

                        Tools.AddToArray(ref output, line.Substring(lastCut, i - lastCut).Trim());
                        lastCut = i + 1;
                    }
                }
            }

            return output;
        }

        /// <summary>
        /// Returns a formatted line of code
        /// </summary>
        /// <param name="text">line of code</param>
        /// <returns>formatted code</returns>
        public static string FormatCode(string text)
        {
            int s = text.IndexOf("//");
            if (s == -1)
                return text.Trim();
            return text.Substring(0, s).Trim();
        }

        /// <summary>
        /// Sets a variable of a string name, to a value from a string.
        /// </summary>
        /// <param name="variable">variable to be set</param>
        /// <param name="value">value to set the variable</param>
        /// <param name="objectType">the type the variable is from</param>
        /// <param name="obj">the object that is affected</param>
        public static void SetVariable<T>(ref LineAttributes line, string variable, string value, ref T obj)
        {
            // Add a way to check if the object has a dictionary called actionmapper, and use that instead of searching
            if(variable != "" && value != "")
            {
                Type type = typeof(T);
                FieldInfo field = type.GetField("ActionMapper");
                if (field != null)
                {
                    var dict = field.GetValue(obj) as Dictionary<string, Action<string>>;
                    if (dict.ContainsKey(variable))
                    {
                        dict[variable](value);
                        return;
                    }
                }

                field = type.GetField(variable);
                if (field != null)
                {
                    object newValue = ConvertType(value, field.FieldType);
                    if (newValue != null)
                        field.SetValue(obj, newValue);
                    else
                        Debug.ErrorLog($"{line.ToString()} {variable} is not a variable that can be set properly in SSL");
                }
                else
                {
                    Debug.ErrorLog($"{line.ToString()} {variable} is not a variable, maybe look over it again");
                }
            }
            else
            {
                Debug.ErrorLog($"{line.ToString()} either the variable or value is set blank");
            }
        }

        public static object ConvertType(string value, Type type)
        {
            if (type == typeof(string))
            {
                return value;
            }
            else if (type.IsEnum)
            {
                return Enum.Parse(type, value);
            }
            else
            {
                return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// if a piece of code is a method or not
        /// </summary>
        /// <param name="line"></param>
        /// <param name="code"></param>
        /// <returns>the first bool is weather or not the code is a method, and the second bool is if the code is valid syntax</returns>
        public static (bool, bool) IsMethod(ref LineAttributes line, string code)
        {
            int set = code.IndexOf(':');
            int parenthases = code.IndexOf('(');

            if (set == -1 && parenthases == - 1) // Error
            {
                Debug.LogInvalidCode(line, code);
                return (false, false);
            }
            else
            {
                if(parenthases == -1)
                    return (false, true);
                else if(set == -1) 
                    return (true, true);
                else
                    return (parenthases < set, true);
            }
        }

        public static List<string> GetDifferences(string[] list1,  string[] list2, bool both = false)
        {
            List<string> differences = new List<string>();

            for (int i = 0; i < list1.Length; i++)
            {
                int index = Array.IndexOf(list2, list1[i]);
                if (index == -1)
                {
                    differences.Add(list1[i]);
                }
            }

            if (both)
            {
                for (int i = 0; i < list2.Length; i++)
                {
                    if (!differences.Contains(list2[i]))
                    {
                        int index = Array.IndexOf(list1, list2[i]);
                        if (index == -1)
                        {
                            differences.Add(list2[i]);
                        }
                    }
                }
            }
            
            return differences;
        }
    }
}

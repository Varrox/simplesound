using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
            int last_cut = index + 1;
            bool string_wrap = false;
            List<string> output = new List<string>();

            for (int i = index + 1; i < line.Length; i++)
            {
                // check if current character is the start or end of a string

                if (line[i] == '"')
                {
                    string_wrap = !string_wrap;
                }

                // skip string

                else if (!string_wrap)
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
                            output.Add(line.Substring(last_cut, i - last_cut).Trim());
                            break;
                        }
                    }
                    else if ((line[i] == ',') && ((starts - ends) < 2))
                    {
                        // finish argument

                        output.Add(line.Substring(last_cut, i - last_cut).Trim());
                        last_cut = i + 1;
                    }
                }
            }

            return output.ToArray();
        }

        /// <summary>
        /// Returns a formatted line of code
        /// </summary>
        /// <param name="text">line of code</param>
        /// <returns>formatted code</returns>
        public static string FormatCode(string text)
        {
            int comment_start = text.IndexOf("//");
            if (comment_start == -1)
                return text.Trim();
            return text.Substring(0, comment_start).Trim();
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
                    object new_value = ConvertStringToValue(value, field.FieldType);
                    if (new_value != null)
                        field.SetValue(obj, new_value);
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

        public static object ConvertStringToValue(string value, Type type)
        {
            if (type == typeof(string))
            {
                if (value.StartsWith('"') && value.EndsWith('"'))
                {
                    return value.Substring(1, value.Length - 2);
                }
                return value;
            }
            else if (type.IsEnum)
            {
                return Enum.Parse(type, value);
            }
            else
            {
                bool iterable = false;

                if (type.IsArray)
                    iterable = true;
                else if (type.IsGenericType)
                    if (type.GetGenericTypeDefinition() == typeof(List<>))
                        iterable = true;

                if (iterable)
                {
                    Type array_item_type = type.GetGenericArguments()[0];

                    return typeof(ParsingTools).GetMethod("ParseArray").MakeGenericMethod(array_item_type).Invoke(null, new object[] { value, "," });
                }

                return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
            }
        }

        public static object ConvertStringToObject(string value)
        {
            if (value.StartsWith('"') && value.EndsWith('"'))
            {
                return value.Substring(1, value.Length - 2);
            }
            else if (value.StartsWith('[') && value.EndsWith(']'))
            {
                return ParseArrayAny(value);
            }

            return value; // If no type was found, just return as string
        }

        public static string ConvertValueToString(object value)
        {
            Type type = value.GetType();

            if (type == typeof(string))
            {
                return $"\"{(string)value}\"";
            }
            else if (type.IsEnum)
            {
                return Enum.GetName(type, value);
            }
            else if (type == typeof(int))
            {
                return ((int)value).ToString();
            }
            else if (type == typeof(float))
            {
                return ((float)value).ToString();
            }
            else if (type == typeof(bool))
            {
                return ((bool)value).ToString().ToLower();
            }
            else if (IsTypeEnumeral(type))
            {
                Array array;

                if (type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    array = Enumerable.Range(0, ((IList)value).Count).Select(i => ((IList)value)[i]).ToArray();
                }
                else
                {
                    array = (Array)value;
                }

                return StringifyArray(array);
            }
                
            return "";
        }

        public static List<string> SmartSplit(string line, string split_chars)
        {
            int starts = 0, ends = 0, last_cut = 0;
            bool wrap = false, last_ignore = false;
            List<string> result = new List<string>();

            for(int i  = 0; i < line.Length; i++)
            {
                char c = line[i];

                if(c == '\\')
                    last_ignore = true;

                else if (c == '"' && !last_ignore)
                    wrap = !wrap;
                
                if (!last_ignore && !wrap)
                {
                    if ("({[".Contains(c))
                        starts++;
                    else if (")}]".Contains(c))
                        ends++;
                    else if (split_chars.Contains(c) && (starts == ends))
                    {
                        result.Add(line.Substring(last_cut, i - last_cut).Trim());
                        last_cut = i + 1;
                    }
                }
                
                if(i == line.Length - 1)
                    result.Add(line.Substring(last_cut, i + 1 - last_cut).Trim());

                last_ignore = false;
            }

            return result;
        }

        public static string StringifyArray(Array array, string start = "[", string end = "]", string in_between = ", ")
        {
            string result = start;

            for (int i = 0; i < array.Length; i++)
            {
                result += $"{ParsingTools.ConvertValueToString(array.GetValue(i))}{(i == array.Length - 1 ? end : in_between)}";
            }

            return result;
        }

        public static List<T> ParseArray<T>(string array, string split = ",")
        {
            List<T> result = new List<T>();

            array = array.Trim();
            array = array.Substring(1, array.Length - 2); // Remove []

            List<string> list = SmartSplit(array, split);

            for(int i = 0; i < list.Count; i++)
            {
                result.Add((T)ConvertStringToValue(list[i], typeof(T)));
            }

            return result;
        }

        public static List<object> ParseArrayAny(string array, string split = ",")
        {
            List<object> result = new List<object>();

            array = array.Trim();
            array = array.Substring(1, array.Length - 2); // Remove []

            List<string> list = SmartSplit(array, split);

            for (int i = 0; i < list.Count; i++)
            {
                result.Add(ConvertStringToObject(list[i]));
            }

            return result;
        }

        public static bool IsTypeEnumeral(Type type)
        {
            bool iterable = false;

            if (type.IsArray)
                iterable = true;
            else if (type.IsGenericType)
                if (type.GetGenericTypeDefinition() == typeof(List<>))
                    iterable = true;

            return iterable;
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

        public static List<T> GetDifferences<T>(T[] list1,  T[] list2, bool both = false)
        {
            List<T> differences = new List<T>();

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

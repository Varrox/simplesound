using System;
using System.Reflection;

namespace SSLParser
{
    public class ParsingTools
    {
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
            return text.Split("//")[0].Trim();
        }

        /// <summary>
        /// Sets a variable of a string name, to a value from a string.
        /// </summary>
        /// <param name="variable">variable to be set</param>
        /// <param name="value">value to set the variable</param>
        /// <param name="objectType">the type the variable is from</param>
        /// <param name="obj">the object that is affected</param>
        public static void SetVariable(string variable, string value, ref Type objectType, object obj)
        {
            FieldInfo field = objectType.GetField(variable);
            if (field != null)
            {
                object newValue = ConvertType(value, field.FieldType);

                if (newValue != null)
                    field.SetValue(obj, newValue);
                else
                    Debug.ErrorLog($"{variable} is not a variable that can be set properly in SSL");
            }
            else
            {
                Debug.ErrorLog($"{variable} is not a variable, maybe look over it again");
            }
        }

        public static object ConvertType(string value, Type type)
        {
            if (type is string)
            {
                return value;
            }
            else if (type is int)
            {
                return Convert.ToInt32(value);
            }
            else if (type is long)
            {
                return Convert.ToInt64(value);
            }
            else if (type is float)
            {
                return Convert.ToSingle(value);
            }
            else if (type is double)
            {
                return Convert.ToDouble(value);
            }
            else
            {
                return null;
            }
        }
    }
}

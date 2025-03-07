namespace SSLParser
{
    public class ParsingTools
    {
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
    }
}

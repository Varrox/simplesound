using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SSLParser
{
    public class MainParser
    {
        static LineAttributes line = new LineAttributes(null, 0);

        public static Playlist ParsePlaylist(string path)
        {
            string[] lines = File.ReadAllLines(path);
            line.path = path;

            Playlist playlist = new Playlist(Path.GetFileNameWithoutExtension(path), null, null);

            for (int i = 0; i < lines.Length; i++)
            {
                string trimmed_start_line = lines[i].Trim();

                if (trimmed_start_line.Length == 0) continue;

                if (trimmed_start_line.StartsWith("Config"))
                {
                    ParseConfiguration(ref playlist, ref lines, ref i);
                }
                else if (trimmed_start_line.StartsWith("Sound"))
                {
                    ParseSound(ref playlist, ref lines, ref i);
                }
                else if (trimmed_start_line.StartsWith("UI"))
                {

                }
                else if (trimmed_start_line.StartsWith("Songs"))
                {
                    ParseSongs(ref playlist, ref lines, ref i);
                }
            }

            return playlist;
        }

        public static void ParseConfiguration(ref Playlist playlist, ref string[] lines, ref int index)
        {
            for (int i = index + 1; i < lines.Length; i++)
            {
                line.line = i;
                string trimmed_line = ParsingTools.FormatCode(lines[i]);

                if (trimmed_line == "}")
                {
                    index = i;
                    return;
                }
                else if (trimmed_line == "{")
                {
                    continue;
                }

                if (trimmed_line.Length > 0)
                {
                    string[] split = SplitLine(trimmed_line);
                    string var = split[0];
                    string value = split[1];

                    ParsingTools.SetVariable(ref line, var, value, ref playlist);

                    if (trimmed_line.EndsWith("}"))
                    {
                        index = i;
                        return;
                    }
                }
            }
        }

        public static void ParseSound(ref Playlist playlist, ref string[] lines, ref int index)
        {
            for (int i = index + 1; i < lines.Length; i++)
            {
                line.line = i;
                string trimmed_line = ParsingTools.FormatCode(lines[i]);

                if (trimmed_line == "}")
                {
                    index = i;
                    return;
                }
                else if (trimmed_line == "{")
                {
                    continue;
                }

                if (trimmed_line.Length > 0)
                {
                    string[] split = SplitLine(trimmed_line);
                    string var = split[0];
                    string value = split[1];

                    if(var == "VolumeReactive")
                    {
                        string[] colors = ParsingTools.GetInParenthases(value, out string selected_variable);
                        bool4 color = new bool4(colors[0].Contains('r'), colors[0].Contains('g'), colors[0].Contains('b'), colors[0].Contains('a'));
                        switch (selected_variable)
                        {
                            case "Overlay-Color":
                                playlist.customInfo.overlay_reactive = color;
                                break;
                            case "Background-Image":
                                playlist.customInfo.background_reactive = color;
                                break;
                        }
                    }

                    if (trimmed_line.EndsWith("}"))
                    {
                        index = i;
                        return;
                    }
                }
            }
        }

        public static void ParseSongs(ref Playlist playlist, ref string[] lines, ref int index)
        {
            playlist.Songs = new List<string>();
            for (int i = index + 1; i < lines.Length; i++)
            {
                line.line = i;
                string trimmed_line = lines[i].Trim();
                if (trimmed_line == "}")
                {
                    index = i;
                    return;
                }
                else if (trimmed_line == "{")
                {
                    continue;
                }

                if (trimmed_line.Length > 0)
                {
                    string input_path = Path.Combine(SaveSystem.USER_DATA, "Music Folders", ParsingTools.FormatCode(trimmed_line));

                    if (Tools.ValidAudioFile(input_path))
                    {
                        if (!Metadata.IsFileCorrupt(input_path))
                        {
                            playlist.Songs.Add(input_path);
                        }
                        else
                        {
                            Debug.ErrorLog($"{input_path} is corrupted");
                        }
                    }
                    else
                    {
                        if (Directory.Exists(input_path))
                        {
                            string[] paths = Directory.GetFiles(input_path);
                            foreach (string path in paths)
                            {
                                if (Tools.ValidAudioFile(path))
                                {
                                    if (!Metadata.IsFileCorrupt(path))
                                    {
                                        playlist.Songs.Add(path);
                                    }
                                    else
                                    {
                                        Debug.ErrorLog($"{path} is corrupted");
                                    }
                                }
                            }
                        }
                        else
                            Debug.ErrorLog($"{input_path} is not a valid audio file or folder / directory");
                    }

                    if (trimmed_line.EndsWith("}"))
                    {
                        index = i;
                        return;
                    }
                }
            }
        }

        public static void ParseFolders(ref Playlist playlist, ref string[] lines, ref int index)
        {
            playlist.Folders = new List<string>();
            for (int i = index + 1; i < lines.Length; i++)
            {
                line.line = i;
                string trimmed_line = ParsingTools.FormatCode(lines[i]);
                
                if (trimmed_line == "}")
                {
                    index = i;
                    return;
                }
                else if (trimmed_line == "{")
                {
                    continue;
                }

                if (trimmed_line.Length > 0)
                {
                    if (Directory.Exists(trimmed_line))
                    {
                        playlist.Folders.Add(trimmed_line);
                    }

                    if (trimmed_line.EndsWith("}"))
                    {
                        index = i;
                        return;
                    }
                }
            }
        }

        public static void ParseFile<T>(string path, out T output_object) where T : new()
        {
            string[] lines = File.ReadAllLines(path);
            line.path = path;

            output_object = new();

            for (int i = 0; i < lines.Length; i++)
            {
                string trimmed_start_line = lines[i].Trim();

                if (trimmed_start_line.Length == 0) continue;

                if (trimmed_start_line.StartsWith(typeof(T).Name))
                {
                    for (int j = i + 1; j < lines.Length; j++)
                    {
                        line.line = j;
                        string trimmed_line = ParsingTools.FormatCode(lines[j]);

                        if (trimmed_line == "}")
                        {
                            i = j;
                            break;
                        }
                        else if (trimmed_line == "{")
                        {
                            continue;
                        }

                        if (trimmed_line.Length > 0)
                        {
                            string[] split = SplitLine(trimmed_line);
                            string var = split[0];
                            string value = split[1];

                            ParsingTools.SetVariable(ref line, var, value, ref output_object);

                            if (trimmed_line.EndsWith("}"))
                            {
                                i = j;
                                break;
                            }
                        }
                    }
                }
            }
        }

        public static string StringifyObject<T>(T input_object)
        {
            string output = $"{typeof(T).Name}\n" + "{\n", after = "";
            
            foreach (FieldInfo prop in typeof(T).GetFields())
            {
                if (prop.FieldType == typeof(Array) || prop.FieldType == typeof(List<>)) // Iterable
                {
                    IEnumerable array = null;

                    if (prop.FieldType == typeof(Array))
                    {
                        array = prop.GetValue(input_object) as Array;
                    }
                    else
                    {
                        object list = prop.GetValue(input_object);

                        Type type = typeof(List<>).GetGenericArguments()[0];
                        Type genericListType = typeof(List<>).MakeGenericType(type);
                        
                        array = (IList)Activator.CreateInstance(genericListType);
                    }

                    if (array != null)
                    {
                        after += prop.Name + "\n{\n";

                        foreach (object item in array)
                        {
                            after += $"\t{ParsingTools.ConvertValueToString(item)}";
                        }

                        after += "\n}\n\n";
                    }

                    continue;
                }

                // Non-Iterable

                object variable = prop.GetValue(input_object);

                output += $"\t{prop.Name} : {ParsingTools.ConvertValueToString(variable)}\n";
            }

            output += "}\n\n" + after;
            
            return output;
        }

        /// <summary>
        /// Splits only a line formatted like this -> Variable : Value
        /// </summary>
        /// <param name="line">line to split</param>
        /// <returns>formatted line</returns>
        public static string[] SplitLine(string line)
        {
            line = ParsingTools.FormatCode(line);
            int index = line.IndexOf(':');
            return new[] {line.Substring(0, index).Trim(), line.Substring(index + 1).Trim()};
        }
    }
}
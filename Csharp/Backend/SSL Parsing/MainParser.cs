using System.Collections.Generic;
using System.IO;
using System;
using static System.Net.Mime.MediaTypeNames;

namespace SSLParser
{
    public class MainParser
    {
        public static Playlist ParsePlaylist(string path)
        {
            string[] lines = File.ReadAllLines(path);

            Playlist playlist = new Playlist(Path.GetFileNameWithoutExtension(path), null, null);

            for (int i = 0; i < lines.Length; i++)
            {
                string trimmedStartLine = lines[i].Trim();

                if (trimmedStartLine.Length == 0) continue;

                if (trimmedStartLine.StartsWith("Tags"))
                {
                    ParseTags(ref playlist, ref lines, ref i);
                }
                else if (trimmedStartLine.StartsWith("Sound"))
                {
                    ParseSound(ref playlist, ref lines, ref i);
                }
                else if (trimmedStartLine.StartsWith("Images"))
                {
                    ParseImages(ref playlist, ref lines, ref i);
                }
                else if (trimmedStartLine.StartsWith("Songs"))
                {
                    ParseSongs(ref playlist, ref lines, ref i);
                }
                else if (trimmedStartLine.StartsWith("Folders"))
                {
                    ParseFolders(ref playlist, ref lines, ref i);
                }
            }

            return playlist;
        }

        public static void ParseTags(ref Playlist playlist, ref string[] lines, ref int index)
        {
            for (int i = index + 1; i < lines.Length; i++)
            {
                string trimmedLine = ParsingTools.FormatCode(lines[i]);
                if (trimmedLine == "}")
                {
                    index = i;
                    return;
                }
                else if (trimmedLine == "{")
                {
                    continue;
                }

                if (trimmedLine.Length > 0)
                {
                    string[] split = SplitLine(trimmedLine);
                    string var = split[0];
                    string value = split[1];

                    switch (var)
                    {
                        case "Type":
                            playlist.SetType(value);
                            break;
                        case "Artist":
                            playlist.artist = value;
                            break;
                        case "Overlay-Color":
                            playlist.customInfo.overlayColor = value;
                            break;
                    }

                    if (trimmedLine.EndsWith("}"))
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
                string trimmedLine = ParsingTools.FormatCode(lines[i]);

                if (trimmedLine == "}")
                {
                    index = i;
                    return;
                }
                else if (trimmedLine == "{")
                {
                    continue;
                }

                if (trimmedLine.Length > 0)
                {
                    string[] split = SplitLine(trimmedLine);
                    string var = split[0];
                    string value = split[1];

                    switch (var)
                    {
                        case "Volume-Reactive":
                            string[] vars = value.Split(',');
                            foreach (string var2 in vars)
                            {
                                string[] colors = ParsingTools.GetInParenthases(var2, out string selectedVar);
                                bool4 color = new bool4(colors[0].Contains('r'), colors[0].Contains('g'), colors[0].Contains('b'), colors[0].Contains('a'));
                                switch (selectedVar)
                                {
                                    case "Overlay-Color":
                                        playlist.customInfo.overlayReactive = color;
                                        break;
                                    case "Background-Image":
                                        playlist.customInfo.backgroundReactive = color;
                                        break;
                                }
                            }
                            break;
                        case "Volume":
                            playlist.customInfo.volume = Convert.ToSingle(value);
                            break;
                        case "Speed":
                            playlist.customInfo.speed = Convert.ToSingle(value);
                            break;
                        case "Reverb":
                            playlist.customInfo.reverb = Convert.ToSingle(value);
                            break;
                    }

                    if (trimmedLine.EndsWith("}"))
                    {
                        index = i;
                        return;
                    }
                }
            }
        }

        public static void ParseImages(ref Playlist playlist, ref string[] lines, ref int index)
        {
            for (int i = index + 1; i < lines.Length; i++)
            {
                string trimmedLine = ParsingTools.FormatCode(lines[i]);
                if (trimmedLine == "}")
                {
                    index = i;
                    return;
                }
                else if (trimmedLine == "{")
                {
                    continue;
                }

                if (trimmedLine.Length > 0)
                {
                    string[] split = SplitLine(trimmedLine);
                    string variable = split[0];
                    string value = split[1];

                    if (variable.Length > 0)
                    {
                        switch (variable)
                        {
                            case "Cover":
                                playlist.Coverpath = File.Exists(value) ? value : null;
                                break;
                            case "Background-Image":
                                playlist.customInfo.backgroundPath = File.Exists(value) ? value : null;
                                break;
                        }

                        if (trimmedLine.EndsWith("}"))
                        {
                            index = i;
                            return;
                        }
                    }
                }
            }
        }

        public static void ParseSongs(ref Playlist playlist, ref string[] lines, ref int index)
        {
            playlist.Songs = new List<string>();
            for (int s = index + 1; s < lines.Length; s++)
            {
                string trimmedLine = lines[s].Trim();
                if (trimmedLine == "}")
                {
                    index = s;
                    return;
                }
                else if (trimmedLine == "{")
                {
                    continue;
                }

                if (trimmedLine.Length > 0)
                {
                    string songPath = Path.Combine(SaveSystem.UserData, "Music Folders", ParsingTools.FormatCode(trimmedLine));

                    if (Tools.ValidAudioFile(songPath))
                    {
                        playlist.Songs.Add(songPath);
                    }

                    if (trimmedLine.EndsWith("}"))
                    {
                        index = s;
                        return;
                    }
                }
            }
        }

        public static void ParseFolders(ref Playlist playlist, ref string[] lines, ref int index)
        {
            playlist.Folders = new List<string>();
            for (int f = index + 1; f < lines.Length; f++)
            {
                string trimmedLine = ParsingTools.FormatCode(lines[f]);
                
                if (trimmedLine == "}")
                {
                    index = f;
                    return;
                }
                else if (trimmedLine == "{")
                {
                    continue;
                }

                if (trimmedLine.Length > 0)
                {
                    if (Directory.Exists(trimmedLine))
                    {
                        playlist.Folders.Add(trimmedLine);
                    }

                    if (trimmedLine.EndsWith("}"))
                    {
                        index = f;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Splits only a line formatted like this -> Variable : Value
        /// </summary>
        /// <param name="line">line to split</param>
        /// <returns>formatted line</returns>
        public static string[] SplitLine(string line)
        {
            line = line.Split("//")[0];
            int index = line.IndexOf(':');
            return new[] {line.Substring(0, index).Trim(), line.Substring(index + 1).Trim()};
        }
    }
}
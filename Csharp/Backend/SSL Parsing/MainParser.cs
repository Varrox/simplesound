using System.Collections.Generic;
using System.IO;
using System;

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
                    for (int t = i + 1; t < lines.Length; t++)
                    {
                        string trimmedLine = lines[t].Split("//")[0].Trim();
                        if (trimmedLine == "}")
                        {
                            i = t;
                            break;
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
                                i = t;
                                break;
                            }
                        }
                    }
                }
                else if (trimmedStartLine.StartsWith("Sound"))
                {
                    ParseSound(ref playlist, ref lines, ref i);
                }
                else if (trimmedStartLine.StartsWith("Images"))
                {
                    for (int c = i + 1; c < lines.Length; c++)
                    {
                        string trimmedLine = lines[c].Split("//")[0].Trim();
                        if (trimmedLine == "}")
                        {
                            i = c;
                            break;
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

                            if (var.Length > 0)
                            {
                                switch (var)
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
                                    i = c;
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (trimmedStartLine.StartsWith("Songs"))
                {
                    playlist.Songs = new List<string>();
                    for (int s = i + 1; s < lines.Length; s++)
                    {
                        string trimmedLine = lines[s].Trim();
                        if (trimmedLine == "}")
                        {
                            i = s;
                            break;
                        }
                        else if (trimmedLine == "{")
                        {
                            continue;
                        }

                        if (trimmedLine.Length > 0)
                        {
                            string songPath = Path.Combine(SaveSystem.UserData, "Music Folders", trimmedLine.Split("//")[0].Trim());

                            if (Tools.ValidAudioFile(songPath))
                            {
                                playlist.Songs.Add(songPath);

                                if (trimmedLine.EndsWith("}"))
                                {
                                    i = s;
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (trimmedStartLine.StartsWith("Folders"))
                {
                    ParseFolders(ref playlist, ref lines, ref i);
                }
            }

            return playlist;
        }

        public static void ParseSound(ref Playlist playlist, ref string[] lines, ref int index)
        {
            for (int s = index + 1; s < lines.Length; s++)
            {
                string trimmedLine = lines[s].Split("//")[0].Trim();

                if (trimmedLine == "}")
                {
                    index = s;
                    break;
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
                        index = s;
                        break;
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
                    break;
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

                        if (trimmedLine.EndsWith("}"))
                        {
                            index = f;
                            break;
                        }
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
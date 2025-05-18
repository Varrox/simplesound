using System.Collections.Generic;
using System.IO;

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
                string trimmedStartLine = lines[i].Trim();

                if (trimmedStartLine.Length == 0) continue;

                if (trimmedStartLine.StartsWith("Config"))
                {
                    ParseConfiguration(ref playlist, ref lines, ref i);
                }
                else if (trimmedStartLine.StartsWith("Sound"))
                {
                    ParseSound(ref playlist, ref lines, ref i);
                }
                else if (trimmedStartLine.StartsWith("UI"))
                {

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

        public static void ParseConfiguration(ref Playlist playlist, ref string[] lines, ref int index)
        {
            for (int i = index + 1; i < lines.Length; i++)
            {
                line.line = i;
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

                    ParsingTools.SetVariable(ref line, var, value, ref playlist);

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
                line.line = i;
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

                    if(var == "VolumeReactive")
                    {
                        string[] colors = ParsingTools.GetInParenthases(value, out string selectedVar);
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

                    if (trimmedLine.EndsWith("}"))
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
                string trimmedLine = lines[i].Trim();
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
                    string songPath = Path.Combine(SaveSystem.UserData, "Music Folders", ParsingTools.FormatCode(trimmedLine));

                    if (Tools.ValidAudioFile(songPath))
                    {
                        playlist.Songs.Add(songPath);
                    }

                    if (trimmedLine.EndsWith("}"))
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
                    if (Directory.Exists(trimmedLine))
                    {
                        playlist.Folders.Add(trimmedLine);
                    }

                    if (trimmedLine.EndsWith("}"))
                    {
                        index = i;
                        return;
                    }
                }
            }
        }

        public static void UpdatePlaylist(ref Playlist playlist)
        {
            // Work on it
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
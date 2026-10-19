using Godot;
using System.Collections.Generic;

struct SearchHelper {
    static public List<string> track_names, track_artists;

    struct PlayIndex
    {
        int playlist_index, track_index;
    }

    public static PlayIndex[] SearchSongs(string search_query, int max_results) {
        if (track_names == null || track_artists == null) {
            SetupTrackDatas();
        }

        // Track name
        // Track artist

        // TODO : Track lyrics

        FuzzySearch fuzzy = new();

        FuzzySearchMatch[] results = new FuzzySearchMatch[track_names.Count];
        Dictionary<int, PlayIndex> = new Dictionary<

        for(int i = 0; i < track_names.Count; i++)
        {
            FuzzySearchMatch result_name = fuzzy.Search(search_query, track_names[i]);
            FuzzySearchMatch result_artist = fuzzy.Search(search_query, track_artists[i]);
            
        }

        // fuzzy.SearchAll("health potion", items)
    }

    public static PlayIndex GetPlayIndex(int list_index)
    {
        
    }

    public static void SetupTrackDatas() {
        track_names = new List<String>();
        track_artists = new List<String>();

        for(int i = 0; i < Globals.main.playlists.Count; i++) {
            for(int j = 0; j < Globals.main.playlists[i].tracks.Count; j++) {
                track_names.Add(Tools.GetMediaTitle(Globals.main.playlists[i].tracks[i]));
                track_artists.Add(Metadata.GetArtist(Globals.main.playlists[i].tracks[i]));
            }
        }
    }
}
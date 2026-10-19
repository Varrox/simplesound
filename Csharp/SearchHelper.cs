using Godot;
using System.Collections.Generic;
using System.Linq;

struct SearchHelper {
    public struct PlayIndex {
        public int playlist_index, track_index;
    }

    public static PlayIndex[] SearchSongs(string search_query) {
        // TODO : Track lyrics

        FuzzySearch fuzzy = new FuzzySearch();

        Dictionary<int, PlayIndex> results = new Dictionary<int, PlayIndex>();

        for(int i = 0; i < Globals.main.playlists.Count; i++) {
            for(int j = 0; j < Globals.main.playlists[i].tracks.Count; j++) {
                string track = Globals.main.playlists[i].tracks[j];

                FuzzySearchMatch result_name = fuzzy.Search(search_query, Tools.GetMediaTitle(track));
                FuzzySearchMatch result_artist = fuzzy.Search(search_query, Metadata.GetArtist(track));

                if (result_name == null && result_artist == null) continue;
                
                int score = (result_name != null ? result_name.Score : -10) + (result_artist != null ? result_artist.Score : -10);

                if (score <= 0) continue;
                
                while (results.ContainsKey(score)) score--;

                results.Add(score, new PlayIndex {playlist_index = i, track_index = j});
            }
        }

        fuzzy.Dispose();

        return results.OrderBy(k => k.Key).Select(v => v.Value).ToArray();
    }
}
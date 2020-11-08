using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using aire_logic_technical_test.Models;
using MetaBrainz.MusicBrainz;
using MetaBrainz.MusicBrainz.Interfaces.Entities;

namespace aire_logic_technical_test
{
    class Controller
    {
        public async Task Run()
        {
            Console.WriteLine("Please enter the name of the artist you would like to find average song length for:");
            var input = Console.ReadLine();

            Console.WriteLine("Finding artist...");

            var artist = await FindArtistByName(input);

            Console.WriteLine($"Artist found. Calculating average song length for artist {artist.Name}");
            Console.WriteLine("Please wait...");

            var average = await GetArtistAverageSongLength(artist);

            if (average < 0)
            {
                Console.WriteLine("Unfortunately, we couldn't find the lyrics for any songs by that artist, or all of that artist's works are instrumental.");
            }
            else
            {
                Console.WriteLine($"Average song length: {average} words");
            }

            Console.ReadLine();
        }

        public async Task<int> GetSongLength(string artist, string songTitle)
        {
            var client = new LyricsClient();
            var lyricsResponse = await client.GetLyricsAsync(artist, songTitle);
            var wordCount = lyricsResponse.lyrics.Split().Length;

            // If wordCount == 1, set it to 0, as this almost certainly means the returned lyrics are either empty ("".Split() has length 1) or consist of the single word
            // "(Instrumental)" or similar.
            // Note that if the API can't find a song matching the artist and title, it simply returns an empty string
            return wordCount == 1 ? 0 : wordCount;
        }

        public async Task<IArtist> FindArtistByName(string artist)
        {
            var q = new Query("Aire Logic Technical Test", "1.0", "heliomance.github.com");

            return (await q.FindArtistsAsync($"name:{artist}")).Results[0].Item;
        }

        // BUG: This is currently returning inconsistent values if run multiple times with the same artist. Check responses from lyrics API?
        public async Task<double> GetArtistAverageSongLength(IArtist artist)
        {
            var q = new Query("Aire Logic Technical Test", "1.0", "heliomance.github.com");
            var lyricLengthLookups = new List<Task<int>>();

            var works = await q.BrowseArtistWorksAsync(artist.Id);
            lyricLengthLookups.AddRange(works.Results.Select(work => GetSongLength(artist.Name, work.Title)));

            while (works.Offset + works.Results.Count < works.TotalResults)
            {
                works = await works.NextAsync();
                lyricLengthLookups.AddRange(works.Results.Select(work => GetSongLength(artist.Name, work.Title)));
            }

            // Only count non-instrumental songs where the lyrics where actually found
            var songLengths = (await Task.WhenAll(lyricLengthLookups)).Where(l => l > 0).ToList();

            // If no songs are found, or all songs are instrumental, return -1 without further calculation (avoids an error if the lyrics API doesn't have any songs by that artist)
            if (songLengths.Any())
            {
                return songLengths.Average();
            }

            return -1;
        }
    }
}
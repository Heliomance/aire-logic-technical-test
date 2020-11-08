using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using aire_logic_technical_test.Models;
using System.Net.Http.Headers;
using System.Web;

namespace aire_logic_technical_test
{
    class LyricsClient
    {
        private static HttpClient client = new HttpClient();

        static LyricsClient()
        {
            client.BaseAddress = new Uri("https://api.lyrics.ovh/v1/");
        }

        public async Task<LyricsResponseModel> GetLyricsAsync(string artist, string title)
        {
            LyricsResponseModel lyrics = null;
            // The lyrics API breaks if passed artists or titles containing the "/" character. I chose to deal with this by replacing / with a space, but I suspect this will generally result 
            // in no lyrics being found. Notably, if you search for a song title with a / in on lyrics.ovh, it will not return any results.
            string path = Uri.EscapeUriString($"{artist.Replace("/", " ")}/{title.Replace("/"," ")}");
            HttpResponseMessage response = await client.GetAsync(path);

            if (response.IsSuccessStatusCode)
            {
                lyrics = await response.Content.ReadAsAsync<LyricsResponseModel>();
            }

            return lyrics;
        }
    }
}

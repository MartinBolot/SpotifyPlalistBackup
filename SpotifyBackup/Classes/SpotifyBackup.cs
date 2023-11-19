using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace SpotifyPlaylistBackup.Classes
{
    public class SpotifyBackup
    {
        public SpotifyBackup(SpotifyWebRequestManager swrm)
        {
            SWRManager = swrm;
        }

        public async Task<List<Track>> GetTracks(IEnumerable<string> trackIds) {
            var trackIdsCount = trackIds.Count();
            var trackList = new List<Track>();

            if(trackIdsCount > MAX_TRACK_COUNT)
            {
                var batches = trackIds
                    .Select((track, index) => new { track, index })
                    .GroupBy(track => track.index / MAX_TRACK_COUNT)
                    .Select(group => group.Select(el => el.track));
                foreach(var batch in batches)
                {
                    trackList.AddRange(await SWRManager.GetTracks(batch));
                    Console.WriteLine($"requested {MAX_TRACK_COUNT} tracks");
                }
            }
            // nécessaire ?
            else
            {
                trackList = await SWRManager.GetTracks(trackIds);
            }
            return trackList;
        }


        public static async Task<string> GetAccessToken(string clientId, string clientSecret)
        {
            var accesToken = "";
            var client = new HttpClient();
            var test = new HttpRequestMessage()
            {
                RequestUri = new Uri("https://accounts.spotify.com/api/token"),
                Method = HttpMethod.Post,
                Content = new StringContent(
                    $"grant_type=client_credentials&client_id={clientId}&client_secret={clientSecret}",
                    Encoding.UTF8,
                    "application/x-www-form-urlencoded"
                )

            };
            var response = await client.SendAsync(test);
            var responseText = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(responseText);
            if(json != null && json["access_token"] != null)
            {
                accesToken = json["access_token"].ToString();
            }

            return accesToken;
        }

        const int MAX_TRACK_COUNT = 50;
        private readonly SpotifyWebRequestManager SWRManager;
    }
}

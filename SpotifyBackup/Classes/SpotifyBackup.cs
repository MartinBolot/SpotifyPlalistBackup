using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
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
            var tracks = await SWRManager.GetTracks(trackIds);
            return tracks;
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

        private readonly SpotifyWebRequestManager SWRManager;
    }
}

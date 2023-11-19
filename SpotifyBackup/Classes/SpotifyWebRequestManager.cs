using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SpotifyPlaylistBackup.Classes
{
    public class SpotifyWebRequestManager
    {
        public SpotifyWebRequestManager() {}
        public SpotifyWebRequestManager(string token)
        {
            AccessToken = token;
        }

        public Track GetTrack(string trackId)
        {
            return new Track { Song = "", Artist = "", Album = ""};
        }

        public async Task<List<Track>> GetTracks(IEnumerable<string> trackIds)
        {
            var tracks = new List<Track>();
            var trackIdList = trackIds.ToList();
            if(trackIdList.Count() > 50)
            {
                throw new Exception("cannot handle more thant 50 tracks");
            }

            var client = new HttpClient();
            var marketParam = "market=FR";
            var requestParams = $"{marketParam}&ids={string.Join(",", trackIdList)}";
            var test = new HttpRequestMessage()
            {
                RequestUri = new Uri($"https://api.spotify.com/v1/tracks?{requestParams}"),
                Method = HttpMethod.Get

            };
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {AccessToken}");
            var response = await client.SendAsync(test);
            var json = await response.Content.ReadAsStringAsync();

            try
            {
                var parsedJson = JObject.Parse(json);
                var tracksArray = (JArray)parsedJson["tracks"];
                tracks = tracksArray
                    .Select(track => new Track()
                    {
                        Song = track["name"].ToString(),
                        Artist = string.Join(",", track["artists"]
                            .Select(artist => artist["name"].ToString())
                        ),//.Aggregate("",(curr, next) => curr = curr + ", " + next),
                    Album = track["album"]["name"].ToString()
                    })
                    .ToList();
            }
            catch(Exception e)
            {
                Console.WriteLine($"Error getting track ids {trackIdList}");
            }

            return tracks;
        }

        private async Task<string> GetAccessToken(string clientId, string clientSecret)
        {
            var client = new HttpClient();
            var test = new HttpRequestMessage()
            {
                RequestUri = new Uri(BaseUrlAccount),
                Method = HttpMethod.Post,
                Content = new StringContent(
                    $"grant_type=client_credentials&client_id={clientId}&client_secrect={clientSecret}"
                )

            };
            var token = await client.SendAsync(test);
            return token.Content.ToString();
        }

        const string BaseUrl = "https://api.spotify.com/v1";
        const string BaseUrlAccount = "https://account.stpotify.com/api/token";

        private readonly string TracksUrl = $@"{BaseUrl}/tracks";
        private readonly string AccessToken;
    }
}

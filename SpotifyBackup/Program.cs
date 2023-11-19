using System;
using SpotifyPlaylistBackup.Classes;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.Collections.Generic;

namespace SpotifyPlaylistBackup
{
    public static class Program
    {
       /*public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }*/

        public static async Task<int> Main(string[] args)
        {
            Console.WriteLine("Spotify Backup");

            const string playlistTracksFilePath = @"/Users/martinbolot/Documents/Martin/Perso/Musique";
            const string playlistTracksFileName = "spotify_titres_likes.txt";
            var playlistTracksFileFullPath = Path.Combine(
                playlistTracksFilePath,
                playlistTracksFileName
            );
            var myPlaylistTracks = File.ReadAllText(playlistTracksFileFullPath);
            var playListTracks = myPlaylistTracks.Replace("https://open.spotify.com/track/", "");
            var trackIds = playListTracks.Split("\r\n").Take(50);

            var config = new ConfigurationBuilder()
                .AddJsonFile("credentials.json")
                .Build();

            var clientId = config.GetRequiredSection("clientId").Get<string>();
            var clientSecret = config.GetRequiredSection("clientSecret").Get<string>();

            var accessToken = await SpotifyBackup.GetAccessToken(clientId, clientSecret);
            // TODO mettre en place un système de logging
            Console.WriteLine($"accessToken : {accessToken}");
            var spotifyWebRequestManager = new SpotifyWebRequestManager(accessToken);
            var spotifyBackup = new SpotifyBackup(spotifyWebRequestManager);

            // generate output line
            var songList = new List<string>();
            if (!string.IsNullOrEmpty(accessToken))
            {
                var tracks = await spotifyBackup.GetTracks(trackIds);
                var trackStringList = tracks.Select(track => track.ToString()).ToList();
                songList.AddRange(trackStringList);
            }

            // write output
            const string outputFolderPath = @"/Users/martinbolot/Documents/Martin/Perso/Musique";
            const string outputFileName = "backup_spotify_titres_likes.txt";
            var outputFilePath = Path.Combine(outputFolderPath, outputFileName);
            File.WriteAllLines(outputFilePath, songList);

            return 0;
            /*
            var tracks = spotifyBackup.GetTracks(trackIds);
            foreach(var track in tracks)
            {
                var displayInfo = $"{track.Song}, {track.Artist}, {track.Album}";
                Console.WriteLine(displayInfo);
            }
            */

            /*
            //var token = await GetAccessToken(clientId, clientSecret);
            var token = "BQBfP2w-JlKOWe0_DXRVSuORY9YxFv2NbYFWULwCNEfcwBycjZgoj8tHry4uggzzn9xvLoLc_EPxaylToP2Ypuv_4AdVOVz2wqric9w1622ecYnaqX0";
            Console.WriteLine($"token : {token}");


            var client = new HttpClient();
            var requestParams = "market=FR&ids=27hhIs2fp6w06N5zx4Eaa5,2EghCFmwn5k0sQsxLH53lx,4N84Uh40oFI6GCmFlGYkwA";
            var test = new HttpRequestMessage()
            {
                RequestUri = new Uri($"https://api.spotify.com/v1/tracks?{requestParams}"),
                Method = HttpMethod.Get

            };
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.SendAsync(test);
            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine(json);
            */


        }

    }
}

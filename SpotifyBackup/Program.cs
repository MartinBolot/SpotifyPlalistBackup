using System;
using SpotifyPlaylistBackup.Classes;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
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
            var trackIds = playListTracks.Split("\r\n");

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
            Console.WriteLine("Tracks written to file");

            return 0;


        }

    }
}

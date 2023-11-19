using System;
namespace SpotifyPlaylistBackup.Classes
{
    public class Track
    {
        public Track()
        {
        }

        public override string ToString()
        {
            return $"Song : \"{Song}\" - Artist : \"{Artist}\" - Album : \"{Album}\"";
        }
        public string Song { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
    }
}

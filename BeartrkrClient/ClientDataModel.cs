namespace BeartrkrClient
{
    public class ClientDataModel
    {
        public string clientKey;
        public AppTimePair[] appTimes;
        public string currentSong, currentSongUrl;
    }

    public class AppTimePair
    {
        public string appName;
        public int time; // in seconds
        public string platform; // i guess this isn't a pair anymore, but fuck it
    }
}

namespace ImageProcessor.UI.Constants
{
    public class StaticMessages
    {
        public const string SelectImage = "Select an image";
        public const string Converting = "Converting...";
        public static string TimeElapsed(long ms) => $"Time elapsed: {ms}ms / {ms / 1000}s";
        public const string LoadingFile = "Loading file...";
        public const string FileLoaded = "File loaded, click RunSync or RunAsync and let the magic happen :)";
        public const string Canceled = "Converting Canceled";
    }
}

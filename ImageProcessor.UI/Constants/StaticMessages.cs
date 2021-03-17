namespace ImageProcessor.UI.Constants
{
    public class StaticMessages
    {
        public static string SelectImage = "Select a image";
        public static string Converting = "Converting...";
        public static string TimeElapsed(long ms) => $"Time elapsed: {ms}ms / {ms / 1000}s";

        public static string LoadingFile = "Loading file...";
        public static string FileLoaded = "File loaded, click RunSync or RunAsync and let the magic happen :)";
    }
}

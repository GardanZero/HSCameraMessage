namespace CameraMessage
{
    public class PluginUserSettings
    {
        public float Textspeed { get; set; }
        public int MessageDelay { get; set; }

        public PluginUserSettings()
        {

        }

        public PluginUserSettings(float textspeed, int messageDelay)
        {
            Textspeed = textspeed;
            MessageDelay = messageDelay;
        }


    }
}

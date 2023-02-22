namespace StepChat.Classes.Configuration
{
    public interface IConfigService
    {
        public string IConfigPath { get; set; }
        public IConfigurationRoot BuildConfiguration();
        public string GetValue(string key);
    }
}

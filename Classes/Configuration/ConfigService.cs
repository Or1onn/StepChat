namespace StepChat.Classes.Configuration
{
    public class ConfigService : IConfigService
    {
        public string? IConfigPath { get; set; }

        public ConfigService(string? iConfigPath)
        {
            IConfigPath = iConfigPath;
        }

        public IConfigurationRoot BuildConfiguration()
        {
            ConfigurationBuilder? builder = new();

            if (IConfigPath!.Contains(".json"))
            {
                builder.AddJsonFile(IConfigPath!);
                var _config = builder.Build();
                return _config;
            }
            throw new ArgumentException("Extension is not .json");

        }

        public string? GetValue(string? key)
        {
            return BuildConfiguration().GetValue(typeof(string), key).ToString();
        }
    }
}

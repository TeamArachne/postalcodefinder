namespace postalcodefinder
{
    using postalcodefinder.Configuration;

    public static class PcfGlobal
    {
        private static IPcfConfigurationSection _config = PcfConfigurationSection.GetSection();

        public static IPcfConfigurationSection Configuration
        {
            get { return _config; }
            internal set { _config = value ?? PcfConfigurationSection.GetSection(); }   // Ensure never null
        }
    }
}
namespace postalcodefinder
{
    using postalcodefinder.Configuration;

    public static class PcfGlobal
    {
        private static IPcfConfigurationSection _config = PcfConfigurationSection.GetSection();

        public static IPcfConfigurationSection Configuration
        {
            get { return _config ?? new PcfConfigurationSection(); }
        }
    }
}

namespace postalcodefinder.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    public class PcfConfigurationSection : ConfigurationSection, IPcfConfigurationSection
    {
        private const string SectionName = "postalcodefinder";

        private const string LoadDataOnAppStartAttributeName = "loadDataOnAppStart";

        [ConfigurationProperty(LoadDataOnAppStartAttributeName, IsRequired = false, DefaultValue = false)]
        bool IPcfConfigurationSection.LoadDataOnAppStart
        {
            get { return (bool)base[LoadDataOnAppStartAttributeName]; }
        }

        IDictionary<string, ConnectionStringSettings> IPcfConfigurationSection.ConnectionStrings
        {
            get
            {
                return ConfigurationManager.ConnectionStrings
                                .OfType<ConnectionStringSettings>()
                                .ToDictionary((p) => p.Name, (p) => p, StringComparer.OrdinalIgnoreCase);
            }
        }

        public static PcfConfigurationSection GetSection()
        {
            PcfConfigurationSection section = ConfigurationManager.GetSection(SectionName) as PcfConfigurationSection;

            if (section == null)
            {
                section = new PcfConfigurationSection();
            }

            return section;
        }
    }
}
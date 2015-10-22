namespace postalcodefinder.Configuration
{
    using System.Collections.Generic;
    using System.Configuration;

    public interface IPcfConfigurationSection
    {
        IDictionary<string, ConnectionStringSettings> ConnectionStrings { get; }

        bool LoadDataOnAppStart { get; }
    }
}

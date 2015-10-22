namespace postalcodefinder.Jobs
{
    using Microsoft.WindowsAzure.Storage.Table;

    public class PostalCodeEntity : TableEntity
    {
        public PostalCodeEntity(string iso2, string postalCode)
        {
            this.PartitionKey = iso2;
            this.RowKey = postalCode;
        }
        public PostalCodeEntity()
        {
        }
        public string iso2 { get; set; }
        public string postalCode { get; set; }
        public string placeName { get; set; }
        public string stateName { get; set; }
        public string stateCode { get; set; }
        public string countyName { get; set; }
        public string countyCode { get; set; }
        public string communityName { get; set; }
        public string communityCode { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public int accuracy { get; set; }
    }
}
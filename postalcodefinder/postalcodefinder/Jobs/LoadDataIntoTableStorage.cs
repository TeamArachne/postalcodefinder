namespace postalcodefinder.Jobs
{
    using log4net;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using System;
    using System.Globalization;
    using Microsoft.VisualBasic.FileIO;
    using System.IO;

    public class LoadDataIntoTableStorage
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(LoadDataIntoTableStorage));

        private readonly string _connectionString;
        private readonly string _appDataFolder;

        public LoadDataIntoTableStorage(string appDataFolder)
            : this(PcfGlobal.Configuration.ConnectionStrings["TableStoragePrimary"].ConnectionString, appDataFolder)
        {
        }

        internal LoadDataIntoTableStorage(string connectionString, string appDataFolder)
        {
            this._connectionString = connectionString;
            this._appDataFolder = appDataFolder;
        }

        public void Execute()
        {
            try
            {
                // Retrieve the storage account from the connection string.
                CloudStorageAccount account;
                if (CloudStorageAccount.TryParse(_connectionString, out account))
                {
                    // Create the table client.
                    CloudTableClient client = account.CreateCloudTableClient();

                    // Create the table if it doesn't exist.                  
                    CloudTable table = client.GetTableReference("postalcodegb");
                    table.CreateIfNotExists();
                    
                    // TO DO : relative path? ~ didnt work :(
                    using (var reader = new TextFieldParser(Path.Combine(_appDataFolder, "GB.txt")))
                    {

                        reader.TextFieldType = FieldType.Delimited;
                        reader.Delimiters = new string[] { "\t" };

                        string[] currentRow;

                        TableBatchOperation batchOp = new TableBatchOperation();

                        while (!reader.EndOfData)
                        {
                            while (batchOp.Count < 100)
                            {

                                currentRow = reader.ReadFields();

                                var postalCodeEntity = new PostalCodeEntity()
                                {
                                    iso2 = currentRow[0],
                                    postalCode = currentRow[1],
                                    placeName = currentRow[2],
                                    stateName = currentRow[3],
                                    stateCode = currentRow[4],
                                    countyName = currentRow[5],
                                    countyCode = currentRow[6],
                                    communityName = currentRow[7],
                                    communityCode = currentRow[8],
                                    latitude = string.IsNullOrEmpty(currentRow[9]) ? (float)0 : float.Parse(currentRow[9], CultureInfo.InvariantCulture.NumberFormat),
                                    longitude = string.IsNullOrEmpty(currentRow[10]) ? (float)0 : float.Parse(currentRow[10], CultureInfo.InvariantCulture.NumberFormat),
                                    accuracy = string.IsNullOrEmpty(currentRow[11]) ? (int)0 : int.Parse(currentRow[11], CultureInfo.InvariantCulture.NumberFormat)
                                };

                                batchOp.InsertOrReplace(postalCodeEntity);
                            }
                          
							table.ExecuteBatch(batchOp);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Error(string.Format(CultureInfo.InvariantCulture, "Failed to load data into Azure Table Storage: {0}", ex.Message), ex);
            }
        }
    }
}
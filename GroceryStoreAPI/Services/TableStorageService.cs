using GroceryStoreAPI.Models;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace GroceryStoreAPI.Services
{
    public class TableStorageService : ITableStorageService
    {
        private const string TableName = "Item";
        private readonly IConfiguration _configuration;

        public TableStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Public Methods

        public async Task<GroceryItemEntity> RetrieveAsync(string category, string id)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<GroceryItemEntity>(category, id);
            return await ExecuteTableOperation(retrieveOperation) as GroceryItemEntity;
        }
        public async Task<GroceryItemEntity> InsertOrMergeAsync(GroceryItemEntity entity)
        {
            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);
            return await ExecuteTableOperation(insertOrMergeOperation) as GroceryItemEntity;
        }
        public async Task<GroceryItemEntity> DeleteAsync(GroceryItemEntity entity)
        {
            TableOperation deleteOperation = TableOperation.Delete(entity);
            return await ExecuteTableOperation(deleteOperation) as GroceryItemEntity;
        } 

        #endregion

        #region Private Methods

        private async Task<object> ExecuteTableOperation(TableOperation retrieveOperation)
        {
            CloudTable table = await GetCloudTable();
            TableResult tableResult = await table.ExecuteAsync(retrieveOperation);
            return tableResult.Result;
        }

        private async Task<CloudTable> GetCloudTable()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_configuration["StorageConnectionString"]);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            CloudTable table = tableClient.GetTableReference(TableName);
            await table.CreateIfNotExistsAsync();
            return table;
        }

        #endregion
    }
}

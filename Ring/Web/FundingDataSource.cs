//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using Microsoft.WindowsAzure;

//namespace OTP.Ring.Web
//{
//    public class FundingDataSource
//    {
//        private FundingDataServiceContext _ServiceContext = null;

//        public FundingDataSource()
//        {
//            var storageAccount = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
//            _ServiceContext = new FundingDataServiceContext(storageAccount.TableEndpoint.ToString(), storageAccount.Credentials);

//            // Create the tables
//            // In this case, just a single table.  
//            storageAccount.CreateCloudTableClient().CreateTableIfNotExist(FundingDataServiceContext.FundingTableName);
//        }

//        public IEnumerable<FundingDataModel> Select()
//        {
//            var results = from c in _ServiceContext.FundingTable
//                          select c;

//            var query = results.AsTableServiceQuery<FundingDataModell>();
//            var queryResults = query.Execute();

//            return queryResults;
//        }

//        public void Delete(FundingDataModel itemToDelete)
//        {
//            _ServiceContext.AttachTo(FundingDataServiceContext.FundingTableName, itemToDelete, "*");
//            _ServiceContext.DeleteObject(itemToDelete);
//            _ServiceContext.SaveChanges();
//        }

//        public void Insert(FundingDataModel newItem)
//        {
//            _ServiceContext.AddObject(FundingDataServiceContext.FundingTableName, newItem);
//            _ServiceContext.SaveChanges();
//        }
//    }
//}
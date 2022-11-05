using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace OTP.Ring.Web
{
	public class FundingDataServiceContext : TableServiceContext
	{
		public FundingDataServiceContext(string baseAddress, StorageCredentials credentials)
			: base(baseAddress, credentials)
		{
		}

		public const string FundingTableName = "FundingTable";

		public IQueryable<FundingDataModel> FundingTable
		{
			get
			{
				return this.CreateQuery<FundingDataModel>(FundingTableName);
			}
		}
	}
}
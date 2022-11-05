using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.StorageClient;

namespace OTP.Ring.Web
{
	public class FundingDataModel : TableServiceEntity
	{
		public FundingDataModel(string partitionKey, string rowKey)
			: base(partitionKey, rowKey)
		{
				
		}

		public FundingDataModel()
			: this(Guid.NewGuid().ToString(), string.Empty)
		{
		}

		public DateTime TimeStamp { get; set; }

		
	}
}
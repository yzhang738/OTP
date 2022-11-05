using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using OTP.Ring.Data.Models;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using OTP.Ring.Common;

namespace OTP.Ring.Models.ViewModel
{
    [KnownType(typeof(ResultsLoadResultsErrorGridViewModel))]
    [Serializable]
    public class ResultsLoadResultsErrorGridViewModel
    {
        public static List<ResultsLoadResultsErrorGridViewModel> GetListFromErrorResults(List<uspLoadDownloadStagingResults_Result> errorList)
        {
            var errors = errorList.ToList();
            var results = (from e in errors
                           orderby e.SourceRow, e.SourceColumn
                           select new ResultsLoadResultsErrorGridViewModel
                            {
                                DataType = e.DataType,
                                DownloadValue = e.DownloadValue,
                                ErrorMessage = e.ErrorMessage,
                                FieldLength = e.FieldLength,
                                SourceColumn = e.SourceColumn,
                                SourceColumnName = e.SourceColumnName,
                                SourceRow = e.SourceRow
                            }).ToList();

            return results;
        }

        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_DataType")]
        public string DataType { get; set; }

        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_DownloadValue")]
        public string DownloadValue { get; set; }

        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_ErrorMessage")]
        public string ErrorMessage { get; set; }

        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_FieldLength")]
        public int? FieldLength { get; set; }

        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_SourceColumn")]
        public string SourceColumn { get; set; }

        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_SourceColumnName")]
        public string SourceColumnName { get; set; }

        [LocalizedDisplayName(typeof(ResultsLocalization), "Title_SourceRow")]
        public int? SourceRow { get; set; }
    }
}
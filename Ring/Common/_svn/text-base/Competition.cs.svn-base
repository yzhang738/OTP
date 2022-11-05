using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace OTP.Ring.Common
{
    [Serializable]
    public class Competition
    {
        public class Comparer : IComparer<Competition>
        {
            public int Compare(Competition x, Competition y)
            {
                if (x.Date == y.Date)
                {
                    return x.SportDesc.CompareTo(y.SportDesc);
                }
                else
                {
                    return x.Date.CompareTo(y.Date);
                }
            }
        }

        [LocalizedDisplayName(typeof(CommonLocalization), "Competition_Title_Id")]
        public int Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(CommonLocalization), ErrorMessageResourceName = "Competition_Required_SportId")]
        [UIHint("SummarySportTemplate")]
        [LocalizedDisplayName(typeof(CommonLocalization), "Competition_Title_SportId")]
        public string SportId { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Competition_Title_SportDesc")]
        public string SportDesc { get; set; }

        [Required(ErrorMessageResourceType = typeof(CommonLocalization), ErrorMessageResourceName = "Competition_Required_Type")]
        [UIHint("SummaryTypeTemplate")]
        [LocalizedDisplayName(typeof(CommonLocalization), "Competition_Title_Type")]
        public string Type { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Competition_Title_TypeDesc")]
        public string TypeDesc { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Competition_Title_YearId")]
        public string YearId { get; set; }

        [Required(ErrorMessageResourceType = typeof(CommonLocalization), ErrorMessageResourceName = "Competition_Required_Name")]
        [LocalizedDisplayName(typeof(CommonLocalization), "Competition_Title_Name")]
        public string Name { get; set; }

        private DateTime _date;

        [Required(ErrorMessageResourceType = typeof(CommonLocalization), ErrorMessageResourceName = "Competition_Required_Date")]
        [DataType(DataType.Date)]
        [LocalizedDisplayName(typeof(CommonLocalization), "Competition_Title_Date")]
        public DateTime Date
        {
            get
            {
                return DateTime.SpecifyKind(new DateTime(_date.Year, _date.Month, _date.Day), DateTimeKind.Utc);
            }
            set
            {
                _date = DateTime.SpecifyKind(new DateTime(value.Year, value.Month, value.Day), DateTimeKind.Utc);
            }
        }

        public string DateLiteral
        {
            get
            {
                return Date.ToShortDateString();
            }
        }

        [LocalizedDisplayName(typeof(CommonLocalization), "Competition_Title_Location")]
        public string Location { get; set; }

        [UIHint("SummaryCountryTemplate")]
        [LocalizedDisplayName(typeof(CommonLocalization), "Competition_Title_CountryCode")]
        public string CountryCode { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Competition_Title_CountryDesc")]
        public string CountryDesc { get; set; }

        [Required(ErrorMessageResourceType = typeof(CommonLocalization), ErrorMessageResourceName = "Competition_Required_BenchmarkCode")]
        [UIHint("SummaryBenchmarkTemplate")]
        [LocalizedDisplayName(typeof(CommonLocalization), "Competition_Title_BenchmarkCode")]
        public int BenchmarkCode { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Competition_Title_BenchmarkDesc")]
        public string BenchmarkDesc { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Competition_Title_Comment")]
        public string Comment { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Competition_Title_DownloadCount")]
        public int DownloadCount { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Competition_Title_ApproveCount")]
        public int ApproveCount { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(CommonLocalization), "Competition_Title_Ratio")]
        public string Ratio
        {
            get
            {
                return string.Format("{0}/{1}", DownloadCount, ApproveCount);
            }
        }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(CommonLocalization), "Competition_Title_DownloadStatus")]
        public string DownloadStatus { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(CommonLocalization), "Competition_Title_ApproveStatus")]
        public string ApproveStatus { get; set; }

        [ReadOnly(true)]
        [LocalizedDisplayName(typeof(CommonLocalization), "Competition_Title_Editable")]
        public bool Editable { get; set; }

        [LocalizedDisplayName(typeof(CommonLocalization), "Competition_Title_ShowApproveConfirmation")]
        public bool ShowApproveConfirmation
        {
            get
            {
                return ApproveStatus == Constant.CompetitionStatus.Posted;
            }
        }

        [LocalizedDisplayName(typeof(CommonLocalization), "Competition_Title_ShowApproveStatus")]
        public bool ShowApproveStatus
        {
            get
            {
                return ApproveStatus != Constant.CompetitionStatus.NotStarted && ApproveStatus != Constant.CompetitionStatus.NoEvents;
            }
        }

        [LocalizedDisplayName(typeof(CommonLocalization), "Competition_Title_ShowDownloadStatus")]
        public bool ShowDownloadStatus
        {
            get
            {
                return Editable && DownloadStatus != Constant.CompetitionStatus.NotStarted && DownloadStatus != Constant.CompetitionStatus.NoEvents;
            }
        }
    }
}

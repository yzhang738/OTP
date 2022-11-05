using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.Extensions;
using OTP.Ring.Models;
using OTP.Ring.Models.ViewModel;
using OTP.Ring.Common;
using OTP.Ring.Business;

namespace OTP.Ring.Web.Controllers
{
    public class ExportExcelButtonController : BaseController
    {
        private IResourceBL _resourceBL;
        private IBenchmarkBL _benchmarkBL;
        private IDecodeBL _decodeBL;
        private IUserBL _userBL;
        private ISportBL _sportBL;

        public ExportExcelButtonController(IResourceBL resourceBL, IBenchmarkBL benchmarkBL, IDecodeBL decodeBL, IUserBL userBL, ISportBL sportBL)
            : base(decodeBL, userBL, sportBL)
        {
            this._resourceBL = resourceBL;
            this._benchmarkBL = benchmarkBL;
            this._decodeBL = decodeBL;
            this._userBL = userBL;
            this._sportBL = sportBL;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ExportAthletePageToExcel(string page, string pageSize, string orderBy, string filter, string selectedSportId, string yearId)
        {
            string filterName = string.Empty;

            GridCommand command = GridCommand.Parse(Convert.ToInt32(page), Convert.ToInt32(pageSize), orderBy, "", filter);

            // Get the data that is exported to excel file
            var query = this._resourceBL.GetMaintenanceAthletesAthleteGridViewModelParameter(selectedSportId, yearId, command);

            // Create exported excel file
            MemoryStream output = this._decodeBL.CreateExcelFile<MaintenanceAthletesAthleteGridViewModel>(query);

            string sportName = this._sportBL.GetSportNameBySportID(selectedSportId);
            if (!string.IsNullOrEmpty(filter))
                filterName = " (" + filter + ")";

            // Create export file name based on sport name
            string exportFileName = Constant.ExportExcelFileName.AthletesPageExport + " (" + sportName + filterName + ")";

            if (exportFileName.Length > Constant.MaxExportExcelFileNameLength)
            {
                exportFileName = exportFileName.Substring(0, Constant.MaxExportExcelFileNameLength) + Constant.ExportExcelFileName.ExportExtension;
            }
            else
            {
                exportFileName += Constant.ExportExcelFileName.ExportExtension;
            }

            // Return the result to the end user
            return File(output.ToArray(),           // The binary data of the XLS file
                "application/vnd.ms-excel",         // MIME type of Excel files
                exportFileName);                    // Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        public ActionResult ExportAthleteResourceSportToExcel(string resourceId, string yearId, string page, string pageSize, string orderBy, string filter)
        {
            string filterName = string.Empty;

            GridCommand command = GridCommand.Parse(Convert.ToInt32(page), Convert.ToInt32(pageSize), orderBy, "", filter);
            int resourceID = Convert.ToInt32(resourceId);

            // Get the data that is exported to excel file
            var query = this._resourceBL.GetMaintenanceAthletesSportsInfoViewModelParameter(resourceID, yearId, command);

            // Create exported excel file
            MemoryStream output = this._decodeBL.CreateExcelFile<MaintenanceAthletesSportsInfoViewModel>(query);

            string name = this._resourceBL.GetResourceNameByResourceId(resourceID);
            if (!string.IsNullOrEmpty(filter))
                filterName = " (" + filter + ")";

            // Create export file name based on sport name
            string exportFileName = Constant.ExportExcelFileName.AthletesResourceSportExport + " (" + name + filterName + ")";

            if (exportFileName.Length > Constant.MaxExportExcelFileNameLength)
            {
                exportFileName = exportFileName.Substring(0, Constant.MaxExportExcelFileNameLength) + Constant.ExportExcelFileName.ExportExtension;
            }
            else
            {
                exportFileName += Constant.ExportExcelFileName.ExportExtension;
            }

            // Return the result to the end user
            return File(output.ToArray(),           // The binary data of the XLS file
                "application/vnd.ms-excel",         // MIME type of Excel files
                exportFileName);                    // Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        public ActionResult ExportAthleteDirectAthleteSupportToExcel(string resourceId, string yearId, string page, string pageSize, string orderBy, string filter)
        {
            string filterName = string.Empty;

            GridCommand command = GridCommand.Parse(Convert.ToInt32(page), Convert.ToInt32(pageSize), orderBy, "", filter);
            int resourceID = Convert.ToInt32(resourceId);

            // Get the data that is exported to excel file
            var query = this._resourceBL.GetMaintenanceAthletesDirectAthleteSupportViewModelParameter(resourceID, command);

            // Create exported excel file
            MemoryStream output = this._decodeBL.CreateExcelFile<MaintenanceAthletesDirectAthleteSupportViewModel>(query);

            string name = this._resourceBL.GetResourceNameByResourceId(resourceID);
            if (!string.IsNullOrEmpty(filter))
                filterName = " (" + filter + ")";

            // Create export file name based on sport name
            string exportFileName = Constant.ExportExcelFileName.AthletesDirectAthleteSupportExport + " (" + name + filterName + ")";

            if (exportFileName.Length > Constant.MaxExportExcelFileNameLength)
            {
                exportFileName = exportFileName.Substring(0, Constant.MaxExportExcelFileNameLength) + Constant.ExportExcelFileName.ExportExtension;
            }
            else
            {
                exportFileName += Constant.ExportExcelFileName.ExportExtension;
            }

            // Return the result to the end user
            return File(output.ToArray(),           // The binary data of the XLS file
                "application/vnd.ms-excel",         // MIME type of Excel files
                exportFileName);                    // Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        public ActionResult ExportAthleteAffiliatesInfoToExcel(string resourceId, string yearId, string page, string pageSize, string orderBy, string filter)
        {
            string filterName = string.Empty;

            GridCommand command = GridCommand.Parse(Convert.ToInt32(page), Convert.ToInt32(pageSize), orderBy, "", filter);
            int resourceID = Convert.ToInt32(resourceId);

            // Get the data that is exported to excel file
            var query = this._resourceBL.GetMaintenanceAthletesAffiliatesInfoViewModelParameter(resourceID, command);

            // Create exported excel file
            MemoryStream output = this._decodeBL.CreateExcelFile<MaintenanceAthletesAffiliatesInfoViewModel>(query);

            string name = this._resourceBL.GetResourceNameByResourceId(resourceID);
            if (!string.IsNullOrEmpty(filter))
                filterName = " (" + filter + ")";

            // Create export file name based on sport name
            string exportFileName = Constant.ExportExcelFileName.AthletesAffiliatesInfoExport + " (" + name + filterName + ")";

            if (exportFileName.Length > Constant.MaxExportExcelFileNameLength)
            {
                exportFileName = exportFileName.Substring(0, Constant.MaxExportExcelFileNameLength) + Constant.ExportExcelFileName.ExportExtension;
            }
            else
            {
                exportFileName += Constant.ExportExcelFileName.ExportExtension;
            }

            // Return the result to the end user
            return File(output.ToArray(),           // The binary data of the XLS file
                "application/vnd.ms-excel",         // MIME type of Excel files
                exportFileName);                    // Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        public ActionResult ExportAthleteResultsToExcel(string resourceId, string page, string pageSize, string orderBy, string filter)
        {
            string filterName = string.Empty;

            GridCommand command = GridCommand.Parse(Convert.ToInt32(page), Convert.ToInt32(pageSize), orderBy, "", filter);
            int resourceID = Convert.ToInt32(resourceId);

            // Get the data that is exported to excel file
            var query = this._resourceBL.GetMaintenanceAthletesResultsViewModelParameter(resourceID, command);

            // Create exported excel file
            MemoryStream output = this._decodeBL.CreateExcelFile<MaintenanceAthletesResultsViewModel>(query);

            string name = this._resourceBL.GetResourceNameByResourceId(resourceID);
            if (!string.IsNullOrEmpty(filter))
                filterName = " (" + filter + ")";

            // Create export file name based on sport name
            //string exportFileName = Constant.ExportExcelFileName.AthletesResultsExport + " (" + name + filterName + ")" + Constant.ExportExcelFileName.ExportExtension;
            string exportFileName = Constant.ExportExcelFileName.AthletesResultsExport + " (" + name + filterName + ")";

            if (exportFileName.Length > Constant.MaxExportExcelFileNameLength)
            {
                exportFileName = exportFileName.Substring(0, Constant.MaxExportExcelFileNameLength) + Constant.ExportExcelFileName.ExportExtension;
            }
            else
            {
                exportFileName += Constant.ExportExcelFileName.ExportExtension;
            }

            // Return the result to the end user
            return File(output.ToArray(),           // The binary data of the XLS file
                "application/vnd.ms-excel",         // MIME type of Excel files
                exportFileName);                    // Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        public ActionResult ExportISTMembersPageToExcel(string page, string pageSize, string orderBy, string filter, string selectedSportId)
        {
            string filterName = string.Empty;

            GridCommand command = GridCommand.Parse(Convert.ToInt32(page), Convert.ToInt32(pageSize), orderBy, "", filter);

            // Get the data that is exported to excel file
            var query = this._resourceBL.GetMaintenanceISTMembersGridViewModelParameter(selectedSportId, command);

            // Create exported excel file
            MemoryStream output = this._decodeBL.CreateExcelFile<MaintenanceISTMembersGridViewModel>(query);

            string sportName = this._sportBL.GetSportNameBySportID(selectedSportId);
            if (!string.IsNullOrEmpty(filter))
                filterName = " (" + filter + ")";

            // Create export file name based on sport name
            string exportFileName = Constant.ExportExcelFileName.ISTMembersPageExport + " (" + sportName + filterName + ")";

            if (exportFileName.Length > Constant.MaxExportExcelFileNameLength)
            {
                exportFileName = exportFileName.Substring(0, Constant.MaxExportExcelFileNameLength) + Constant.ExportExcelFileName.ExportExtension;
            }
            else
            {
                exportFileName += Constant.ExportExcelFileName.ExportExtension;
            }

            // Return the result to the end user
            return File(output.ToArray(),           // The binary data of the XLS file
                "application/vnd.ms-excel",         // MIME type of Excel files
                exportFileName);                    // Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        public ActionResult ExportISTMembersSportInfoToExcel(string resourceId, string page, string pageSize, string orderBy, string filter)
        {
            string filterName = string.Empty;

            GridCommand command = GridCommand.Parse(Convert.ToInt32(page), Convert.ToInt32(pageSize), orderBy, "", filter);
            int resourceID = Convert.ToInt32(resourceId);

            // Get the data that is exported to excel file
            var query = this._resourceBL.GetMaintenanceISTMembersSportsInfoViewModelParameter(resourceID, command);

            // Create exported excel file
            MemoryStream output = this._decodeBL.CreateExcelFile<MaintenanceISTMembersSportsInfoViewModel>(query);

            string name = this._resourceBL.GetResourceNameByResourceId(resourceID);
            if (!string.IsNullOrEmpty(filter))
                filterName = " (" + filter + ")";

            // Create export file name based on sport name
            string exportFileName = Constant.ExportExcelFileName.ISTMembersSportInfoExport + " (" + name + filterName + ")";

            if (exportFileName.Length > Constant.MaxExportExcelFileNameLength)
            {
                exportFileName = exportFileName.Substring(0, Constant.MaxExportExcelFileNameLength) + Constant.ExportExcelFileName.ExportExtension;
            }
            else
            {
                exportFileName += Constant.ExportExcelFileName.ExportExtension;
            }

            // Return the result to the end user
            return File(output.ToArray(),           // The binary data of the XLS file
                "application/vnd.ms-excel",         // MIME type of Excel files
                exportFileName);                    // Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        public ActionResult ExportNSOMembersPageToExcel(string page, string pageSize, string orderBy, string filter, string selectedSportId)
        {
            string filterName = string.Empty;

            GridCommand command = GridCommand.Parse(Convert.ToInt32(page), Convert.ToInt32(pageSize), orderBy, "", filter);

            // Get the data that is exported to excel file
            var query = this._resourceBL.GetMaintenanceNSOMembersGridViewModelParameter(selectedSportId, command);

            // Create exported excel file
            MemoryStream output = this._decodeBL.CreateExcelFile<MaintenanceNSOMembersGridViewModel>(query);

            string sportName = this._sportBL.GetSportNameBySportID(selectedSportId);
            if (!string.IsNullOrEmpty(filter))
                filterName = " (" + filter + ")";

            // Create export file name based on sport name
            string exportFileName = Constant.ExportExcelFileName.NSOMembersPageExport + " (" + sportName + filterName + ")";

            if (exportFileName.Length > Constant.MaxExportExcelFileNameLength)
            {
                exportFileName = exportFileName.Substring(0, Constant.MaxExportExcelFileNameLength) + Constant.ExportExcelFileName.ExportExtension;
            }
            else
            {
                exportFileName += Constant.ExportExcelFileName.ExportExtension;
            }

            // Return the result to the end user
            return File(output.ToArray(),           // The binary data of the XLS file
                "application/vnd.ms-excel",         // MIME type of Excel files
                exportFileName);                    // Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        public ActionResult ExportNSOMembersSportInfoToExcel(string resourceId, string page, string pageSize, string orderBy, string filter)
        {
            string filterName = string.Empty;

            GridCommand command = GridCommand.Parse(Convert.ToInt32(page), Convert.ToInt32(pageSize), orderBy, "", filter);
            int resourceID = Convert.ToInt32(resourceId);

            // Get the data that is exported to excel file
            var query = this._resourceBL.GetMaintenanceNSOMembersSportsInfoViewModelParameter(resourceID, command);

            // Create exported excel file
            MemoryStream output = this._decodeBL.CreateExcelFile<MaintenanceNSOMembersSportsInfoViewModel>(query);

            string name = this._resourceBL.GetResourceNameByResourceId(resourceID);
            if (!string.IsNullOrEmpty(filter))
                filterName = " (" + filter + ")";

            // Create export file name based on sport name
            string exportFileName = Constant.ExportExcelFileName.NSOMembersSportInfoExport + " (" + name + filterName + ")";

            if (exportFileName.Length > Constant.MaxExportExcelFileNameLength)
            {
                exportFileName = exportFileName.Substring(0, Constant.MaxExportExcelFileNameLength) + Constant.ExportExcelFileName.ExportExtension;
            }
            else
            {
                exportFileName += Constant.ExportExcelFileName.ExportExtension;
            }

            // Return the result to the end user
            return File(output.ToArray(),           // The binary data of the XLS file
                "application/vnd.ms-excel",         // MIME type of Excel files
                exportFileName);                    // Suggested file name in the "Save as" dialog which will be displayed to the end user
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OTP.Ring;
using OTP.Ring.Common;
using OTP.Ring.Models;

namespace OTP.Ring.Business
{
    public class ActionItemBL : IActionItemBL
    {
        private bool _frenchLanguage = false;
        private Data.Models.RingEntities _repository;
        private IDecodeBL _decodeBL;
        private ISportBL _sportBL;

        public ActionItemBL(string connectionString, IDecodeBL decodeBL, ISportBL sportBL)
        {
            _frenchLanguage = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToUpper() == Constant.LanguageCode.French;
            _repository = new Data.Models.RingEntities(connectionString);
            this._decodeBL = decodeBL;
            this._sportBL = sportBL;
        }

        #region PUBLIC

        public List<ActionItem> GetActionItems(string selectedStatusGrouping, string referenceId, string source, string type, int userId, IList<ListItem> selectedSports)
        {
            List<ActionItem> actionItems = new List<ActionItem>();

            List<Data.Models.ActionItem> dataActionItems = new List<Data.Models.ActionItem>();

            this._repository.ActionItems.MergeOption = System.Data.Objects.MergeOption.OverwriteChanges;

            if (string.IsNullOrEmpty(referenceId))
            {
                //links from nav menu, Funding Action Item item
                dataActionItems = (from a in this._repository.ActionItems
                                   join t in this._repository.ActionItemTypes on a.ActionItemTypeID equals t.ActionItemTypeID
                                   where t.SourceName_EN == source
                                   select a).ToList();
            }
            else if (!string.IsNullOrEmpty(type))
            {
                //links from three funding pages
                dataActionItems = (from a in this._repository.ActionItems
                                   where a.ActionItemTypeID == type && a.ReferenceID == referenceId
                                   select a).ToList();
            }
            else if (!string.IsNullOrEmpty(source)) 
            {
                //links from Funding Summary screen
                dataActionItems = (from a in this._repository.ActionItems
                                   join t in this._repository.ActionItemTypes on a.ActionItemTypeID equals t.ActionItemTypeID
                                   where t.SourceName_EN == source && a.ReferenceID == referenceId 
                                   select a).ToList();
            }

            //apply status grouping
            dataActionItems = (from a in dataActionItems
                                where (selectedStatusGrouping == "O" && (a.ActionItemStatus == Constant.ActionItemStatus.New || a.ActionItemStatus == Constant.ActionItemStatus.InProgress))
                                   || (selectedStatusGrouping == "C" && !(a.ActionItemStatus == Constant.ActionItemStatus.New || a.ActionItemStatus == Constant.ActionItemStatus.InProgress))
                                orderby a.DueDate
                                select a).ToList();

            //apply filtered sports
            if (selectedSports != null)
            { 
                IEnumerable<string> selectedSportIds = (from a in selectedSports select a.Id);
                dataActionItems = (from a in dataActionItems where selectedSportIds.Contains(a.SportID) select a).ToList();
            }

            actionItems = ConvertActionItems(dataActionItems);

            actionItems = PopulateCodeDescription(actionItems);

            return actionItems;
        }

        public IEnumerable<Decode> GetActionItemTypes()
        {
            return this._decodeBL.GetActionItemTypes();
        }

        public int UpdateActionItem(ActionItem postedActionItem)
        { 
            int result = 0;

            var dataActionItems = from d in this._repository.ActionItems
                            where d.ActionItemID == postedActionItem.Id
                            select d;

            if (dataActionItems != null && dataActionItems.Count() == 1)
            {
                Data.Models.ActionItem dataActionItem = dataActionItems.First();

                var output = this._repository.uspUpdateActionItem(dataActionItem.ActionItemID,
                    dataActionItem.ActionItemTypeID,
                    dataActionItem.ReferenceID,
                    dataActionItem.OrganizationID,
                    dataActionItem.SportID,
                    postedActionItem.Description,
                    postedActionItem.DueDate,
                    postedActionItem.Status,
                    postedActionItem.Priority,
                    dataActionItem.Comments).SingleOrDefault();

                result = output == null ? result : (int)output;
            }

            return result;
        }

        public int CreateActionItem(ActionItem postedActionItem)
        {
            int result = 0;

            var output = this._repository.uspAddActionItem(postedActionItem.TypeId,
                postedActionItem.ReferenceId,
                postedActionItem.OrganizationId,
                postedActionItem.SportId,
                postedActionItem.Description,
                postedActionItem.DueDate,
                postedActionItem.Status,
                postedActionItem.Priority,
                postedActionItem.Comment).SingleOrDefault();

                result = output == null ? result : (int)output;

            return result;
        }

        #endregion

        #region PRIVATE HELPERS

        private List<ActionItem> PopulateCodeDescription(List<ActionItem> actionItems)
        {
            if (actionItems == null) return null;

            IEnumerable<Decode> actionItemTypes = this._decodeBL.GetActionItemTypes();
            IEnumerable<Decode> statusTypes = this._decodeBL.GetDecodesByCategory(Constant.DecodeCategoryId.ActionItemStatus);
            IEnumerable<Decode> priorityTypes = this._decodeBL.GetDecodesByCategory(Constant.DecodeCategoryId.ActionItemPriority);
            IEnumerable<Organization> orgs = this._decodeBL.GetAllOrganizations();
            IEnumerable<ListItem> sports = this._sportBL.GetAllSportsOnSportLevel();

            foreach (var item in actionItems)
            {
                item.TypeDesc = (from t in actionItemTypes where t.Id == item.TypeId select t.Name).SingleOrDefault();
                item.StatusDesc = (from t in statusTypes where t.Id == item.Status select t.Name).SingleOrDefault();
                item.PriorityDesc = (from t in priorityTypes where t.Id == item.Priority select t.Name).SingleOrDefault();

                item.OrganizationDesc = (from o in orgs where o.Id == item.OrganizationId select o.Name).SingleOrDefault();
                item.SportDesc = (from s in sports where s.Id == item.SportId select s.Description).SingleOrDefault();
            }

            return actionItems;
        }

        private List<ActionItem> ConvertActionItems(List<Data.Models.ActionItem> dataActionItems)
        {
            return (from a in dataActionItems
                       select new ActionItem
                                {
                                    Id = a.ActionItemID,
                                    TypeId = a.ActionItemTypeID,
                                    ReferenceId = a.ReferenceID,
                                    OrganizationId = a.OrganizationID,
                                    SportId = a.SportID,
                                    Description = _frenchLanguage ? a.Description : a.Description,
                                    DueDate = a.DueDate,
                                    Status = a.ActionItemStatus,
                                    Priority = a.ActionItemPriority,
                                    Comment = a.Comments,
                                    HasComment = !string.IsNullOrEmpty(a.Comments),
                                    UserId = 9999,
                                    ControllerName = GetControllerName(a.ActionItemTypeID),
                                    ActionName = GetActionName(a.ActionItemTypeID)
                                }).ToList();
        }

        private string GetActionName(string actionItemType)
        {
            if (actionItemType == Constant.ActionItemType.FundingRequest)
                return Constant.FundingAction.Request;

            if (actionItemType == Constant.ActionItemType.FundingRecommendation)
                return Constant.FundingAction.Recommend;

            if (actionItemType == Constant.ActionItemType.FundingRevision)
                return Constant.FundingAction.Revise;

            return "EvaluationForm";
        }

        private string GetControllerName(string actionItemType)
        {
            if (actionItemType == Constant.ActionItemType.FundingRequest
                || actionItemType == Constant.ActionItemType.FundingRecommendation
                || actionItemType == Constant.ActionItemType.FundingRevision)
            {
                return "Funding";
            }

            return "Evaluations";
        }
        #endregion
    }
}

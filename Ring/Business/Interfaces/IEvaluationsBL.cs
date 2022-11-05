using System;
using System.Collections.Generic;
using OTP.Ring.Models.ViewModel;
using OTP.Ring.Common;
namespace OTP.Ring.Business
{
    public interface IEvaluationsBL
    {
        List<EvaluationsQuestionsEvaluationVersionGridViewModel> GetEvaluationsQuestionsEvaluationVersionGridViewModel(string yearId);
        string IfEvaluationVersionExist(string organizationTypeId, string yearId, string versionId);        
        int AddEvaluationVersion(
            string organizationTypeId,
            string yearId,
            string versionId,
            string versionPhase,
            string name_EN,
            string name_FR,
            string comments);
        int UpdateEvaluationVersion(
            int evaluationVersionId,
            string organizationTypeId = Constant.UndefinedString,
            string yearId = Constant.UndefinedString,
            string versionId = Constant.UndefinedString,
            string versionPhase = Constant.UndefinedString,
            string name_EN = Constant.UndefinedString,
            string name_FR = Constant.UndefinedString,
            string comments = Constant.UndefinedString);
        void PopulateEvaluationsQuestionsViewData(System.Web.Mvc.ViewDataDictionary viewData, string yearId);
        int GetFirstEvaluationVersionId(string yearId);
        List<EvaluationsQuestionsEvaluationPillarsGridViewModel> GetEvaluationsQuestionsEvaluationPillarsGridViewModel(int evaluationVersionId, bool showDeleted);
        List<EvaluationsQuestionsEvaluationPillarQuestionsGridViewModel> GetEvaluationsQuestionsEvaluationPillarQuestionsGridViewModel(int evaluationVersionId, string evaluationMasterPillarId, bool showDeleted);
        int UpdateEvaluationVersionQuestion(
            int evaluationVersionQuestionId,
            string versionQuestion = Constant.UndefinedString,
            string questionPhase = Constant.UndefinedString,
            int? sortOrder = null,
            string comments = Constant.UndefinedString,
            string sportId = Constant.UndefinedString);
        int AddEvaluationVersionQuestion(
            int evaluationVersionId,
            string evaluationMasterPillarId,
            string versionQuestion,
            string questionPhase,
            int sortOrder,
            string comments,
            string sportId);
        Common.Evaluation GetEvaluation(int id);
        IEnumerable<Common.EvaluationPillar> GetEvaluationQuestions(int id);
        string GetActionItemRatioForEvaluationPillar(int evaluationId, string pillarId);
        List<OTP.Ring.Models.ListItem> GetCopyToYearsList(int evaluationVersionId);
        IEnumerable<Common.EvaluationBestPractice> GetEvaluationBestPractices(string pillarId);
        Common.EvaluationBestPractice GetEvaluationBestPractice(int bestPracticeId);
        int SaveEvaluationQuestion(int evaluationId, string pillarId, Common.EvaluationQuestion question);
        int UpdateEvaluationPhase(int id, string fromPhase, string toPhase);
        int InsertEvaluationBestPractice(Common.EvaluationBestPractice bestPractice);
        int UpdateEvaluationBestPractice(Common.EvaluationBestPractice bestPractice);
        int DeleteEvaluationBestPractice(int bestPracticeId);
        int CopyEvaluationVersion(int evaluationVersionId, string copyToYearId);
        EvaluationsSummaryViewModel PopulateEvaluationsSummaryViewModel(string yearId, int? evaluationId, IList<Models.ListItem> selectedSports);
        int AddEvaluation(
            int evaluationVersionId,
            string sportId,
            string evaluationPhase,
            string comments);
        List<OTP.Ring.Models.ListItem> GetAddEvaluationSportList(int evaluationVersionId);
        List<OTP.Ring.Models.ListItem> GetVersionPhasesList(string currentVersionPhase);
        List<OTP.Ring.Models.ListItem> GetVersionIdList();
        List<OTP.Ring.Models.ListItem> GetVersionNameENList();
        List<OTP.Ring.Models.ListItem> GetVersionNameFRList();
        string GetVersionID(string versionName);
        string GetVersionNameEn(string versionID);
        string GetVersionNameFr(string versionID);
        List<OTP.Ring.Models.ListItem> GetQuestionPhasesList(string currentQuestionPhase);
        int DeleteEvaluationVersionQuestion(int evaluationVersionQuestionId);
        int CopyEvaluation(
            int evaluationId,
            int copyToEvaluationVersionId);
        bool CanCopyEvaluation(int evaluationId, int copyToEvaluationVersionId, System.Web.Mvc.ModelStateDictionary modelState);
        List<OTP.Ring.Models.ListItem> GetCopyToEvaluationVersionsList(int copyFromEvaluationId);
    }
}

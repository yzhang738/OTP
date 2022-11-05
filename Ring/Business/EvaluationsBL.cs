using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OTP.Ring.Common;
using OTP.Ring.Data.Models;
using System.Web;
using OTP.Ring.Models.ViewModel;
using System.Web.Mvc;
using OTP.Ring.Models;


namespace OTP.Ring.Business
{
    public class EvaluationsBL : IEvaluationsBL
    {        
        private bool _frenchLanguage = false;
        private Data.Models.RingEntities _repository;
        private IDecodeBL _decodeBL;

        public EvaluationsBL(string connectionString, IDecodeBL decodeBL)
        {
            _frenchLanguage = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToUpper() == Constant.LanguageCode.French;
            _repository = new Data.Models.RingEntities(connectionString);
            this._decodeBL = decodeBL;
        }

        private List<EvaluationsSummaryGridViewModel> GetEvaluationsSummaryGridViewModel(string yearId, int versionId, IList<ListItem> selectedSports)
        {
            List<EvaluationsSummaryGridViewModel> model = null;

            IEnumerable<string> selectedSportIds = (from a in selectedSports select a.Id);

            model = (from e in this._repository.Evaluations
                     where e.EvaluationVersionID == versionId && selectedSportIds.Contains(e.SportID)
                     select new EvaluationsSummaryGridViewModel
                     {
                         EvaluationId = e.EvaluationID,
                         EvaluationPhase = e.EvaluationPhase,
                         Rank = (from r in e.Sport.SportRankings
                                 where r.YearID == yearId
                                 select r.SportRankingCategoryID).FirstOrDefault(),
                         SportName = _frenchLanguage ? e.Sport.Name_FR : e.Sport.Name_EN,
                         Pillars = (from vq in e.EvaluationVersion.EvaluationVersionQuestions
                                    where vq.SortOrder == 0
                                    let q = e.EvaluationQuestions.Where(q2 => q2.EvaluationVersionQuestionID == vq.EvaluationVersionQuestionID).FirstOrDefault()
                                    select new Models.ListItem
                                    {
                                        Id = vq.EvaluationMasterQuestion.EvaluationMasterPillarID,
                                        Description = q.EvaluationStatusCurrent ?? q.EvaluationStatusInitial
                                    })
                     }).ToList();


            var evaluationActionItemTypes = (from t in this._repository.ActionItemTypes
                                             where t.SourceName_EN == "Evaluation"
                                             select t.ActionItemTypeID).ToList();

            foreach (var item in model)
            {
                var evaluationId = item.EvaluationId.ToString();

                var actionItems = from a in this._repository.ActionItems
                                  where evaluationActionItemTypes.Contains(a.ActionItemTypeID)
                                  && a.ReferenceID == evaluationId
                                  select a;

                item.OpenActionItems = (from a in actionItems
                                        where a.ActionItemStatus == Constant.ActionItemStatus.InProgress
                                            || a.ActionItemStatus == Constant.ActionItemStatus.New
                                        select a).Count();

                item.TotalActionItems = actionItems.Count();

                item.EvaluationOverallQuestions = this.GetEvaluationsSummaryOverallQuestionsGridViewModel(item.EvaluationId);
            }

            return model;
        }

        public List<EvaluationsSummaryOverallQuestionsGridViewModel> GetEvaluationsSummaryOverallQuestionsGridViewModel(int evaluationId)
        {
            var model = new List<EvaluationsSummaryOverallQuestionsGridViewModel>();

            model = (from q in this._repository.EvaluationQuestions
                     where q.EvaluationID == evaluationId
                     && q.EvaluationVersionQuestion.SortOrder == 0
                     orderby q.EvaluationVersionQuestion.EvaluationMasterQuestion.EvaluationMasterPillarID
                     select new EvaluationsSummaryOverallQuestionsGridViewModel
                     {
                         EvaluationQuestionId = q.EvaluationQuestionID,
                         EvaluationStatusCurrent = q.EvaluationStatusCurrent,
                         EvaluationStatusInitial = q.EvaluationStatusInitial,
                         EvalutionMasterPillarId = q.EvaluationVersionQuestion.EvaluationMasterQuestion.EvaluationMasterPillarID,
                         HPAAssessment = q.HPAAssessmentCurrent ?? q.HPAAssessmentInitial,
                         VersionQuestion = q.EvaluationVersionQuestion.VersionQuestion
                     }).ToList();

            return model;
        }

        public List<EvaluationsQuestionsEvaluationVersionGridViewModel> GetEvaluationsQuestionsEvaluationVersionGridViewModel(string yearId)
        {
            List<EvaluationsQuestionsEvaluationVersionGridViewModel> model = null;

            model = (from v in this._repository.EvaluationVersions
                     where v.YearID == yearId
                     select new EvaluationsQuestionsEvaluationVersionGridViewModel
                     {
                         EvaluationVersionId = v.EvaluationVersionID,
                         HasComments = !string.IsNullOrEmpty(v.Comments),
                         Name_EN = v.Name_EN,
                         Name_FR = v.Name_FR,
                         Name = _frenchLanguage ? v.Name_FR : v.Name_EN,
                         OrganizationTypeId = (from d in _repository.Decodes
                                               where d.DecodeID == v.OrganizationTypeID && d.DecodeCategoryID == "OrganizationType"
                                               select _frenchLanguage ? d.DecodeID_FR : d.DecodeID_EN).FirstOrDefault(),
                         VersionId = v.VersionID,
                         VersionPhase = (from d in _repository.Decodes
                                        where d.DecodeID == v.VersionPhase && d.DecodeCategoryID == "VersionPhase"
                                        select _frenchLanguage ? d.DecodeID_FR : d.DecodeID_EN).FirstOrDefault(),
                         YearId = v.YearID
                     }).ToList();

            return model;
        }

        public string IfEvaluationVersionExist(string organizationTypeId, string yearId, string versionId)
        {
            string versionID = (from v in this._repository.EvaluationVersions
                                where v.OrganizationTypeID == organizationTypeId && v.YearID == yearId && v.VersionID == versionId
                                select v.VersionID).FirstOrDefault();

            return versionID;
        }

        public int AddEvaluationVersion(
	        string organizationTypeId,
	        string yearId,
	        string versionId,
            string versionPhase,
	        string name_EN,
	        string name_FR,
            string comments)
        {
            int changes = 0;

            var returnValue = this._repository.uspAddEvaluationVersion(
                organizationTypeId,
                yearId,
                versionId,
                versionPhase,
                name_EN,
                name_FR,
                HttpUtility.HtmlEncode(comments)).SingleOrDefault();

            changes = returnValue == null ? changes : (int)returnValue;

            return changes;
        }

        public int UpdateEvaluationVersion(
            int evaluationVersionId,
            string organizationTypeId = Constant.UndefinedString,
            string yearId = Constant.UndefinedString,
            string versionId = Constant.UndefinedString,
            string versionPhase = Constant.UndefinedString,
            string name_EN = Constant.UndefinedString,
            string name_FR = Constant.UndefinedString,
            string comments = Constant.UndefinedString)
        {
            int changes = 0;

            var version = (from v in this._repository.EvaluationVersions
                         where v.EvaluationVersionID == evaluationVersionId
                         select v).First();

            version.OrganizationTypeID = organizationTypeId == Constant.UndefinedString ? version.OrganizationTypeID : organizationTypeId;
            version.YearID = yearId == Constant.UndefinedString ? version.YearID : yearId;
            version.VersionID = versionId == Constant.UndefinedString ? version.VersionID : versionId;
            version.VersionPhase = versionPhase == Constant.UndefinedString ? version.VersionPhase : versionPhase;
            version.Name_EN = name_EN == Constant.UndefinedString ? version.Name_EN : name_EN;
            version.Name_FR = name_FR == Constant.UndefinedString ? version.Name_FR : name_FR;
            version.Comments = comments == Constant.UndefinedString ? version.Comments : HttpUtility.HtmlEncode(comments);

            var returnValue = this._repository.uspUpdateEvaluationVersion(
                version.EvaluationVersionID,
                version.OrganizationTypeID,
                version.YearID,
                version.VersionID,
                version.VersionPhase,
                version.Name_EN,
                version.Name_FR,
                version.Comments).SingleOrDefault();

            changes = returnValue == null ? changes : (int)returnValue;

            return changes;
        }

        public void PopulateEvaluationsQuestionsViewData(ViewDataDictionary viewData, string yearId)
        {
            var id = GetFirstEvaluationVersionId(yearId);

            viewData["evaluationVersionId"] = id;

            viewData["versionId"] = (from v in this._repository.EvaluationVersions
                                                where v.EvaluationVersionID == id
                                                select v.VersionID).SingleOrDefault();

            if (viewData["versionId"] == null)
            {
                viewData["versionId"] = 0;
            }
        }

        public int GetFirstEvaluationVersionId(string yearId)
        {
            return (from v in this._repository.EvaluationVersions
                    where v.YearID == yearId
                    orderby v.EvaluationVersionID
                    select v.EvaluationVersionID).FirstOrDefault();
        }

        public List<EvaluationsQuestionsEvaluationPillarsGridViewModel> GetEvaluationsQuestionsEvaluationPillarsGridViewModel(int evaluationVersionId, bool showDeleted)
        {
            List<EvaluationsQuestionsEvaluationPillarsGridViewModel> model = null;

            model = (from q in this._repository.EvaluationVersionQuestions
                     where q.EvaluationVersionID == evaluationVersionId
                     let p = q.EvaluationMasterQuestion.EvaluationMasterPillar
                     orderby p.SortOrder
                     select new EvaluationsQuestionsEvaluationPillarsGridViewModel
                     {
                         EvaluationMasterPillarId = p.EvaluationMasterPillarID,
                         EvaluationMasterPillarName = (from d in _repository.Decodes 
                                                     where d.DecodeID == p.EvaluationMasterPillarID && d.DecodeCategoryID == "EvalMasterPillar"
                                                     select _frenchLanguage ? d.DecodeID_FR : d.DecodeID_EN).FirstOrDefault(),
                         EvaluationVersionId = evaluationVersionId,
                         Description = p.Description_EN,
                         Name = _frenchLanguage ? p.Name_FR : p.Name_EN,
                         NumberOfQuestions = (from q2 in this._repository.EvaluationVersionQuestions
                                              where q2.EvaluationVersionID == evaluationVersionId
                                              && q2.EvaluationMasterQuestion.EvaluationMasterPillarID == p.EvaluationMasterPillarID
                                              && (showDeleted || (q2.QuestionPhase != Constant.QuestionPhase.DeletedPrevious && q2.QuestionPhase != Constant.QuestionPhase.DeletedCurrent))
                                              select q2).Count()

                     }).Distinct().ToList();

            model = model.OrderBy(x => x.EvaluationMasterPillarName).ToList();
            return model;
        }

        public List<EvaluationsQuestionsEvaluationPillarQuestionsGridViewModel> GetEvaluationsQuestionsEvaluationPillarQuestionsGridViewModel(int evaluationVersionId, string evaluationMasterPillarId, bool showDeleted)
        {
            List<EvaluationsQuestionsEvaluationPillarQuestionsGridViewModel> model = null;

            model = (from q in this._repository.EvaluationVersionQuestions
                     where q.EvaluationVersionID == evaluationVersionId
                     && q.EvaluationMasterQuestion.EvaluationMasterPillarID == evaluationMasterPillarId
                     && (showDeleted || (q.QuestionPhase != Constant.QuestionPhase.DeletedPrevious && q.QuestionPhase != Constant.QuestionPhase.DeletedCurrent))
                     orderby q.SortOrder
                     select new EvaluationsQuestionsEvaluationPillarQuestionsGridViewModel
                     {
                         EvaluationMasterQuestionId = q.EvaluationMasterQuestionID,
                         EvaluationMasterPillarId = evaluationMasterPillarId,
                         EvaluationVersionId = q.EvaluationVersionID,
                         EvaluationVersionQuestionId = q.EvaluationVersionQuestionID,
                         HasComments = !string.IsNullOrEmpty(q.Comments),
                         QuestionPhase = (from d in _repository.Decodes 
                                          where d.DecodeID == q.QuestionPhase && d.DecodeCategoryID == "QuestionPhase"
                                          select _frenchLanguage ? d.DecodeID_FR : d.DecodeID_EN).FirstOrDefault(),
                         SortOrder = q.SortOrder,
                         VersionQuestion = _frenchLanguage ? q.VersionQuestionFR : q.VersionQuestion,
                         SportId = q.SportID,
                         Editable = q.EvaluationVersion.VersionPhase == Constant.VersionPhase.Edit
                     }).ToList();

            foreach (var question in model)
            {
                question.SportName = GetFlattenedSportHeirachySportName(question.SportId);
            }

            return model;
        }

        private string GetFlattenedSportHeirachySportName(string sportId)
        {
            var sport = (from s in this._repository.Sports
                         where s.SportID == sportId
                         select s).FirstOrDefault();

            var sportName = _frenchLanguage ? sport.Name_FR : sport.Name_EN;

            if (sport.SportLevelID >= Constant.SportLevelId.Discipline)
            {
                sportName = (_frenchLanguage ? sport.ParentSport.Name_FR : sport.ParentSport.Name_EN) + " - " + sportName;
            }

            if (sport.SportLevelID == 5)
            {
                sportName = (_frenchLanguage ? sport.ParentSport.ParentSport.Name_FR : sport.ParentSport.ParentSport.Name_EN) + " - " + sportName;
            }

            return sportName;
        }

        public int AddEvaluationVersionQuestion(
            int evaluationVersionId,
            string evaluationMasterPillarId,
            string versionQuestion,
            string questionPhase,
            int sortOrder,
            string comments,
            string sportId)
        {
            int changes = 0;

            var returnValue = this._repository.uspAddEvaluationVersionQuestion(
                evaluationVersionId,
                evaluationMasterPillarId,
                versionQuestion,
                questionPhase,
                sortOrder,
                HttpUtility.HtmlEncode(comments),
                sportId).SingleOrDefault();

            changes = returnValue == null ? changes : (int)returnValue;

            return changes;
        }

        public int UpdateEvaluationVersionQuestion(
            int evaluationVersionQuestionId,
            string versionQuestion = Constant.UndefinedString,
            string questionPhase = Constant.UndefinedString,
            int? sortOrder = null,
            string comments = Constant.UndefinedString,
            string sportId = Constant.UndefinedString)
        {
            int changes = 0;

            var question = (from q in this._repository.EvaluationVersionQuestions
                            where q.EvaluationVersionQuestionID == evaluationVersionQuestionId
                            select q).First();

            question.VersionQuestion = versionQuestion == Constant.UndefinedString ? question.VersionQuestion : versionQuestion;
            question.QuestionPhase = questionPhase == Constant.UndefinedString ? question.QuestionPhase : questionPhase;
            question.SortOrder = sortOrder == null ? question.SortOrder : (int)sortOrder;
            question.Comments = comments == Constant.UndefinedString ? question.Comments : HttpUtility.HtmlEncode(comments);
            question.SportID = sportId == Constant.UndefinedString ? question.SportID : sportId;

            var returnValue = this._repository.uspUpdateEvaluationVersionQuestion(
                question.EvaluationVersionQuestionID,
                question.VersionQuestion,
                question.QuestionPhase,
                question.SortOrder,
                question.Comments,
                question.SportID).SingleOrDefault();

            changes = returnValue == null ? changes : (int)returnValue;

            return changes;
        }

        public Common.EvaluationBestPractice GetEvaluationBestPractice(int bestPracticeId)
        {
            return (from b in this._repository.EvaluationBestPractices
                                                                 where b.EvaluationBestPracticeID == bestPracticeId
                                                                 select new Common.EvaluationBestPractice
                                                                 {
                                                                     Id = b.EvaluationBestPracticeID,
                                                                     EvaluationId = b.EvaluationID,
                                                                     MasterPillarId = b.EvaluationMasterPillarID,
                                                                     SportId = b.SportID,
                                                                     Description = b.Description,
                                                                     Comment = b.Comments,
                                                                     HasComment = !string.IsNullOrEmpty(b.Comments)
                                                                 }).SingleOrDefault();
        }

        public IEnumerable<Common.EvaluationBestPractice> GetEvaluationBestPractices(string pillarId)
        {
            List<Common.EvaluationBestPractice> bestPractices = (from b in this._repository.EvaluationBestPractices
                                                                 where b.EvaluationMasterPillarID == pillarId
                                                                 select new Common.EvaluationBestPractice
                                                                 {
                                                                     Id = b.EvaluationBestPracticeID,
                                                                     EvaluationId = b.EvaluationID,
                                                                     MasterPillarId = b.EvaluationMasterPillarID,
                                                                     SportId = b.SportID,
                                                                     YearId = b.Evaluation.EvaluationVersion.YearID,
                                                                     Description = b.Description,
                                                                     //DescriptionFR = b.Description_FR,
                                                                     Comment = b.Comments,
                                                                     HasComment = !string.IsNullOrEmpty(b.Comments)
                                                                 }).ToList();

            bestPractices = PopulateCodeDescription(bestPractices);

            bestPractices.Sort(new Common.EvaluationBestPractice.Comparer());

            return bestPractices;
        }

        public Common.Evaluation GetEvaluation(int id)
        {
            Common.Evaluation evaluation = null;

            var evaluations = (from e in this._repository.Evaluations
                                   where e.EvaluationID == id
                                   select new Common.Evaluation 
                                   { 
                                     Id = e.EvaluationID,
                                     VersionId = e.EvaluationVersionID,
                                     VersionYearId = e.EvaluationVersion.YearID,
                                     VersionName = _frenchLanguage ? e.EvaluationVersion.Name_FR : e.EvaluationVersion.Name_EN,
                                     SportId = e.SportID,
                                     SportDesc = _frenchLanguage ? e.Sport.Name_FR : e.Sport.Name_EN,
                                     Phase = e.EvaluationPhase,
                                     Comment = e.Comments,
                                     VersionPhase = e.EvaluationVersion.VersionPhase
                                   }).ToList();

            if (evaluations != null && evaluations.Count == 1)
            {
                evaluation = evaluations.First();
                evaluation = PopulateCodeDescription(evaluation);

                evaluation.Pillars = GetEvaluationQuestions(id);
            }

            return evaluation;
        }

        public IEnumerable<Common.EvaluationPillar> GetEvaluationQuestions(int id)
        {
            //e.SportID is always at sport level
            //vq.SportID could be at sport level and up
            List<Common.EvaluationQuestion> questions = (from vq in this._repository.EvaluationVersionQuestions
                      join mq in this._repository.EvaluationMasterQuestions on vq.EvaluationMasterQuestionID equals mq.EvaluationMasterQuestionID
                      join e in this._repository.Evaluations on vq.EvaluationVersionID equals e.EvaluationVersionID
                      join s3 in this._repository.Sports on e.SportID equals s3.SportID 
                      join s2 in this._repository.Sports on s3.ParentSportID equals s2.SportID 
                      join s1 in this._repository.Sports on s2.ParentSportID equals s1.SportID 
                      where e.EvaluationID == id && s3.SportLevelID == Constant.SportLevelId.Sport && s2.SportLevelID == Constant.SportLevelId.Season && s1.SportLevelID == Constant.SportLevelId.Olympic
                        && (e.SportID == vq.SportID || s3.ParentSportID == vq.SportID || s2.ParentSportID == vq.SportID || s1.ParentSportID == vq.SportID) 
                        && (vq.QuestionPhase == Constant.QuestionPhase.New || vq.QuestionPhase == Constant.QuestionPhase.CopiedFromPrevious)
                      select new Common.EvaluationQuestion
                      {
                          VersionQuestionId = vq.EvaluationVersionQuestionID,
                          VersionId = vq.EvaluationVersionID,
                          MasterQuestionId = vq.EvaluationMasterQuestionID,
                          MasterPillarId = mq.EvaluationMasterPillarID,
                          SportId = vq.SportID,

                          Question = _frenchLanguage ? vq.VersionQuestionFR : vq.VersionQuestion,
                          Phase = vq.QuestionPhase,
                          SortOrder = vq.SortOrder
                      }).ToList();

            questions = FillEvaluationAnswers(id, questions);
            questions.Sort(new Common.EvaluationQuestion.Comparer());

            //group questions by pillar
            List<Common.EvaluationPillar> pillars = (from g in (from q in questions group q by q.MasterPillarId) 
                                                 select new Common.EvaluationPillar { Id = g.Key, Questions = g.ToList() }).ToList();
            pillars = PopulateCodeDescription(pillars);
            pillars.Sort(new Common.EvaluationPillar.Comparer());

            return pillars;
        }

        public int SaveEvaluationQuestion(int evaluationId, string pillarId, Common.EvaluationQuestion question)
        {
            int result = 0;

            //Save Sport Specific Question
            result += SaveEvaluationQuestion_Question(evaluationId, pillarId, question);

            //Save Answer
            result += SaveEvaluationQuestion_Answer(evaluationId, pillarId, question);

            return result;
        }

        private int SaveEvaluationQuestion_Question(int evaluationId, string pillarId, Common.EvaluationQuestion question)
        {
            int result = 0;

            Common.Evaluation evaluation = this.GetEvaluation(evaluationId);

            if (evaluation != null && evaluation.Pillars != null)
            {
                EvaluationPillar pillar = (from p in evaluation.Pillars where p.Id == pillarId select p).FirstOrDefault();

                if (pillar != null && pillar.Questions != null && pillar.Questions.Count() > 0)
                {
                    Common.EvaluationQuestion overallQuestion = pillar.Questions.First();
                    int sortOrder = (from q in pillar.Questions orderby q.SortOrder descending select q.SortOrder).FirstOrDefault();

                    if (question.VersionQuestionId == 0)
                    {
                        question.VersionQuestionId = this.AddEvaluationVersionQuestion(overallQuestion.VersionId, pillarId, question.Question, Constant.QuestionPhase.New, sortOrder + 1, null, evaluation.SportId);
                    }
                    else if (!string.IsNullOrEmpty(question.Question))
                    {
                        this.UpdateEvaluationVersionQuestion(question.VersionQuestionId, versionQuestion: question.Question);
                    }
                }
            }

            return result;
        }

        private int SaveEvaluationQuestion_Answer(int evaluationId, string pillarId, Common.EvaluationQuestion question)
        {
            int result = 0;

            //Save Answer
            var dataQuestions = (from c in this._repository.EvaluationQuestions
                                 where c.EvaluationQuestionID == question.Id
                                 select c).ToList();

            if (dataQuestions == null || dataQuestions.Count() == 0)
            {
                //Insert
                var output = this._repository.uspAddEvaluationQuestion(evaluationId,
                    question.VersionQuestionId,
                    question.NotApplicable,
                    question.InitialAssessment,
                    question.InitialStatus,
                    question.InitialAssessment,
                    question.InitialStatus,
                    question.Comment).SingleOrDefault();

                result = output == null ? result : (int)output;
            }
            else
            {
                var dataQuestion = dataQuestions.First();

                string initialAssessment = null;
                string initialStatus = null;
                string currentAssessment = null;
                string currentStatus = null;

                if (!question.NotApplicable)
                {
                    initialAssessment = string.IsNullOrEmpty(question.InitialAssessment) ? dataQuestion.HPAAssessmentInitial : question.InitialAssessment;
                    initialStatus = string.IsNullOrEmpty(question.InitialStatus) ? dataQuestion.EvaluationStatusInitial : question.InitialStatus;
                    currentAssessment = question.CurrentAssessment;
                    currentStatus = question.CurrentStatus;
                }

                //Update
                var output = this._repository.uspUpdateEvaluationQuestion(question.Id,
                        dataQuestion.EvaluationID,
                        dataQuestion.EvaluationVersionQuestionID,
                        question.NotApplicable,
                        initialAssessment,
                        initialStatus,
                        currentAssessment,
                        currentStatus,
                        question.Comment).SingleOrDefault();

                result = output == null ? result : (int)output;

                this._repository.EvaluationQuestions.MergeOption = System.Data.Objects.MergeOption.OverwriteChanges;
            }

            return result;
        }

        public int UpdateEvaluationPhase(int id, string fromPhase, string toPhase)
        {
            int result = 0;

            var dataEvaluations = (from e in this._repository.Evaluations
                                   where e.EvaluationID == id
                                   select e).ToList();

            if (dataEvaluations != null && dataEvaluations.Count() == 1)
            {
                var dataEvaluation = dataEvaluations.First();

                var output = this._repository.uspUpdateEvaluation(id, dataEvaluation.EvaluationVersionID, dataEvaluation.SportID, toPhase, dataEvaluation.Comments).SingleOrDefault();
                result = output == null ? result : (int)output;
            }

            result += UpdateEvaluationQuestionForPhaseChange(id, fromPhase, toPhase);

            return result;
        }

        public int UpdateEvaluationQuestionForPhaseChange(int id, string fromPhase, string toPhase)
        {
            int result = 0;

            //Copy InitialStatus to CurrentStatus when evaluation is submitted
            if (toPhase == Constant.EvaluationPhase.Submit)
            {
                var dataEvaluationQuestions = (from q in this._repository.EvaluationQuestions
                                               where q.EvaluationID == id
                                               select q).ToList();

                if (dataEvaluationQuestions != null)
                {
                    foreach (var dataEvaluationQuestion in dataEvaluationQuestions)
                    { 
                        var output = this._repository.uspUpdateEvaluationQuestion(dataEvaluationQuestion.EvaluationQuestionID,
                            dataEvaluationQuestion.EvaluationID,
                            dataEvaluationQuestion.EvaluationVersionQuestionID,
                            dataEvaluationQuestion.NotApplicable,
                            dataEvaluationQuestion.HPAAssessmentInitial,
                            dataEvaluationQuestion.EvaluationStatusInitial,
                            dataEvaluationQuestion.HPAAssessmentInitial,
                            dataEvaluationQuestion.EvaluationStatusInitial,
                            dataEvaluationQuestion.Comments).SingleOrDefault();

                        result += output == null ? 0 : (int)output;
                    }
                }
            }

            return result;
        }

        public int InsertEvaluationBestPractice(Common.EvaluationBestPractice bestPractice)
        {
            int result = 0;

            var output = this._repository.uspAddEvaluationBestPractice(bestPractice.EvaluationId,
                bestPractice.MasterPillarId,
                bestPractice.SportId,
                bestPractice.Description,
                bestPractice.Comment).SingleOrDefault();

            result = output == null ? result : (int)output;

            return result;
        }

        public int UpdateEvaluationBestPractice(Common.EvaluationBestPractice bestPractice)
        {
            int result = 0;

            var dataBestPractices = (from c in this._repository.EvaluationBestPractices
                                   where c.EvaluationBestPracticeID == bestPractice.Id 
                                   select c).ToList();

            if (dataBestPractices != null && dataBestPractices.Count() == 1)
            {
                var dataBestPractice = dataBestPractices.First();

                var output = this._repository.uspUpdateEvaluationBestPractice(bestPractice.Id,
                    dataBestPractice.EvaluationID,
                    dataBestPractice.EvaluationMasterPillarID,
                    dataBestPractice.SportID,
                    bestPractice.Description,
                    dataBestPractice.Comments).SingleOrDefault();

                result = output == null ? result : (int)output;

                this._repository.EvaluationBestPractices.MergeOption = System.Data.Objects.MergeOption.OverwriteChanges;
            }

            return result;
        }

        public int DeleteEvaluationBestPractice(int bestPracticeId)
        {
            int result = 0;

            var dataBestPractices = (from c in this._repository.EvaluationBestPractices
                                     where c.EvaluationBestPracticeID == bestPracticeId
                                     select c).ToList();

            if (dataBestPractices != null && dataBestPractices.Count() == 1)
            {
                var output = this._repository.uspDeleteEvaluationBestPractice(bestPracticeId).SingleOrDefault();
                result = output == null ? result : (int)output;
            }

            return result;
        }

        public string GetActionItemRatioForEvaluationPillar(int evaluationId, string pillarId)
        {
            string referenceId = evaluationId.ToString();

            var actionItems = (from a in this._repository.ActionItems
                               where a.ActionItemTypeID == pillarId && a.ReferenceID == referenceId
                               select a).ToList();

            var openActionItems = (from a in actionItems
                                   where a.ActionItemStatus == Constant.ActionItemStatus.InProgress
                                       || a.ActionItemStatus == Constant.ActionItemStatus.New
                                   select a).ToList();

            return string.Format("{0}/{1}", openActionItems.Count, actionItems.Count);
        }

        private List<Common.EvaluationBestPractice> PopulateCodeDescription(List<Common.EvaluationBestPractice> bestPractices)
        {
            List<Data.Models.Sport> sports = (from s in this._repository.Sports where s.SportLevelID < Constant.SportLevelId.Event select s).ToList();

            foreach (var bestPractice in bestPractices)
            {
                Data.Models.Sport sport = (from s in sports where s.SportID == bestPractice.SportId select s).SingleOrDefault();
                if (sport == null) continue;

                bestPractice.SportDesc = _frenchLanguage ? sport.Name_FR : sport.Name_EN;
            }

            return bestPractices;
        }

        private Common.Evaluation PopulateCodeDescription(Common.Evaluation evaluation)
        {
            if (evaluation == null) return null;

            var types = this._decodeBL.GetDecodesByCategory(Constant.DecodeCategoryId.EvaluationPhase);

            evaluation.PhaseDesc = (from d in types where d.Id == evaluation.Phase select d.Name).FirstOrDefault();

            return evaluation;
        }

        private List<Common.EvaluationPillar> PopulateCodeDescription(List<Common.EvaluationPillar> pillars)
        {
            if (pillars == null) return pillars;

            var dataPillars = (from p in this._repository.EvaluationMasterPillars
                               select p).ToList();

            foreach (var pillar in pillars)
            {
                var dataPillar = (from p in dataPillars where p.EvaluationMasterPillarID == pillar.Id select p).SingleOrDefault();

                if (dataPillar == null) continue;

                pillar.Id_Name = (from d in this._repository.Decodes
                                  where d.DecodeCategoryID == "EvalMasterPillar" && d.DecodeID == pillar.Id
                                  select _frenchLanguage ? d.DecodeID_FR : d.DecodeID_EN).SingleOrDefault();
                pillar.Name = dataPillar.Name_EN;
                pillar.Description = dataPillar.Description_EN;
                pillar.SortOrder = dataPillar.SortOrder;
                pillar.Comment = dataPillar.Comments;
            }

            return pillars;
        }

        private List<Common.EvaluationQuestion> FillEvaluationAnswers(int id, List<Common.EvaluationQuestion> questions)
        {
            var dataAnswers = (from q in this._repository.EvaluationQuestions
                                 where q.EvaluationID == id
                                 select q).ToList();

            foreach (var question in questions)
            {
                var answers = (from a in dataAnswers
                               where a.EvaluationVersionQuestionID == question.VersionQuestionId
                               select a).ToList();

                if (answers != null && answers.Count == 1)
                {
                    var answer = answers.First();

                    question.Id = answer.EvaluationQuestionID;

                    question.NotApplicable = answer.NotApplicable;
                    question.InitialStatus = answer.EvaluationStatusInitial;
                    question.CurrentStatus = answer.EvaluationStatusCurrent;
                    question.InitialAssessment = answer.HPAAssessmentInitial;
                    question.CurrentAssessment = answer.HPAAssessmentCurrent;
                    question.HasComments = !string.IsNullOrEmpty(answer.Comments);
                }
            }

            return questions;
        }

        public List<ListItem> GetCopyToYearsList(int evaluationVersionId)
        {
            List<ListItem> years = null;

            years = (from y in this._repository.Years
                     where (y.Active == null || (bool)y.Active)
                     orderby y.YearID
                     select new ListItem
                     {
                         Id = y.YearID,
                         Description = y.YearID
                     }).ToList();

            return years;
        }

        public int CopyEvaluationVersion(
            int evaluationVersionId,
            string copyToYearId)
        {
            int changes = 0;

            var returnValue = this._repository.uspCopyEvaluationVersion(
                evaluationVersionId,
                copyToYearId).SingleOrDefault();

            changes = returnValue == null ? changes : (int)returnValue;

            return changes;
        }

        public List<ListItem> GetCopyToEvaluationVersionsList(int copyFromEvaluationId)
        {
            var versionList = new List<ListItem>();

            var versions = (from e in this._repository.EvaluationVersions
                            where e.EvaluationVersionID != copyFromEvaluationId
                            && e.VersionPhase == Constant.VersionPhase.Open
                            let d = (from d in this._repository.Decodes
                                     where d.DecodeCategoryID == Constant.DecodeCategoryId.VersionPhase
                                     && d.DecodeID == e.VersionPhase
                                     select d).FirstOrDefault()
                            select new
                            {
                                Id = e.EvaluationVersionID,
                                Description = e.OrganizationTypeID + " - " + e.YearID + " - " + e.VersionID + " (" + (_frenchLanguage ? d.DecodeID_FR : d.DecodeID_EN) + ")",
                            }).ToList();

            versionList = versions.Select(v => new ListItem { Id = v.Id.ToString(), Description = v.Description }).ToList();

            return versionList;
        }

        private List<ListItem> GetEvaluationVersionsList(string yearId, int? evaluationId, out int selectedIndex, out bool evaluationVersionEditable)
        {
            selectedIndex = 0;
            evaluationVersionEditable = false;
            var versionList = new List<ListItem>();

            var versions = (from e in this._repository.EvaluationVersions
                            where e.YearID == yearId
                            && e.VersionPhase != Constant.VersionPhase.Edit
                            let d = (from d in this._repository.Decodes
                                     where d.DecodeCategoryID == Constant.DecodeCategoryId.VersionPhase
                                     && d.DecodeID == e.VersionPhase
                                     select d).FirstOrDefault()
                            select new
                            {
                                SortOrder = d.SortOrder,
                                Id = e.EvaluationVersionID,
                                Description = e.OrganizationTypeID + " - " + e.VersionID + " (" + (_frenchLanguage ? d.DecodeID_FR : d.DecodeID_EN) + ")",
                                Phase = e.VersionPhase
                            }).OrderBy(v => v.SortOrder).ToList();

            if (versions != null && versions.Count > 0)
            {
                if (evaluationId != null)
                {
                    selectedIndex = versions.IndexOf(versions.Where(v => v.Id == (int)evaluationId).SingleOrDefault());
                }
                else
                {
                    selectedIndex = versions.IndexOf(versions.Where(v => v.Phase != Constant.EvaluationPhase.Closed).FirstOrDefault());
                }

				evaluationVersionEditable = versions[selectedIndex < 0 ? 0 : selectedIndex].Phase == Constant.VersionPhase.Open;
                versionList = versions.Select(v => new ListItem { Id = v.Id.ToString(), Description = v.Description }).ToList();
            }

            return versionList;
        }

        public EvaluationsSummaryViewModel PopulateEvaluationsSummaryViewModel(string yearId, int? evaluationId, IList<ListItem> selectedSports)
        {
            var model = new EvaluationsSummaryViewModel();
            int selectedIndex = 0;
            bool evaluationVersionEditable = false;

			//year should be based on the evaluationID. previously it was using the "season" which led to wrong collection of versions
			if (evaluationId != null)
				yearId = (from y in this._repository.EvaluationVersions
						  where y.EvaluationVersionID == evaluationId
						  select y.YearID).FirstOrDefault().ToString();

            model.EvaluationVersions = this.GetEvaluationVersionsList(yearId, evaluationId, out selectedIndex, out evaluationVersionEditable);
            model.SelectedEvaluationVersionIndex = selectedIndex < 0 ? 0 : selectedIndex;
            model.EvaluationVersionEditable = evaluationVersionEditable;
			model.Year = yearId;

            if (model.EvaluationVersions.Count > 0)
            {
                model.Evaluations = this.GetEvaluationsSummaryGridViewModel(yearId, int.Parse(model.EvaluationVersions[model.SelectedEvaluationVersionIndex].Id), selectedSports);
            }
            else
            {
                model.Evaluations = new List<EvaluationsSummaryGridViewModel>();
            }

            return model;
        }

        public int AddEvaluation(
	        int evaluationVersionId,
	        string sportId,
	        string evaluationPhase,
	        string comments)
        {
            int changes = 0;

            var returnValue = this._repository.uspAddEvaluation(
                evaluationVersionId,
                sportId,
                evaluationPhase,
                HttpUtility.HtmlEncode(comments)).SingleOrDefault();

            changes = returnValue == null ? changes : (int)returnValue;

            return changes;
        }

        public List<ListItem> GetAddEvaluationSportList(int evaluationVersionId)
        {
            List<ListItem> sports = null;

            var allSports = (from s in this._repository.Sports
                             where s.SportLevelID == Constant.SportLevelId.Sport
                             && s.Active
                             select s).ToList();

            var alreadyUsedSports = (from e in this._repository.Evaluations
                                     where e.EvaluationVersionID == evaluationVersionId
                                     select e.Sport).ToList();

            sports = (from s in allSports
                      where !alreadyUsedSports.Contains(s)
                      select new ListItem
                      {
                          Id = s.SportID,
                          Description = _frenchLanguage ? s.Name_FR : s.Name_EN
                      }).OrderBy(o => o.Description).ToList();

            return sports;
        }

        public List<ListItem> GetVersionPhasesList(string currentVersionPhase)
        {
            List<ListItem> phases = new List<ListItem>();

            var versionPhases = (from p in this._repository.Decodes
                                 where p.DecodeCategoryID == Constant.DecodeCategoryId.VersionPhase
                                 orderby p.SortOrder
                                 select p).ToList();

            OTP.Ring.Data.Models.Decode currentPhase = null;

            if (!string.IsNullOrEmpty(currentVersionPhase))
            {
                currentPhase = (from p in versionPhases
                                where p.DecodeID == currentVersionPhase
                                select p).SingleOrDefault();
            }
            else
            {
                currentPhase = (from p in versionPhases
                                where p.SortOrder == 0
                                select p).SingleOrDefault();
            }

            var nextPhase = (from p in versionPhases
                             where p.SortOrder == currentPhase.SortOrder + 1
                             select p).SingleOrDefault();

            phases.Add(new ListItem
                {
                    Id = currentPhase.DecodeID,
                    Description = _frenchLanguage ? currentPhase.DecodeID_FR : currentPhase.DecodeID_EN
                });

            if (nextPhase != null)
            {
                phases.Add(new ListItem
                            {
                                Id = nextPhase.DecodeID,
                                Description = _frenchLanguage ? nextPhase.DecodeID_FR : nextPhase.DecodeID_EN
                            });
            }

            return phases;
        }

        public List<ListItem> GetVersionIdList()
        {
            List<ListItem> versionIDs = null;

            versionIDs = (from v in this._repository.EvaluationVersions
                          select new ListItem
                          {
                              Id = v.VersionID,
                              Description = _frenchLanguage ? v.Name_FR : v.Name_EN
                          }).Distinct().OrderBy(o => o.Id).ToList();

            return versionIDs;
        }

        public List<ListItem> GetVersionNameENList()
        {
            List<ListItem> versionNames = (from v in this._repository.EvaluationVersions
                                           select new ListItem
                                           {
                                               Id = v.VersionID,
                                               Description = v.Name_EN
                                           }).Distinct().OrderBy(o => o.Id).ToList();

            return versionNames;
        }

        public List<ListItem> GetVersionNameFRList()
        {
            List<ListItem> versionNames = (from v in this._repository.EvaluationVersions
                                           select new ListItem
                                           {
                                               Id = v.VersionID,
                                               Description = v.Name_FR
                                           }).Distinct().OrderBy(o => o.Id).ToList();

            return versionNames;
        }

        public string GetVersionID(string versionName)
        {
            string versionID = (from v in this._repository.EvaluationVersions
                                  where v.Name_EN == versionName || v.Name_FR == versionName
                                  select v.VersionID
                                  ).FirstOrDefault();

            return versionID;
        }

        public string GetVersionNameEn(string versionID)
        {
            string versionName = (from v in this._repository.EvaluationVersions
                                  where v.VersionID == versionID
                                  select v.Name_EN
                                  ).FirstOrDefault();

            return versionName;
        }

        public string GetVersionNameFr(string versionID)
        {
            string versionName = (from v in this._repository.EvaluationVersions
                                  where v.VersionID == versionID
                                  select v.Name_FR
                                  ).FirstOrDefault();

            return versionName;
        }

        public List<ListItem> GetQuestionPhasesList(string currentQuestionPhase)
        {
            List<ListItem> phases = new List<ListItem>();

            if (string.IsNullOrEmpty(currentQuestionPhase))
            {
                currentQuestionPhase = Constant.QuestionPhase.New;
            }

            var allowedPhases = new List<string>();

            switch (currentQuestionPhase)
            {
                case Constant.QuestionPhase.New:
                    allowedPhases.Add(Constant.QuestionPhase.New);
                    break;
                case Constant.QuestionPhase.CopiedFromPrevious:
                    allowedPhases.Add(Constant.QuestionPhase.CopiedFromPrevious);
                    allowedPhases.Add(Constant.QuestionPhase.DeletedCurrent);
                    break;
                case Constant.QuestionPhase.DeletedCurrent:
                    allowedPhases.Add(Constant.QuestionPhase.DeletedCurrent);
                    allowedPhases.Add(Constant.QuestionPhase.CopiedFromPrevious);
                    break;
                case Constant.QuestionPhase.DeletedPrevious:
                    allowedPhases.Add(Constant.QuestionPhase.DeletedPrevious);
                    allowedPhases.Add(Constant.QuestionPhase.CopiedFromPrevious);
                    break;

                default:
                    break;
            }

            phases = (from p in this._repository.Decodes
                      where p.DecodeCategoryID == Constant.DecodeCategoryId.QuestionPhase
                      && allowedPhases.Contains(p.DecodeID)
                      orderby p.SortOrder
                      select new ListItem
                      {
                          Id = p.DecodeID,
                          Description = _frenchLanguage ? p.DecodeID_FR : p.DecodeID_EN
                      }).ToList();

            return phases;
        }

        public int DeleteEvaluationVersionQuestion(int evaluationVersionQuestionId)
        {
            int result = 0;

            var output = this._repository.uspDeleteEvaluationVersionQuestion(evaluationVersionQuestionId).SingleOrDefault();
            result = output == null ? result : (int)output;

            return result;
        }

        public int CopyEvaluation(
            int evaluationId,
            int copyToEvaluationVersionId)
        {
            int changes = 0;

            var returnValue = this._repository.uspCopyEvaluation(
                evaluationId,
                copyToEvaluationVersionId).SingleOrDefault();

            changes = returnValue == null ? changes : (int)returnValue;

            return changes;
        }

        public bool CanCopyEvaluation(int evaluationId, int copyToEvaluationVersionId, ModelStateDictionary modelState)
        {
            OTP.Ring.Common.Evaluation oldEval = this.GetEvaluation(evaluationId);

            var result = !(from e in this._repository.Evaluations
                           where e.EvaluationVersionID == copyToEvaluationVersionId
                           && e.SportID == oldEval.SportId
                           select e).Any();

            if (!result)
            {
                modelState.AddModelError("", BusinessLocalization.EvaluationsBL_Error_VersionCopyExists);
            }

            //validation: when submit or close an evluation, ensure all status are entered
            if (oldEval != null && oldEval.Pillars != null)
            {
                foreach (var pillar in oldEval.Pillars)
                {
                    if (pillar.Questions == null)
                    {
                        continue;
                    }

                    var questionWithoutAnswer = (from q in pillar.Questions where q.Id == 0 select q).Any();

                    if (questionWithoutAnswer)
                    {
                        result = false;
                        modelState.AddModelError("", string.Format(BusinessLocalization.EvaluationBL_Error_MissingAssessment, pillar.Name));
                    }
                }
            }

            return result;
        }
    }
}

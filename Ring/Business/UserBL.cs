using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OTP.Ring.Data.Models;
using OTP.Ring.Common;
using OTP.Ring.Models;

namespace OTP.Ring.Business
{
    public class UserBL : IUserBL
    {
        private bool _frenchLanguage = false;
        private Data.Models.RingEntities _repository;

        public UserBL(string connectionString)
        {
            _frenchLanguage = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToUpper() == Constant.LanguageCode.French;
            _repository = new Data.Models.RingEntities(connectionString);
        }

        public List<User> GetAllUsers()
        {
            var allUsers = new List<User>();

            foreach (var securityUser in this._repository.SecurityUsers)
            {
                allUsers.Add(ConvertUser(securityUser));
            }

            return allUsers;
        }

        public User GetUserByLiveId(string liveId)
        {
            var user = RetrieveUser(liveId);

            if (user == null)
            {
                user = RetrieveUser("guest");
            }

            user.AuthenticatedLiveId = liveId;
            user.Pages = RetrievePages(user);
            user.Sports = RetrieveSports(user);

            return user;
        }

        public int SaveUser(User user)
        {
            int result = 0;

            var dataUsers = from d in this._repository.SecurityUsers 
                                        where d.LiveID == user.LiveId
                                        select d;

            if (dataUsers != null && dataUsers.Count() == 1)
            {
                //Update
                var output = this._repository.uspUpdateUser(user.LiveId, user.FirstName, user.LastName, user.Comments).SingleOrDefault();
                result = output == null ? result : (int)output;
            }
            else
            { 
                //Insert
                var output = this._repository.uspAddUser(user.LiveId, user.FirstName, user.LastName, user.Comments).SingleOrDefault();
                result = output == null ? result : (int)output;
            }

            return result;
        }

        private List<Sport> RetrieveSports(User user)
        {
            var sports = from s in this._repository.Sports
                         join us in this._repository.SecurityUserSports on s.SportID equals us.SportID
                         where us.SecurityUserProfileID == user.UserProfileId 
                         select s;

            return sports.ToList();
        }

        private List<Page> RetrievePages(User user)
        {
            var pages = from p in this._repository.SecurityPages
                        join gp in this._repository.SecurityGroupPages on p.SecurityPageID equals gp.SecurityPageID
                        where gp.SecurityGroupProfileID == user.GroupProfileId
                        select new Page() 
                        { 
                            Id = p.SecurityPageID, 
                            Name_EN = p.Name_EN, 
                            Name_FR = p.Name_FR,
                            Url = p.URL, 
                            ParentId = p.ParentSecurityPageID, 
                            Order = p.Order, 
                            ReadAccess = gp.ReadAccess, 
                            WriteAccess = gp.WriteAccess, 
                            Recursive = gp.Recursive 
                        };

            return pages.ToList();
        }

        private User RetrieveUser(string liveId)
        {
            User user = null;

            //We retry here as this is the first call to the DB
            //Number of times the call will be retry'd if previous call timed out
            int retryCount = 5;
            bool success = false;
            while (retryCount > 0 && !success)
            {
                try
                {
                    retryCount--;

                    var sUsers = (from u in this._repository.SecurityUsers
                                  where u.LiveID == liveId
                                  select u).SingleOrDefault();

                    if (sUsers != null)
                    {
                        user = ConvertUser(sUsers);
                    }

                    success = true;
                }
                catch (System.Data.EntityException e)
                {
                    Logger.LogError(e.ToString());
                    //Only show timeout message if no more retrys
                    if (e.Message != "The underlying provider failed on Open." || retryCount == 0)
                    {
                        throw e;
                    }

                    //Wait a small amount of time before retrying
                    System.Threading.Thread.Sleep(5000);
                }
                catch (Exception e)
                {
                    Logger.LogError(e.ToString());
                    //Don't retry 
                    retryCount = 0;
                    throw e;
                }
            }

            return user;
        }

        private User ConvertUser(SecurityUser securityUser)
        {
            return new User()
            {
                Id = securityUser.SecurityUserID,
                LiveId = securityUser.LiveID,
                FirstName = securityUser.FirstName,
                LastName = securityUser.LastName,
                UserProfileId = securityUser.SecurityUserProfile.SecurityUserProfileID,
                UserProfileName = securityUser.SecurityUserProfile.Name,
                GroupProfileId = securityUser.SecurityUserProfile.SecurityGroupProfile.SecurityGroupProfileID,
                GroupProfileName = securityUser.SecurityUserProfile.SecurityGroupProfile.Name,
                HomePageId = securityUser.SecurityUserProfile.SecurityGroupProfile.SecurityPageIDHome
            };
        }

        public ActionLocation GetPageActionLocation(string securityPageId)
        {
            var pageUrl = (from p in this._repository.SecurityPages
                           where p.SecurityPageID == securityPageId
                           select p.URL).FirstOrDefault();

            return new ActionLocation(pageUrl);
        }
    }
}

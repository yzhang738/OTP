using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OTP.Ring.Common;
using OTP.Ring.Data.Models;
using System.Web;
using System.Text.RegularExpressions;


namespace OTP.Ring.Business
{
    public class CommentsBL : ICommentsBL
    {        
        private bool _frenchLanguage = false;
        private Data.Models.RingEntities _repository;

        public CommentsBL(string connectionString)
        {
            _frenchLanguage = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToUpper() == Constant.LanguageCode.French;
            _repository = new Data.Models.RingEntities(connectionString);
        }

        public string GetComments(
            string tableName,
            string primaryKey)
        {
            string comments = null;

            comments = this._repository.uspGetComments(
                tableName,
                primaryKey).SingleOrDefault();

            comments = HttpUtility.HtmlDecode(comments);
            if (comments == null || comments == "null")
            {
                comments = "";
            }

            var encodedComments = Microsoft.JScript.GlobalObject.escape(comments);
            return encodedComments;
        }

        public void UpdateComments(
            string tableName,
            string primaryKey,
            string comments)
        {
            var decodedComments = Microsoft.JScript.GlobalObject.unescape(comments);
            var sansHtmlComments = Regex.Replace(decodedComments, "</?[a-z][a-z0-9]*[^<>]*>|<!--.*?-->", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);

            if (string.IsNullOrWhiteSpace(sansHtmlComments))
            {
                decodedComments = null;
            }
            else
            {
                decodedComments = HttpUtility.HtmlEncode(decodedComments);
            }

            this._repository.uspUpdateComments(
                tableName,
                primaryKey,
                decodedComments);
        }
    }
}

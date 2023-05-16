using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KP.Register.Web.API.Helpers
{
    public static class TokenFileReader
    {
        public static List<TokenFileModel> GetAll()
        {
            List<TokenFileModel> ret = new List<TokenFileModel>();
            try
            {
                #region old path
                /*
                string path = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                var directory = Path.GetDirectoryName(path);
                var sFileName = directory + Path.DirectorySeparatorChar + "Content" + Path.DirectorySeparatorChar + "AllowToken.txt";
                sFileName = sFileName.Replace("file:\\", "");
                */
                #endregion
                string sFileName = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/AllowToken.txt");
                string[] lines = System.IO.File.ReadAllLines(sFileName);
                foreach (var line in lines)
                {
                    if (!line.StartsWith("#"))
                    {
                        var tmp = line.Split(',');
                        var model = new TokenFileModel();
                        model.Name = tmp.First().Trim();
                        model.Token = tmp.Last().Trim();
                        ret.Add(model);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return ret;
        }
    }

    public class TokenFileModel
    {
        public string Name { get; set; }
        public string Token { get; set; }
    }
}
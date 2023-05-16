using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace KP.Register.API.Helpers
{
    public class TokenFileReader
    {
        private IPathProvider _pathProvider;
        public TokenFileReader(IPathProvider pathProvider)
        {
            _pathProvider = pathProvider;
        }
        public List<TokenFileModel> GetAll()
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
                //string sFileName = HostingEnvironment.MapPath("~/Content/AllowToken.txt");
                string sFileName = _pathProvider.MapPath("~/Content/AllowToken.txt");
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NlpSql.DemoWeb.Data
{
    public class AppConstants
    {

        public const int TokenLimit = 4000;
        public static string OpenAIApiKey = "";//"-- Open AI Key --";
        public static string OrgID = "";//"-- ORG ID --";
        public static string Model = "";
        public static string Type = "openai";
        public static string SchemaNames = "";
        public static (string model, string apiKey, string orgId) GetSettings()
        {
            return (Model, OpenAIApiKey, OrgID);
        }
        
        public static long MaxAllowedFileSize = 10 * 1024000;
        public static long DefaultStorageSize = 21474836480;

    }
}

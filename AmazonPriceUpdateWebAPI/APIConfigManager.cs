using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace AmazonPriceUpdateWebAPI
{
    public static class APIConfigManager
    {
        public const string PrmAWSAccessKey = "AWSAccessKey";
        public const string PrmAWSSecretKey = "AWSSecretKey";
        public const string PrmServiceAcctId = "ServiceAccountId";
        public const string PrmServiceAcctPwd = "ServiceAccountPassword";
        public const string PrmAdminAcctId = "AdminAccountId";
        public const string PrmTablesToCreate = "TablesToCreate";
        public const string PrmBaseRequestUri = "BaseRequestUri";
        public static string AWSAccessKey
        {
            get
            {
                return ConfigurationManager.AppSettings[PrmAWSAccessKey];        
            }
        }

        public static string AWSSecretKey
        {
            get
            {
                return ConfigurationManager.AppSettings[PrmAWSSecretKey];
            }
        }

        public static string ServiceAccountId
        {
            get
            {
                return ConfigurationManager.AppSettings[PrmServiceAcctId];
            }
        }

        public static string ServiceAccountPassword
        {
            get
            {
                return ConfigurationManager.AppSettings[PrmServiceAcctPwd];
            }
        }

        public static string AdminAccountId
        {
            get
            {
                return ConfigurationManager.AppSettings[PrmAdminAcctId];
            }
        }

        public static List<string> TablesToCreate
        {
            get
            {
                string tablesString = ConfigurationManager.AppSettings[PrmTablesToCreate];
                var tables = tablesString.Split(',').ToList();
                return tables;
            }
        }

        public static string BaseRequestUri
        {
            get
            {
                return ConfigurationManager.AppSettings[PrmBaseRequestUri];
            }
        }
    }
}
using AWSProjects.CommonUtils;
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
                string encryptedKey = ConfigurationManager.AppSettings[PrmAWSSecretKey];
                return ConfigDecryptionHelper.DecryptConfiguration(encryptedKey);
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
                string encryptedPwd = ConfigurationManager.AppSettings[PrmServiceAcctPwd];
                return ConfigDecryptionHelper.DecryptConfiguration(encryptedPwd);   
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

    static class ConfigDecryptionHelper
    {
        /// <summary>
        /// Decrypts message if the cert is present else returns raw string
        /// </summary>
        /// <param name="msgToDecrypt"></param>
        /// <returns></returns>
        public static string DecryptConfiguration(string msgToDecrypt)
        {
            EncryptionProvider encryptionProvider = new EncryptionProvider(null); 
            string decryptedMsg = msgToDecrypt;
            try
            {
                decryptedMsg = encryptionProvider.Decrypt(msgToDecrypt);
            }
            catch
            {

            }
            return decryptedMsg;
        }
    }
}
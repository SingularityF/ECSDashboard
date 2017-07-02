using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Aliyun
{
    public static class ECSAPI
    {
        public static class UserData
        {
            public static string AccessKeyId;
            public static string Secret;
            public static string SecurityGroupId;
            public static string DefaultPassword;
            public static string DefaultHostName;
            public static string ImageTypeId0;
            public static string ImageTypeId1;
            public static string ImageTypeName0;
            public static string ImageTypeName1;

        };
        public static Dictionary<string, string> dict;
        public enum ImageType { Type0, Type1 };

        public static string RebootInstance(string instanceId)
        {
            dict = new Dictionary<string, string>();
            dict.Add("Action", "RebootInstance");
            dict.Add("InstanceId", instanceId);
            return CalcPublicParams();
        }

        public static string StopInstance(string instanceId)
        {
            dict = new Dictionary<string, string>();
            dict.Add("Action", "StopInstance");
            dict.Add("InstanceId", instanceId);
            return CalcPublicParams();
        }

        public static string DeleteInstance(string instanceId)
        {
            dict = new Dictionary<string, string>();
            dict.Add("Action", "DeleteInstance");
            dict.Add("InstanceId", instanceId);
            return CalcPublicParams();
        }

        public static string ModifyPassword(string instanceId)
        {
            dict = new Dictionary<string, string>();
            dict.Add("Action", "ModifyInstanceAttribute");
            dict.Add("InstanceId", instanceId);
            dict.Add("Password", UserData.DefaultPassword);
            return CalcPublicParams();
        }

        public static string StartInstance(string instanceId)
        {
            dict = new Dictionary<string, string>();
            dict.Add("Action", "StartInstance");
            dict.Add("InstanceId", instanceId);
            return CalcPublicParams();
        }
        public static string AllocateIP(string instanceId)
        {
            dict = new Dictionary<string, string>();
            dict.Add("Action", "AllocatePublicIpAddress");
            dict.Add("InstanceId", instanceId);
            return CalcPublicParams();
        }

        public static string GetInstanceList()
        {
            dict = new Dictionary<string, string>();
            dict.Add("Action", "DescribeInstances");
            dict.Add("RegionId", "cn-hongkong");
            return CalcPublicParams();
        }

        public static string CreateInstance(ImageType imageType)
        {
            dict = new Dictionary<string, string>();
            dict.Add("Action", "CreateInstance");
            dict.Add("RegionId", "cn-hongkong");
            switch (imageType)
            {
                case (ImageType.Type0):
                    dict.Add("ImageId", UserData.ImageTypeId0);
                    break;
                case (ImageType.Type1):
                    dict.Add("ImageId", UserData.ImageTypeId1);
                    break;
            }
            dict.Add("SecurityGroupId", UserData.SecurityGroupId);
            dict.Add("InternetChargeType", "PayByTraffic");
            dict.Add("InternetMaxBandwidthOut", "50");
            dict.Add("IoOptimized", "optimized");
            dict.Add("SystemDisk.Category", "cloud_efficiency");
            dict.Add("HostName", UserData.DefaultHostName);
            dict.Add("InstanceType", "ecs.n1.small");
            return CalcPublicParams();
        }

        public static string CalcPublicParams()
        {
            dict.Add("Version", "2014-05-26");
            dict.Add("AccessKeyId", UserData.AccessKeyId);
            dict.Add("TimeStamp", GetTimestamp());
            dict.Add("SignatureMethod", "HMAC-SHA1");
            dict.Add("SignatureVersion", "1.0");
            dict.Add("SignatureNonce", Guid.NewGuid().ToString());
            dict.Add("Format", "XML");
            var ordered = dict.OrderBy(x => x.Key);
            string request = "";
            foreach (var x in ordered)
            {
                request += x.Key + "=" + Uri.EscapeDataString(x.Value) + "&";
            }
            String str2sign = "GET&%2F&" + Uri.EscapeDataString(request);
            str2sign = str2sign.Substring(0, str2sign.Length - 3);

            var sha = new HMACSHA1(Encoding.UTF8.GetBytes(UserData.Secret+"&"));
            byte[] hashed = sha.ComputeHash(Encoding.UTF8.GetBytes(str2sign));
            string sig = Convert.ToBase64String(hashed);
            dict.Add("Signature", sig);
            request = "https://ecs.aliyuncs.com/?";
            foreach (var x in dict)
            {
                request += x.Key + "=" + Uri.EscapeDataString(x.Value) + "&";
            }
            return request.Substring(0, request.Length - 1);

        }
        public static string GetTimestamp()
        {
            DateTime datetime = DateTime.UtcNow;
            return datetime.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'");
        }
    }
}

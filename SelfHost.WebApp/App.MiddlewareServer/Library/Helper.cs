using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;

namespace App.MiddlewareServer.Library
{
    public static class Helper
    {
        public static Dictionary<string, string> StaticFiles
        {
            get
            {
               return new Dictionary<string, string>
                {
                    { "jpg" , "image/jpg" },
                    { "jpeg" , "image/jpeg" },
                    { "gif" , "image/gif" },
                    { "png" , "image/png" },
                    { "js" , "application/javascript" },
                    { "css" , "text/css" },
                    { "woff" , "font/x-woff" },
                    { "woff2" , "font/x-woff" },
                    { "ttf" , "font/x-woff" },
                    { "map", "text/javascript" }
                };
            }
        }

        public static string[] ImagesAndFonts
        {
            get
            {
                return new []
                {
                    "jpg", "jpeg", "gif", "png","woff","woff2", "ttf", "ico"
                };
            }
        }

        public static string GetApplicationRootPath()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            var fileInfo = new FileInfo(path);
            return $"{fileInfo.Directory.Parent.Parent.Parent.FullName}{ConfigurationManager.AppSettings["Host.App.Mvc.FolderName"].ToString()}";
        }

        public static string GetUrlPath()
        {
            return ConfigurationManager.AppSettings["Host.Url"];
        }

        public static List<KeyValuePair<string, object>> ParseQueryString(string queryString, string regexPattern = null)
        {
            var result = new List<KeyValuePair<string, object>>();
            var regexQueryString = null as string[];

            if (!string.IsNullOrWhiteSpace(regexPattern))
            {
                var regex = new Regex(regexPattern);
                var match = regex.Match(queryString);

                if (match.Success)
                {
                    queryString = queryString.Replace(match.Value, string.Empty);
                    regexQueryString = match.Value.Split(new[] { '=' }, 2);

                    result.Add(new KeyValuePair<string, object>(regexQueryString[0], regexQueryString[1].Substring(0, regexQueryString[1].Length -1)));
                }

            }

            var pairs = queryString.Split('&');

            for (int i = 0; i < pairs.Length; i++)
            {
                var keyValue = pairs[i].Split('=');
                var key = keyValue[0];
                var value = keyValue[1];

                result.Add(new KeyValuePair<string, object>(key, value));
            }

            return result;
        }

        public static bool IsStaticFile(string file)
        {
            var extension = file.Substring(file.LastIndexOf('.') + 1);
            return StaticFiles.ContainsKey(extension);
        }

        public static object[] HandleRequestBody(IOwinContext context, string regexPattern = null)
        {
            var reader = new StreamReader(context.Request.Body).ReadToEnd();
            var requestBody = WebUtility.UrlDecode(reader);

            if (string.IsNullOrWhiteSpace(requestBody))
                return new object[] { };

            var queryString = ParseQueryString(requestBody, regexPattern);
            var result = queryString.Select(x => x.Value).ToArray();
            
            return result;
        }

        public static object[] HandleRequestQueryString(IOwinContext context)
        {
            var queryString = string.Empty;

            if (context.Request.QueryString != null)
                queryString = context.Request.QueryString.Value;

            if (string.IsNullOrWhiteSpace(queryString))
                return new object[] { };

            var result = ParseQueryString(queryString).Select(x => x.Value).ToArray();

            return result;
        }

        public static object[] Cast(this object[] values, ParameterInfo[] parameters )
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].ParameterType == typeof(int) || parameters[i].ParameterType == typeof(int?))
                    values[i] = (int)values[i];
                else if (parameters[i].ParameterType == typeof(bool?) || parameters[i].ParameterType == typeof(bool))
                    values[i] = "true" == (string)values[i];
            }

            return values;
        }
    }
}

using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace Commons
{
    public sealed class ConfigHelper
    {
        /// <summary>
        /// test json xml configuration file support
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConfigFromJson(string key, string file= "app.json")
        {
            var builder = new ConfigurationBuilder()
                         .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile(file);

            IConfiguration Configuration = builder.Build();

            //Console.WriteLine($"{key}={Configuration[key]}");
            return Configuration[key];
        }

        //public  static string GetConfigString(string key)
        //{
        //    var builder = new ConfigurationBuilder()
        //                 .SetBasePath(Directory.GetCurrentDirectory())
        //                .AddJsonFile("app.json");

        //    IConfiguration Configuration = builder.Build();

        //    Console.WriteLine($"option1 = {Configuration["option1"]}");
        //    Console.WriteLine($"option2 = {Configuration["option2"]}");
        //    Console.WriteLine(
        //        $"suboption1 = {Configuration["subsection:suboption1"]}");
        //    Console.WriteLine();

        //    Console.WriteLine("Wizards:");
        //    Console.Write($"{Configuration["wizards:0:Name"]}, ");
        //    Console.WriteLine($"age {Configuration["wizards:0:Age"]}");
        //    Console.Write($"{Configuration["wizards:1:Name"]}, ");
        //    Console.WriteLine($"age {Configuration["wizards:1:Age"]}");
        //    return "";
        //}

        public static bool GetConfigBool(string key)
        {
            bool result = false;
            string configString = ConfigHelper.GetConfigFromJson(key);
            if (configString != null && string.Empty != configString)
            {
                try
                {
                    result = bool.Parse(configString);
                }
                catch (FormatException)
                {
                }
            }
            return result;
        }

        public static decimal GetConfigDecimal(string key)
        {
            decimal result = 0m;
            string configString = ConfigHelper.GetConfigFromJson(key);
            if (configString != null && string.Empty != configString)
            {
                try
                {
                    result = decimal.Parse(configString);
                }
                catch (FormatException)
                {
                }
            }
            return result;
        }

        public static int GetConfigInt(string key)
        {
            int result = 0;
            string configString = ConfigHelper.GetConfigFromJson(key);
            if (configString != null && string.Empty != configString)
            {
                try
                {
                    result = int.Parse(configString);
                }
                catch (FormatException)
                {
                }
            }
            return result;
        }
    }
}

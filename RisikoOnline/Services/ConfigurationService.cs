using System;
using Microsoft.Extensions.Configuration;

namespace RisikoOnline.Services
{
    public class ConfigurationService
    {
        public string ConnectionString { get; }
        public string PasswordSecret { get; }
        public string TokenSigningSecret { get; }
        
        public ConfigurationService(IConfiguration configuration)
        {
            ConnectionString = Get(configuration, "ConnectionString");
            PasswordSecret = Get(configuration, "PasswordSecret");
            TokenSigningSecret = Get(configuration, "TokenSigningSecret");
        }
        
        private static string Get(IConfiguration configuration, string key)
        {
            string value = configuration[key];
            if (string.IsNullOrEmpty(value))
                throw new SystemException($"Configuration entry '{key}' not found");
            return value;
        }
    }
}
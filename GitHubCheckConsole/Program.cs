using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using System.Configuration;

namespace GitHubCheckConsole
{
    class Program
    {
        private const string URL = "https://api.github.com";
        private const string HeaderName = "user-agent";
        private const string HeaderValue = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";
        private readonly static string Auth = ConfigurationManager.AppSettings["OauthToken"];

        static void Main(string[] args)
        {
            Console.Write("User Name: ");
            var input = Console.ReadLine();

            Console.WriteLine("-----------------------------------");
            printRepoStats(input);

            Console.Write("Press any button to exit.");
            Console.ReadKey(true);
        }

        public static GitHubAccount getUser(string request)
        {
            var client = new WebClient();
            client.Headers.Add(HeaderName, HeaderValue);
            var uri = $"{URL}/users/{request}{Auth}";
            var response = client.DownloadString(uri);

            GitHubAccount account = JsonConvert.DeserializeObject<GitHubAccount>(response);

            return account;
        }

        public static List<GitHubRepo> getRepo(string userName)
        {
            var account = getUser(userName);
            var numberOfRepos =  account.public_repos;
            
            var client = new WebClient();
            client.Headers.Add(HeaderName, HeaderValue);

            var uri = $"{URL}/users/{userName}/repos{Auth}";
            var response = client.DownloadString(uri);

            List<GitHubRepo> repos = JsonConvert.DeserializeObject<List<GitHubRepo>>(response);
          
            return repos;
        }

        public static void getLanguage(string userName, int numberOfRepo)
        {
            var repos = getRepo(userName);
            
            var client = new WebClient();
            client.Headers.Add(HeaderName, HeaderValue);

            var repo = repos[numberOfRepo].Name;
            var uri = $"{URL}/repos/{userName}/{repo}/languages{Auth}";
            var response = client.DownloadString(uri);
       
            var dictionaryOfLanguage = JsonConvert.DeserializeObject<Dictionary<string, int>>(response);
            
            var totalValue = 0;
            
            Console.WriteLine("Repo: " + repo);
            
            foreach (KeyValuePair<string, int> entry in dictionaryOfLanguage)
            {
                totalValue += entry.Value;
            }

            foreach (KeyValuePair<string, int> entry in dictionaryOfLanguage)
            {
                var language = entry.Key;
                var languageFrequency = entry.Value;
                var languageFrequencyPercent = (float)languageFrequency / (float)totalValue;
                var languageFrequencyPercentRounded = Math.Round(languageFrequencyPercent, 3) * 100; ;
                Console.WriteLine($"{language}: {languageFrequencyPercentRounded}%");
            }

            Console.WriteLine("-----------------------------------");
        }
        
        public static void printRepoStats(string userName)
        {
            var repos = getRepo(userName);
            var numberOfRepos = repos.Count();

            for (int i = 0; i < numberOfRepos; i++)
            {
                getLanguage(userName, i);
            }
        }
    }
}
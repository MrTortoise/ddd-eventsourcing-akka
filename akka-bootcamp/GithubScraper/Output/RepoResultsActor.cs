using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Akka.Actor;
using GithubScraper.RepoQuerying;
using Newtonsoft.Json;

namespace GithubScraper.Output
{
    /// <summary>
    /// Actor responsible for printing the results and progress from a <see cref="GithubCoordinatorActor"/>
    /// onto a <see cref="RepoResultsForm"/> (runs on the UI thread)
    /// </summary>
    public class RepoResultsActor : ReceiveActor
    {
        public const string Name = "repoResultsActor";
        
        private readonly Action<string, string> _statusUpdate;
        private readonly FileInfo _outputFilePath;

        public static Props CreateProps(Action<string, string> statusUpdater, FileInfo outputFilePath)
        {
            return  Props.Create(()=>new RepoResultsActor(statusUpdater, outputFilePath));
        }

        public RepoResultsActor(Action<string, string> statusUpdate, FileInfo outputFilePath)
        {
            _statusUpdate = statusUpdate;
            _outputFilePath = outputFilePath;
            InitialReceives();
        }

        private void InitialReceives()
        {
            //progress update
            Receive<GithubProgressStats>(stats =>
            {
                if (stats.ExpectedUsers > 0)
                {
                    _statusUpdate(Name, string.Format("{4}%: {0} out of {1} users ({2} failures) [{3} elapsed], ",
                        stats.UsersThusFar, stats.ExpectedUsers, stats.QueryFailures, stats.Elapsed, (stats.UsersThusFar + stats.QueryFailures) / stats.ExpectedUsers));    
                }
                else
                {
                    _statusUpdate(Name, $"No expected users");
                }
                
            });

            //user update
            Receive<IEnumerable<SimilarRepo>>(repos =>
            {
                var similarRepos = repos.Select(r => new RepoData(r));
                var data = JsonConvert.SerializeObject(similarRepos,Formatting.Indented);
                File.WriteAllText(_outputFilePath.FullName, data);
            });

            //critical failure, like not being able to connect to Github
            Receive<GithubCoordinatorActor.JobFailed>(failed =>
            {
                _statusUpdate(Name, string.Format("Failed to gather data for Github repository {0} / {1}", failed.Repo.Owner,
                    failed.Repo.Repo));
            });
        }

        internal class RepoData
        {
            public RepoData(SimilarRepo repo)
            {
                Owner = repo.Repo.Owner.Name;
                Name = repo.Repo.Name;
                HtmlUrl = repo.Repo.HtmlUrl;
                SharedStarrers = repo.SharedStarrers;
                OpenIssuesCount = repo.Repo.OpenIssuesCount;
                StargazersCount = repo.Repo.StargazersCount;
                ForksCount = repo.Repo.ForksCount;
            }

            public string Owner { get; set; }
            public string Name { get; set; }
            public string HtmlUrl { get; set; }
            public int SharedStarrers { get; set; }
            public int OpenIssuesCount { get; set; }
            public int StargazersCount { get; set; }
            public int ForksCount { get; set; }
        }
    }
}
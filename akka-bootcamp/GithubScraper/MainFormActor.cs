using System;
using System.IO;
using System.Linq;
using Akka.Actor;

namespace GithubScraper
{
    /// <summary>
    /// Actor that runs on the UI thread and handles
    /// UI events for <see cref="LauncherForm"/>
    /// </summary>
    public class MainFormActor : ReceiveActor, IWithUnboundedStash, ILogReceive
    {
        public const string Path = "/user/serviceActor/mainform";
        public const string Name = "mainform";
        
        private readonly Action<string, string> _statusUpdater;
        private readonly FileInfo _outputFilePath;

        public static Props CreateProps(Action<string, string> statusUpdater, FileInfo outputFilePath)
        {
            return Props.Create(() => new MainFormActor(statusUpdater, outputFilePath));
        }

        public MainFormActor(Action<string,string> statusUpdater, FileInfo outputFilePath)
        {                       
            _statusUpdater = statusUpdater;
            _outputFilePath = outputFilePath;
            Ready();
        }

        /// <summary>
        /// State for when we're able to accept new jobs
        /// </summary>
        private void Ready()
        {
            Receive<ProcessRepo>(repo =>     
            {
                Context.ActorSelection(GithubValidatorActor.Path)
                    .Tell(new GithubValidatorActor.ValidateRepo(repo.RepoUri));
                BecomeBusy(repo.RepoUri);
            });

            //launch the window
            Receive<LaunchRepoResultsWindow>(window =>
            {
                var resultsActor = Context.ActorOf(
                    RepoResultsActor.CreateProps(_statusUpdater, _outputFilePath));

                _statusUpdater(Name, string.Format("Repos Similar to {0} / {1}", window.Repo.Owner, window.Repo.Repo));
                
                window.Coordinator.Tell(new GithubCoordinatorActor.SubscribeToProgressUpdates(resultsActor));
            });
        }

        /// <summary>
        /// Make any necessary URI updates, then switch our state to busy
        /// </summary>
        private void BecomeBusy(string repoUrl)
        {
            _statusUpdater(Name, string.Format("Validating {0}...", repoUrl));
            Become(Busy);
        }

        /// <summary>
        /// State for when we're currently processing a job
        /// </summary>
        private void Busy()
        {
            Receive<GithubValidatorActor.RepoIsValid>(valid => BecomeReady("Valid!"));
            Receive<GithubValidatorActor.InvalidRepo>(invalid => BecomeReady(invalid.Reason, false));
            //yes
            Receive<GithubCommanderActor.UnableToAcceptJob>(job =>
                BecomeReady(
                    string.Format("{0}/{1} is a valid repo, but system can't accept additional jobs", job.Repo.Owner,
                        job.Repo.Repo), false));

            //no
            Receive<GithubCommanderActor.AbleToAcceptJob>(job =>
                BecomeReady(string.Format("{0}/{1} is a valid repo - starting job!", job.Repo.Owner, job.Repo.Repo)));
            Receive<LaunchRepoResultsWindow>(window => Stash.Stash());
        }

        private void BecomeReady(string message, bool isValid = true)
        {
            _statusUpdater(Name, message);
            Stash.UnstashAll();
            Become(Ready);
        }

        public IStash Stash { get; set; }

        public class LaunchRepoResultsWindow
        {
            public LaunchRepoResultsWindow(RepoKey repo, IActorRef coordinator)
            {
                Repo = repo;
                Coordinator = coordinator;
            }

            public RepoKey Repo { get; }

            public IActorRef Coordinator { get; }
        }
        
        /// <summary>
        /// Begin processing a new Github repository for analysis
        /// </summary>
        public class ProcessRepo
        {
            public ProcessRepo(string repoUri)
            {
                RepoUri = repoUri;
            }

            public string RepoUri { get; private set; }
        }
    }
}
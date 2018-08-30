using System;
using Akka.Actor;
using Octokit;

namespace GithubScraper
{
    /// <summary>
    /// Top-level actor responsible for coordinating and launching repo-processing jobs
    /// </summary>
    public class GithubCommanderActor : ReceiveActor
    {
        private IActorRef _coordinator;
        private IActorRef _canAcceptJobSender;
        public const string Path = "/user/serviceActor/commander";
        public const string Name = "commander";
        
        
        public static Props CreateProps()
        {
            return Props.Create(() => new GithubCommanderActor());
        }

        public GithubCommanderActor()
        {
            Receive<CanAcceptJob>(job =>
            {
                _canAcceptJobSender = Sender;
                _coordinator.Tell(job);
            });

            Receive<UnableToAcceptJob>(job =>
            {
                _canAcceptJobSender.Tell(job);
            });

            Receive<AbleToAcceptJob>(job =>
            {
                _canAcceptJobSender.Tell(job);

                //start processing messages
                _coordinator.Tell(new GithubCoordinatorActor.BeginJob(job.Repo));

                //launch the new window to view results of the processing
                Context.ActorSelection(MainFormActor.Path).Tell(new MainFormActor.LaunchRepoResultsWindow(job.Repo, Sender));
            });
        }

        protected override void PreStart()
        {
            _coordinator = Context.ActorOf(GithubCoordinatorActor.CreateProps(), GithubCoordinatorActor.Name);
            base.PreStart();
        }

        protected override void PreRestart(Exception reason, object message)
        {
            //kill off the old coordinator so we can recreate it from scratch
            _coordinator.Tell(PoisonPill.Instance);
            base.PreRestart(reason, message);
        }
        
        public class CanAcceptJob
        {
            public CanAcceptJob(RepoKey repoKey)
            {
                Repo = repoKey;
            }
            
            public RepoKey Repo { get; }
        }

        public class AbleToAcceptJob
        {
            public AbleToAcceptJob(RepoKey repo)
            {
                Repo = repo;
            }

            public RepoKey Repo { get; }
        }

        public class UnableToAcceptJob
        {
            public UnableToAcceptJob(RepoKey repo)
            {
                Repo = repo;
            }
            
            public RepoKey Repo { get; }
        }

    }
}
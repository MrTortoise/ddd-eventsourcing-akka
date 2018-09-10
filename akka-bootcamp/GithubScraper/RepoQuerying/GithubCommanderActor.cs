using System;
using System.Linq;
using Akka.Actor;
using Akka.Routing;

namespace GithubScraper.RepoQuerying
{
    /// <summary>
    /// Top-level actor responsible for coordinating and launching repo-processing jobs
    /// </summary>
    public class GithubCommanderActor : ReceiveActor, IWithUnboundedStash
    {
        public const string Path = "/user/service/commander";
        public const string Name = "commander";

        private IActorRef _coordinator;
        private IActorRef _canAcceptJobSender;
        private int _pendingJobReplies;

        public IStash Stash { get; set; }

        public static Props CreateProps()
        {
            return Props.Create(() => new GithubCommanderActor());
        }

        public GithubCommanderActor()
        {
            Ready();
        }
        
        private void BecomeReady()
        {
            Become(Ready);
            Stash.UnstashAll();
        }

        private void Ready()
        {
            Receive<CanAcceptJob>(job =>
            {
                _coordinator.Tell(job);
                BecomesAsking();
            });
        }

        private void BecomesAsking()
        {
            _canAcceptJobSender = Sender;
            // block, but ask the router for the number of routees. Avoids magic numbers.
            _pendingJobReplies = _coordinator.Ask<Routees>(new GetRoutees())
                .Result.Members.Count();
            Become(Asking);
        }

        private void Asking()
        {
            Receive<CanAcceptJob>(job => Stash.Stash());
            Receive<UnableToAcceptJob>(job =>
            {
                _pendingJobReplies--;
                if (_pendingJobReplies == 0)
                {
                    _canAcceptJobSender.Tell(job);
                    BecomeReady();
                }
            });
            Receive<AbleToAcceptJob>(job =>
            {
                _canAcceptJobSender.Tell(job);

                //start processing messages
                _coordinator.Tell(new GithubCoordinatorActor.BeginJob(job.Repo));

                //launch the new window to view results of the processing
                Context.ActorSelection(ServiceActor.Path)
                    .Tell(new ServiceActor.StartOutputActorSubscription(job.Repo, Sender));
                
                BecomeReady();
            });

        }

        protected override void PreStart()
        {
            // create a broadcast router who will ask all 
            // of them if they're available for work
            _coordinator =        
                Context.ActorOf(Props.Create(() => new GithubCoordinatorActor())
                        .WithRouter(FromConfig.Instance),
                    GithubCoordinatorActor.Name);
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
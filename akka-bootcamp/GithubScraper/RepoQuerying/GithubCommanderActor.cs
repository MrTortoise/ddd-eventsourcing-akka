using System;
using Akka.Actor;
using Akka.Routing;

namespace GithubScraper.RepoQuerying
{
    /// <summary>
    /// Top-level actor responsible for coordinating and launching repo-processing jobs
    /// </summary>
    public class GithubCommanderActor : ReceiveActor, IWithUnboundedStash
    {
        public const string Path = "/user/serviceActor/commander";
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
            _pendingJobReplies = 3;     
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
            // create three GithubCoordinatorActor instances
            var c1 = Context.ActorOf(GithubCoordinatorActor.CreateProps(),GithubCoordinatorActor.Name + "1");
            var c2 = Context.ActorOf(GithubCoordinatorActor.CreateProps(),GithubCoordinatorActor.Name + "2");
            var c3 = Context.ActorOf(GithubCoordinatorActor.CreateProps(),GithubCoordinatorActor.Name + "3");

            // create a broadcast router who will ask all of them 
            // if they're available for work
            _coordinator =
                Context.ActorOf(Props.Empty.WithRouter(
                    new BroadcastGroup(GithubCoordinatorActor.Path + "1",
                        GithubCoordinatorActor.Path + "2",
                        GithubCoordinatorActor.Path + "3")));
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
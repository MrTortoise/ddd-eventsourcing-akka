using System;
using System.IO;
using Akka.Actor;

namespace GithubScraper
{
    internal class ServiceActor : ReceiveActor, ILogReceive , IWithUnboundedStash
    {
        private readonly Action<string, string> _statusUpdate;
        private readonly FileInfo _outputFilePath;
        private IActorRef _authActor;

        public const string Name = "serviceActor";
        public const string Path = "/user/serviceActor";

        public static Props CreateProps(Action<string, string> authStatusUpdate, FileInfo outputFilePath)
        {
            return Props.Create(() => new ServiceActor(authStatusUpdate, outputFilePath));
        }
        
        public IStash Stash { get; set; }

        public ServiceActor(Action<string, string> statusUpdate, FileInfo outputFilePath)
        {
            _statusUpdate = statusUpdate;
            _outputFilePath = outputFilePath;
            _authActor = Context.ActorOf(AuthenticationActor.CreateProps(statusUpdate),
                AuthenticationActor.Name);

            Receive<Start>(msg =>
            {
                Become(Auth);
            });
        }

        private void Auth()
        {
            Receive<Input>(input => { _authActor.Tell(input); });
                
            Receive<AuthenticationActor.AuthenticationSuccess>(success =>
            {
                BecomeAutenticated("successful authentication");
            });
        }

        private void BecomeAutenticated(string reason)
        {
            _statusUpdate(Name, $"Starting: {reason}");
            _statusUpdate(Name, "Enter url of repo to visit");
            Become(Authenicated);
            Stash.UnstashAll();
        }

        private void Authenicated()
        {
            Receive<Input>(input =>
            {
                var validation = Context.ActorOf(RepositoryValidationActor.CreateProps(GithubClientFactory.GetClient()),
                    RepositoryValidationActor.Name);
                Context.ActorOf(GithubCommanderActor.CreateProps(), GithubCommanderActor.Name);


                var repoUri = input.Text;
                validation.Tell(new RepositoryValidationActor.ValidateRepo(repoUri));
                BecomeBusy(repoUri);
            });

            //launch the window
            Receive<StartOutputActorSubscription>(window =>
            {
                var resultsActor = Context.ActorOf(
                    RepoResultsActor.CreateProps(_statusUpdate, _outputFilePath));

                _statusUpdate(Name, string.Format("Repos Similar to {0} / {1}", window.Repo.Owner, window.Repo.Repo));
                
                window.Coordinator.Tell(new GithubCoordinatorActor.SubscribeToProgressUpdates(resultsActor));
            });
        }
        
        /// <summary>
        /// Make any necessary URI updates, then switch our state to busy
        /// </summary>
        private void BecomeBusy(string repoUrl)
        {
            _statusUpdate(Name, string.Format("Validating {0}...", repoUrl));
            Become(Busy);
        }

        /// <summary>
        /// State for when we're currently processing a job
        /// </summary>
        private void Busy()
        {
            Receive<RepositoryValidationActor.RepoIsValid>(valid => BecomeAutenticated("Valid!"));
            Receive<RepositoryValidationActor.InvalidRepo>(invalid => BecomeAutenticated(invalid.Reason));
            //yes
            Receive<GithubCommanderActor.UnableToAcceptJob>(job =>
                BecomeAutenticated(
                    string.Format("{0}/{1} is a valid repo, but system can't accept additional jobs", job.Repo.Owner,
                        job.Repo.Repo)));

            //no
            Receive<GithubCommanderActor.AbleToAcceptJob>(job =>
                BecomeAutenticated(string.Format("{0}/{1} is a valid repo - starting job!", job.Repo.Owner, job.Repo.Repo)));
            Receive<StartOutputActorSubscription>(window => Stash.Stash());
        }


        public class Start
        {
        }

        public class StartOutputActorSubscription
        {
            public StartOutputActorSubscription(RepoKey repo, IActorRef coordinator)
            {
                Repo = repo;
                Coordinator = coordinator;
            }

            public RepoKey Repo { get; }

            public IActorRef Coordinator { get; }
        }
    }
}
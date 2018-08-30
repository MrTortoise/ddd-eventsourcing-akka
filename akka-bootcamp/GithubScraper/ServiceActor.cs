using System;
using System.IO;
using Akka.Actor;

namespace GithubScraper
{
    internal class ServiceActor : ReceiveActor, ILogReceive
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

        public ServiceActor(Action<string, string> statusUpdate, FileInfo outputFilePath)
        {
            _statusUpdate = statusUpdate;
            _outputFilePath = outputFilePath;
            _authActor = Context.ActorOf(GithubAuthenticationActor.CreateProps(statusUpdate),
                GithubAuthenticationActor.Name);

            Receive<Start>(msg =>
            {
                Become(Auth);
//                _authActor.Tell(new GithubAuthenticationActor.Authenticate(tbOAuth.Text));
            });
        }

        private void Auth()
        {
            Receive<Input>(input => { _authActor.Tell(input); });
                
            Receive<GithubAuthenticationActor.AuthenticationSuccess>(success =>
            {
                BecomeAutenticated();
            });
        }

        private void BecomeAutenticated()
        {
            _statusUpdate(Name, "Enter url of repo to visit");
            Become(Authenicated);

        }

        private void Authenicated()
        {
            Receive<Input>(input =>
            {
                var mainFormActor = Context.ActorOf(MainFormActor.CreateProps(_statusUpdate, _outputFilePath),
                    MainFormActor.Name);
                Context.ActorOf(GithubValidatorActor.CreateProps(GithubClientFactory.GetClient()),
                    GithubValidatorActor.Name);
                Context.ActorOf(GithubCommanderActor.CreateProps(), GithubCommanderActor.Name);

                mainFormActor.Tell(new MainFormActor.ProcessRepo(input.Text));
            });
        }

        public class Start
        {
        }
    }
}
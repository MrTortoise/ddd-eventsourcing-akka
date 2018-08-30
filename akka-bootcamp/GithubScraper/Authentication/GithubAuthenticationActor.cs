using System;
using Akka.Actor;
using Octokit;

namespace GithubScraper
{
    public class GithubAuthenticationActor : ReceiveActor, ILogReceive
    {
        public const string Name = "authenticator";
        public const string Path = "/user/serviceActor/authenticator";

        private readonly Action<string, string> _statusUpdate;
        private IActorRef _service;

        public static Props CreateProps(Action<string, string> statusUpdater)
        {
            return Props.Create(() => new GithubAuthenticationActor(statusUpdater));
        }

        public GithubAuthenticationActor(Action<string, string> statusUpdate)
        {
            _statusUpdate = statusUpdate;
            BecomeUnauthenticated("just starting");
        }

        private void BecomeUnauthenticated(string reason)
        {
            _statusUpdate(Name, $"Authentication failed. Please try again. reason: {reason}");
            Become(Unauthenticated);
        }

        private void Unauthenticated()
        {
            _statusUpdate(Name, "Enter Oauth ID (ie github key)");
            Receive<Input>(input =>
            {
                _service = Sender;
                Self.Tell(new Authenticate(input.Text));
            });

            Receive<Authenticate>(auth =>
            {
                //need a client to test our credentials with
                var client = GithubClientFactory.GetUnauthenticatedClient();
                GithubClientFactory.OAuthToken = auth.OAuthToken;
                client.Credentials = new Credentials(auth.OAuthToken);
                BecomeAuthenticating();
                client.User.Current().ContinueWith<object>(tr =>
                {
                    if (tr.IsFaulted)
                        return new AuthenticationFailed();
                    if (tr.IsCanceled)
                        return new AuthenticationCancelled();
                    return new AuthenticationSuccess();
                }).PipeTo(Self);
            });
        }

        private void BecomeAuthenticating()
        {
            _statusUpdate(Name, "Authenticating...");
            Become(Authenticating);
        }


        private void Authenticating()
        {
            Receive<AuthenticationFailed>(failed => BecomeUnauthenticated("Authentication failed."));
            Receive<AuthenticationCancelled>(cancelled => BecomeUnauthenticated("Authentication timed out."));
            Receive<AuthenticationSuccess>(success =>
            {
                _statusUpdate(Name, "Authenticated");
                _service.Tell(success);
            });
        }


        public class Authenticate
        {
            public Authenticate(string oAuthToken)
            {
                OAuthToken = oAuthToken;
            }

            public string OAuthToken { get; private set; }
        }

        public class AuthenticationFailed
        {
        }

        public class AuthenticationCancelled
        {
        }

        public class AuthenticationSuccess
        {
        }
    }
}
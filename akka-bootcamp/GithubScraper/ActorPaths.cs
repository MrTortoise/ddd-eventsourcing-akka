﻿using System;
using Akka.Actor;

namespace GithubScraper
{
    /// <summary>
    /// Static helper class used to define paths to fixed-name actors
    /// (helps eliminate errors when using <see cref="ActorSelection"/>)
    /// </summary>
    [Obsolete("dont like it")]
    public static class ActorPaths
    {
        public static readonly ActorMetaData GithubAuthenticatorActor = new ActorMetaData("authenticator", "akka://GithubActors/user/authenticator");
        public static readonly ActorMetaData MainFormActor = new ActorMetaData("mainform", "akka://GithubActors/user/mainform");
        public static readonly ActorMetaData GithubValidatorActor = new ActorMetaData("validator", "akka://GithubActors/user/validator");
        public static readonly ActorMetaData GithubCommanderActor = new ActorMetaData("commander", "akka://GithubActors/user/commander");
        public static readonly ActorMetaData GithubCoordinatorActor = new ActorMetaData("coordinator", "akka://GithubActors/user/commander/coordinator");
        
        /// <summary>
        /// Meta-data class
        /// </summary>
        public class ActorMetaData
        {
            public ActorMetaData(string name, string path)
            {
                Name = name;
                Path = path;
            }

            public string Name { get; } 

            public string Path { get; }
        }
    }
}
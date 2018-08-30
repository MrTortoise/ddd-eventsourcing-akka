# Akka demonstration

This project is a rewrite of [the petabridge akka bootcamp](https://github.com/petabridge/akka-bootcamp)

I had to make some changes as this is a .net core project. 
So rather than use winforms I rewrote it to be console driven.

A few things got added such as logging.
Also ended up adding a service actor to orchestrate the parts - and pass input to the active actors using become

## Things to try

1. Try throwing an exception in the `Authenticated` handler in 
[GithubAuthenticationActor](./GithubAuthenticationActor.cs) - Notice how the supervisor restarts the actor and we simply get asked to log in again.
2.

## Authentication Failed
``` mermaid
sequenceDiagram
  participant user
  participant service
  participant authentication
  participant github client

  service->>service: Become Authenticating
  service->>authentication: Create Authentication Actor
  authentication->>authentication: Become Unauthenticated
  user ->> service: github auth details (as Input)
  service->>authentication: Input
  authentication->>authentication: Become Authenticating
  authentication->>github client: Attempt to auth
  github client->> authentication: Authentication Failed / Authentication Cancelled
  authentication ->> authentication: Become Unauthenticated  
```

## Invalid Repo
``` mermaid
sequenceDiagram
  participant user
  participant service
  participant authentication
  participant mainform
  participant validator
  participant commander
  participant github client

  service->>service: Become Authenticating
  service->>authentication: Create Authentication Actor
  authentication->>authentication: Become Unauthenticated
  user ->> service: github auth details (as Input)
  service->>authentication: Input
  authentication->>authentication: Become Authenticating
  authentication->>github client: Attempt to auth
  github client->> authentication: Authentication Successful
  authentication ->> service: Authentication Successful
  service ->> service: Become Authenticated
  user ->> service: github repo to visit
  service ->> mainform: create
  mainform ->> mainform: Become Ready
  service ->> validator: create
  service ->> commander: create
  service ->> mainform: processRepo(input)
  mainform ->> validator: validate repo
  mainform->>mainform: Become busy
  validator->>github client: validate repo
  github client->>validator: boom
  validator->>validator: invalid repo
  validator ->> mainform: invalid repo
  mainform ->> mainform: Become Ready

```

## Processing a repo
``` mermaid
sequenceDiagram
  participant user
  participant service
  participant authentication
  participant mainform
  participant validator
  participant commander
  participant coordinator
  participant scheduler
  participant worker
  participant results
  participant github client
  participant output

  service ->> service: Become Authenticated
  user ->> service: github repo to visit
  service ->> mainform: create
  mainform ->> mainform: Become Ready
  service ->> validator: create
  service ->> commander: create
  commander->> coordinator: create
  coordinator ->> coordinator: Become Waiting
  service ->> mainform: processRepo(input)
  mainform ->> validator: validate repo
  mainform->>mainform:Become busy
  validator ->> github client: Get repo
  github client ->> validator: Valid repo
  validator->>validator: Repository
  validator ->> commander: CanAcceptJob
  commander ->> coordinator: CanAcceptJob
  coordinator->>commander: AbleToAcceptJob
  commander->>validator: AbleToAcceptJob
  validator->>mainform: AbleToacceptJob
  mainform->>mainform: Become Ready
  commander->>coordinator: Begin Job
  coordinator->>coordinator: Become Working
  coordinator->>worker: Query Starers
  worker ->> github client: get all users that starred a repo
  github client ->> worker: users
  worker->>coordinator: Starers of a repo (users)
  coordinator->>worker: FOR EACH USER: Query starrer (user)
  worker->github client: Get all repos
  github client->worker: repos



  commander->>mainform: launch results window
  mainform->> coordinator: SubscribeToProgressUpdates
  coordinator->>scheduler: Send PublishUpdate to Self every time period
  scheduler->>coordinator: publish update
  coordinator-->output: data


```

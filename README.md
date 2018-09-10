# ddd-eventsourcing-akka
Source material for a talk

Akka.net quickstart

## install Akka

package-install akka (or however you do it in .net core - I use Rider so have not found out yet)

## How to create an actor system?


```csharp
var cfg = ConfigurationFactory.ParseString(File.ReadAllText("config.hocon"));
var system = ActorSystem.Create("GitHubScraper", cfg);
```

Thats it.

Whats that config.hocon? 
Well you can leave the `cfg` out to begin with. I just wanted to demonstrate how easy it is to load a config file.

## How to create an Actor?
The simplest way
```csharp
var actor = system.ActorOf<DaveActor>();
```
But most of the time you will have some dependencies.
Then you generate properties.
```csharp
var someDependency = new SomeDependency(somethingElse, andAnotherThing);
var myActorReference = system.ActorOf(Props.Create(()=> new ActorInstance(someDependency)), ActorInstance.Name);
```
That looks complex?
Well akka.net is hooking it up and registering it into the framework.
The props works basically like props works in a ton of other frameworks (React, Vue, Elixir) - think of it as DI in a world where you cannot new up things yourself. Its short for properties, it just wraps them up in a form of partial application and passes them through.

Partial application is a functional concept and is basically the equivalent of a constructor where you pass in fields. Very simple idea, but terse.
 
## How to Define an Actor?

```csharp
public class AnActor: ReceiveActor
{
    public const string Name = "anActor";
    public static Props(Some dependency)
    {
        return Props.Create(()=>new AnActor(dependency))
    }

    public AnActor(Some dependency)
    {
        Receive<Message>(msg=>{
            var actorThatSentMessage = Sender;  // Some examples of helpful properties you gain access to.
            var thisActorRef = Self;
            ... do something
        })
    }
}
```

I prefer this pattern. It puts the responsibility for constructing the actor from its dependencies inside itself rather than the caller.

The caller gets to
1. name it 
1. give it a location in the actor system as a result and alIt leaves the code that will create it to decide what to call it
1. how to hook up with routers.


## How To Send a message to an Actor?
if you create one, you can just tell it something
```csharp
var actorRef = system.ActorOf(AnActor.Props(someDependency), AnActor.Name)
actorRef.Tell(new Message());
```

If you cannot have a direct reference (Eg the actor is not known 100% to be local) then you can use actor selection from the context of the current actor to look up a reference via an address

```csharp
 var actor = Context.ActorSelection(GithubCommanderActor.Path)
```

I tend to put the path as a `const string` in the actor. Some people maintain a paths file (much like a routes file in mvc) - I hate groupings by type remember.

My folder structures in a project will often (by coincidence ...) map onto the actor heirachy im building.

## How to deal with a long running bit of code

Ideally we do not want to block actors, so `async await` is not a good thing the best way is to pipe the reult of an async operation back into the actor as a message

```csharp
// lets say we have a long runnign task - EG database / web call
ResultOfLongRunningTask PerformSomeLongRunningTask(Message msg){
    return Task.Run(()=> {
        Thread.Sleep(tillTheSunDies);
        return new ResultOfLongRunningTask();
    });
}

Receive<Message>(msg=>{
  PerformSomeLongRunningTask(msg).PipeTo(Self);
});

Receive<ResultOfLongRunningTask>(result=>{
   _someOtherActor.Tell(new Yay(result));
});
```

**This allows the actor to handle other messages whilst the work is going on**

That is basically everything you need to get going with akka.net


# Want to do something more fun?

State machine?

```csharp 
public class LookMahAStateMachine : ReceiveActor
{
    public LookMahAStateMachine(){
        Become(Waiting)
    }

    public void Waiting()
    {
        Receive<Start>(msg=>{
            Become(Busy);
            Sender.Tell(new DoingWorkYo());
            _sender = Sender;
            SomeFunctionThatReturnsDoingWork().PipeTo(Self);
        });
    }

    public void Busy()
    {
        Recive<Start>(msg=> Stash.Stash()); // this stashes the message so that when it is waiting it can consume it laster - purley optional a you can choose to let the mesage goto dead letters.

        Recieve<DoingWork>(msg=>{
            /*  do some work 
            ... maybe pass reference to Self to another
            ... actor and let it Tell WorkCompleted 
            */
           Self.Tell(new WorkCompleted(msg));     
        });

        Receive<WorkCompleted>(msg=>{
            _sender.Tell(msg);
            Become(Waiting);
            Stash.UnstashAll();
        });
    }
}

```

# Want to possibly put a router onto an actor to enable some kind of fan out (or a behavioural wrapper - think attribute)

```csharp
  _coordinator =        
                Context.ActorOf(Props.Create(() => new GithubCoordinatorActor())
                        .WithRouter(FromConfig.Instance),
```

The config file might look like
```hocon
akka.actor.deployment{
  /service/commander/coordinator{
    router = broadcast-pool
    nr-of-instances = 3
  }     
}   
```




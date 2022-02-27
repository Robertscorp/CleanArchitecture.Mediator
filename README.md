# Clean Architecture Mediator

### What is Clean Architecture Mediator?

Clean Architecture Mediator is designed to be the right tool for building Clean Architecture Use Cases. There are other, more generic, mediators out there, but their limitations become apparent when building use cases. The generic tools are great, but they aren't designed to handle output ports and flow of control.

### Why are Output Ports and Flow of Control important?
Great question.

#### Setting the Stage
Let's say we're building a system, and we know the use cases in our system are going to have potentially common behaviour - authorisation, input port validation, and an interactor. Our use cases will have some combination of those 3 behaviours, but which combination?

#### Well, what are people doing with the generic tools?
More generic tools simply have an input and an output. Examples around tend to have the response as the use case's "success" response, and any other outputs are implemented as exceptions to that pipeline. The user was unauthorised? That's an authorisation exception. The input was invalid? That's a validation exception. While this may functionally solve the problem, it's impractical _for the consumers of the use case_. Does this specific use case have validation? Does it have authorisation? Is there any way to know other than looking over the use case code? Are we going to add those outputs to the use case in the future? That one's not ideal - if we implement them and don't catch every exception we've got an application failure on our hands, and our Users are the ones who are going to feel it.

#### Ok, what can we do about that?
That's what why we have an Output Port - an Output Port is an interface that defines _all possible outputs of a specific use case_. This will let the consumers of the use case know exactly what can be outputted from the use case. Does this use case have authorisation? If it does, the Output Port will have a method for that. Does this use case have input validation? If it does, the Output Port will have a method for that.

#### But how is it different?
Since the Output Port is an interface and dependency injected into the use case, every consumer of the use case will need to implement the Output Port and define exactly what to do for each method that exists. The consumer will know _exactly_ which outputs exist on the use case, since every possible output is defined on the Output Port. What's even better? If we add a new output to the use case, the Output Port will define it - and then every implementation of that Output Port _will not compile_ until it implements it. No surprises!

#### Sounds great, but what's this about Flow of Control?
Use Cases are very similar to Enums - they have both state and related behaviour. We all know how Enums wind up working in our code - the state of the Enum determines some related behaviour. If enum value is a, do x. If enum value is b, do y. As a once-off, this doesn't hurt us too bad. It doesn't even hurt too bad when we copy and paste that code into every place we want that behaviour. What hurts us is when we need to add enum value c. Now we need to perform shotgun surgery - track down every little pellet in our code base that is this state-to-behaviour relationship - and modify it to include "if enum value is c, do z". It's really not ideal when we miss one - hopefully our state-to-behaviour code handles previously undefined values in a way that isn't too painful for our Users! This is why the Smart Enum pattern exists - it pushes the behaviour into the Smart Enum, such that the Flow of Control goes into it, instead of the consumer of it.

#### That's great about Enums, but how's it related to Use Cases?
Without proper Flow of Control, our use cases will behave exactly like an Enum in our code. More generic tools simply return a state - either as a successful response, or by throwing some exception - and then leave it up to the consumers of that use case to interpret that state and then invoke some behaviour. If exception a, do x. If exception b, do y. If success, do z. Rinse and repeat in every consumer of this use case. It's much nicer when the consumer doesn't need to care about state, only behaviour. With the Output Port, the state-to-behaviour relationship is handled in the use case - when the state is encountered, the method on the output port (behaviour) is invoked. A consumer doesn't need to care _if_ a use case input port is invalid, it only needs to handle what to do _when_ a use case input port is invalid.

#### So to sum it up?
What's so great about Output Ports and Flow of Control is we no longer need to care about the _what ifs_ of a use case, only the _whens_. 

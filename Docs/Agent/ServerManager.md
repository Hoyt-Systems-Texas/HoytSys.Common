# Server Manager

Manages the servers and responsible for coordinating between the workers.

Responsible for the following things:

* Client Connections - Keeping track of the location of the client connections.
* Worker Agent Assignment - Which worker has the specified agent.
* Worker Status - The status of the server.
* Client Worker Assignment - The server the client is assigned to.

Need to design a total of 4 replicating state machines and a raft implementation or find one that will work for what I'm trying to do.

# Worker

A worker is used to process requests for a client.  Server manager is used to maintain the location of the clients.

# Client

Used to talk to a client like a web browser.  Responsible for allocating the server.

Responsible for getting assigned to a worker and sending heartbeats to the server manager.
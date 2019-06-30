# Distributed Agent

Goal is to have a distributed agent which is fast and has at most 1 guarantee which requires a consensus algorithm.

## Raft

Raft is the first choice for replicating which server currently owns the data.  Need to design a state machine so we can assign the agent to the server.

## State Machine

The state machine needs to be relatively simple. The goal is to have everything in memory so it's extremely fast.  Would require have a ledger so when a node is added we can get a snapshot of the events.

## Agent Raft States

The raft states are different than the states on the server.  Each server also has it's own states for joing the cluster.

### States

* Inactive - None of the server has yet requested the agent.
* Assigned - The agent has been assigned to a server.
* Unassigned - The agent currently isn't assigned to any server.

### Events

* Requesting - An agent has been requested.
* Timeout - The server assigned to the agent has timed out.

## Agent Requesting States

Represents the agent states on the server.

### State

* Pending - The agent has been requested to be assigned.
* Assigned - The agent is currently assigned to the system.
* Unassigned - The server no longer has the agents.

### Events

* Requesting - The agent has been requested.
* RequestTimedOut - The agent request has timed out.
* Release - The agent has been requested to be released.

## Server States

The server needs to have separate states to detect connection lost so it will no longer use any of the agents.  This could prove to be difficult and deciding on a timeout factor may be hard.  Need to find a time to expect to receive a heartbeat and renew the agents.

### States

* Inactive - The server is currently inactive and not accepting request.
* Active - The server is active.
* ConnectionLost - The server connection has been lost and needs to be reestablished. 

### Events

* Started - The server is starting up.
* Renew - The server has been renewed.
* TimedOut - The server has timeout and can no longer accept anymore messages.
* Reconnect - Attempting to reconnect to the server.

## Potential Issues

Need to figure out how to design it where we don't have a potential timing issue.
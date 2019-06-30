# Clustering

Would like to have an ultra fast clustering algorithm for distribute agents.

## Communication

Can use NetMq with a binary protocol for best performance.  Might be able to use a very simple one with minimum encoding/decoding.

## Synchronization

Raft would be the most logical choice for synchronization.  There is an issue making sure the server that has the current implementation releases it, which in itself would be another state machine.  Would likely require multiple services for managing the agents and would be easier to code.
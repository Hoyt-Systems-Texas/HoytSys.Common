# NetMq

NetMq is a .Net implementation of ZeroMq and provides sockets.  It is pretty fast being able to send messages at over 100k a second which is way faster than signalr.

## Push/Pull Sockets

The protocol we will mainly use is the push pull sockets since we are broadcasting.  Need to also be able to handle routing which means we need a service to coordinate.

* Create our on registration service to keep track.
* See if it's possible to use one that existing applications like Zoo Keep to coordinate.
* Just work on an implementation for single services.
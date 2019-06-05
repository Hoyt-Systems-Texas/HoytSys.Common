# Database Diff Design

Only support immutable design.  Only needs to update for immutability.  Only supports simple updates not mass updates for easy of use.  However this could change if we need higher throughput for updating massive amounts of data.

## Support Batching

Do we want this one to support batching?  Not really sure at this time but it could be useful if we are going to read in large amounts of data.  Would like to not have the collections since that is overly complicated and only necessary if we are doing massive database updates and we don't want to have the duplicate records which I don't know if we want that this time.

## Updating

Would need to be some type of recursive data structure for doing the update that includes the repository.  Ideally we would be able to do everything in a batch so we don't have to do more than the minimum amount of database calls.  Not sure if this really possible or not.
namespace A19.Database.Diff
{
    public interface IUpdateManyToOne<
        TNew,
        TDb,
        TKey,
        TUserId> :
        IUpdateRecordValue<TNew, TDb, TKey, TUserId>,
        IDbUpdateNode,
        IBuildDependencyGraph
        where TDb : AbstractDatabaseRecord<TKey, TNew>
    {
    }
}
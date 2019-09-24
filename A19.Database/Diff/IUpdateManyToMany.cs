namespace A19.Database.Diff
{
    public interface IUpdateManyToMany<TNew, TDb, TKey, TUserId> :
        IUpdateRecordValue<TNew, TDb, TKey, TUserId>,
        IBuildDependencyGraph,
        IDbUpdateNode
        where TDb : AbstractDatabaseRecord<TKey, TNew>
    {
    }
}
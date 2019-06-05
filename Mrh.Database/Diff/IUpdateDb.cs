namespace Mrh.Database.Diff
{
    public interface IUpdateDb
    {

        /// <summary>
        ///     Called when to update the many-to-many records.
        /// </summary>
        void ManyToMany();

        /// <summary>
        ///     Called when to update the many to one record.
        /// </summary>
        void ManyToOne();

        /// <summary>
        ///     Used to update the records in the database.
        /// </summary>
        void Update();
    }
}
namespace A19.User.Common
{
    public class SystemLoginRq
    {
        /// <summary>
        ///     The system that's trying to login as.
        /// </summary>
        public int SystemId { get; set; }
        
        /// <summary>
        ///     The id of the system it's trying to access.
        /// </summary>
        public int AccessingSystemId { get; set; }
        
        /// <summary>
        ///     The pass code for the system.
        /// </summary>
        public string PassCode { get; set; }
    }
}
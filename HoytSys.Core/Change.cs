namespace A19.Core
{
    /// <summary>
    ///      The base change type.
    /// </summary>
    /// <typeparam name="TValue">The internal value</typeparam>
    public class Change<TKey, TValue>
    {
        private Change(
            ChangeState changeState,
            TKey id,
            TValue value)
        {
            this.CurrentState = changeState;
            this.Id = id;
            this.Value = value;
        }
        
        public TKey Id { get; private set; }
        
        public TValue Value { get; private set; }
        
        public ChangeState CurrentState { get; private set; }

        public static Change<TKey, TValue> Unset(TValue initialValue)
        {
            return new Change<TKey, TValue>(
                ChangeState.None,
                default,
                initialValue);
        }
        
        public static Change<TKey, TValue> NewValue(TValue initialValue)
        {
            return new Change<TKey, TValue>(
                ChangeState.New,
                default,
                initialValue);
        }

        public static Change<TKey, TValue> Existing(TValue initialValue, TKey id)
        {
            return new Change<TKey, TValue>(
                ChangeState.Clean,
                id,
                initialValue);
        }

        public void Update(TValue model)
        {
            this.Value = model;
            this.UpdatedInt();
        }

        public TValue UpdateMut()
        {
            this.UpdatedInt();
            return this.Value;
        }
        
        public void Delete()
        {
            switch (this.CurrentState)
            {
                case ChangeState.New:
                    this.CurrentState = ChangeState.Abandon;
                    break;

                case ChangeState.None:
                    break;
                
                case ChangeState.Clean:
                case ChangeState.Dirty:
                    this.CurrentState = ChangeState.Deleted;
                    break;
                
                case ChangeState.Deleted:
                case ChangeState.Abandon:
                    break;
            }
        }

        public bool RequiredUpdate
        {
            get
            {
                switch (this.CurrentState)
                {
                    case ChangeState.New:
                    case ChangeState.Deleted:
                    case ChangeState.Dirty:
                        return true;

                    case ChangeState.Abandon:
                    case ChangeState.None:
                    case ChangeState.Clean:
                        return false;

                    default:
                        return false;
                }
            }
        }

        public bool Active
        {
            get
            {
                switch (this.CurrentState)
                {
                    case ChangeState.New:
                    case ChangeState.Clean:
                    case ChangeState.Dirty:
                        return true;
                    
                    case ChangeState.Abandon:
                    case ChangeState.Deleted:
                    case ChangeState.None:
                        return false;
                    
                    default:
                        return false;
                }
            }
        }
        
        private void UpdatedInt()
        {
            switch (this.CurrentState)
            {
                case ChangeState.New:
                case ChangeState.Dirty:
                    break;
                
                case ChangeState.None:
                case ChangeState.Abandon:
                    this.CurrentState = ChangeState.New;
                    break;
                
                case ChangeState.Deleted:
                case ChangeState.Clean:
                    this.CurrentState = ChangeState.Dirty;
                    break;
            }
        }
    }
}
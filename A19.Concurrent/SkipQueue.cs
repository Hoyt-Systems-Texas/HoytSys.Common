namespace A19.Concurrent
{
    public class SkipQueue<TNode> where TNode : class
    {
        private CurrentQueue currentQueue;
        private readonly MpmcRingBuffer<TNode> primary;

        /// <summary>
        ///     For now just use the current queue. It maybe better later to just switch to a ring buffer and just
        /// overwrite older events.
        /// </summary>
        private readonly MpmcRingBuffer<TNode> defer;

        private long deferStop = 0;

        public SkipQueue(uint size = 32)
        {
            this.primary = new MpmcRingBuffer<TNode>(size);
            this.defer = new MpmcRingBuffer<TNode>(size / 2);
            this.currentQueue = CurrentQueue.NormalQueue;
        }

        public bool Next(out TNode node)
        {
            if (this.currentQueue == CurrentQueue.Defer)
            {
                if (this.deferStop >= this.defer.ConsumerIndex)
                {
                    this.currentQueue = CurrentQueue.NormalQueue;
                }
                else
                {
                    if (!this.defer.TryPoll(out node))
                    {
                        this.currentQueue = CurrentQueue.NormalQueue;
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            return this.primary.TryPoll(out node);
        }

        public bool Add(TNode node)
        {
            return this.primary.Offer(node);
        }

        public bool AddDefer(TNode node)
        {
            return this.defer.Offer(node);
        }

        public void Reset()
        {
            if (this.defer.TryPeek(out var p))
            {
                this.currentQueue = CurrentQueue.Defer;
                this.deferStop = this.defer.ProducerIndex;
            }
            else
            {
                this.currentQueue = CurrentQueue.NormalQueue;
            }
        }

        public enum CurrentQueue
        {
            Defer,
            NormalQueue
        }
    }
}
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.Serialization;

namespace Microsoft.StreamProcessing
{
    [DataContract]
    internal sealed class FStartPipe<TPayload> : IStreamObserver<Empty, TPayload>
    {
        private BlockingCollection<StreamMessage<Empty, TPayload>> _queue;
        internal FInputOperation<TPayload> _iop;
        internal long Period;
        internal long Offset;

        public FStartPipe(long period, long offset)
        {
            Period = period;
            Offset = offset;
            _iop = new FInputOperation<TPayload>(period, offset);
            _queue = _iop.GetInputQueue();
        }

        public int CurrentlyBufferedOutputCount
        {
            get { return 0; }
        }

        public int CurrentlyBufferedInputCount
        {
            get { return 0; }
        }

        public void ProduceQueryPlan(PlanNode previous)
        {
            throw new NotImplementedException();
        }

        public void OnFlush()
        {
            _queue.CompleteAdding();
        }

        public void Checkpoint(Stream stream)
        {
            throw new NotImplementedException();
        }

        public void Restore(Stream stream)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            _queue.CompleteAdding();
        }

        public void OnNext(StreamMessage<Empty, TPayload> batch)
        {
            this._queue.Add(batch);
        }

        public void OnCompleted()
        {
            _queue.CompleteAdding();
        }

        public FOperation<TPayload> GetFOP()
        {
            return _iop;
        }

        public Guid ClassId
        {
            get { return Guid.Parse("FStartPipe"); }
        }
    }
}
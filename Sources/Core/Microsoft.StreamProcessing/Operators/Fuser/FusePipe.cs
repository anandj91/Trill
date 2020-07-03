using System;
using System.Runtime.Serialization;
using Microsoft.StreamProcessing.Internal;
using Microsoft.StreamProcessing.Internal.Collections;

namespace Microsoft.StreamProcessing
{
    [DataContract]
    internal sealed class FusePipe<TPayload, TState, TResult> : UnaryPipe<Empty, TPayload, TResult>
    {
        [SchemaSerialization] private Func<InputBStream<Empty, TPayload>, BStreamable<TState, TResult>> Transform;
        private readonly MemoryPool<Empty, TResult> pool;
        private InputBStream<Empty, TPayload> input;
        private BStreamable<TState, TResult> outStream;

        [DataMember] private StreamMessage<Empty, TResult> output;

        [Obsolete("Used only by serialization. Do not call directly.")]
        public FusePipe()
        {
        }

        public FusePipe(FuseStreamable<TPayload, TState, TResult> stream, IStreamObserver<Empty, TResult> observer)
            : base(stream, observer)
        {
            Transform = stream.Transform;
            this.pool = MemoryManager.GetMemoryPool<Empty, TResult>(stream.Properties.IsColumnar);
            this.pool.Get(out this.output);
            this.output.Allocate();
            input = new InputBStream<Empty, TPayload>(stream.Period, stream.Offset);
            outStream = Transform(input);
        }

        public override void ProduceQueryPlan(PlanNode previous)
            => this.Observer.ProduceQueryPlan(
                new FusePlanNode(previous, this, typeof(Empty), typeof(TPayload), typeof(TResult))
            );

        public override void OnNext(StreamMessage<Empty, TPayload> batch)
        {
            input.Batch = batch;

            var state = outStream.Init();
            while (!outStream.IsDone(state))
            {
                AddToBatch(
                    outStream.GetSyncTime(state), outStream.GetOtherTime(state), default,
                    outStream.GetPayload(state), outStream.GetHash(state));
                state = outStream.Next(state);
            }

            batch.payload.Return();
            batch.Return();
            FlushContents();
        }

        private void AddToBatch(long start, long end, Empty key, TResult payload, int hash)
        {
            int index = this.output.Count++;
            this.output.vsync.col[index] = start;
            this.output.vother.col[index] = end;
            this.output.key.col[index] = key;
            this.output.payload.col[index] = payload;
            this.output.hash.col[index] = hash;
            if (end == StreamEvent.PunctuationOtherTime)
                this.output.bitvector.col[index >> 6] |= (1L << (index & 0x3f));

            if (this.output.Count == Config.DataBatchSize) FlushContents();
        }

        protected override void FlushContents()
        {
            if (this.output.Count == 0) return;
            this.Observer.OnNext(this.output);
            this.pool.Get(out this.output);
            this.output.Allocate();
        }

        public override int CurrentlyBufferedOutputCount => 0;

        public override int CurrentlyBufferedInputCount => 0;
    }
}
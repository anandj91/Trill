using System;
using System.Runtime.Serialization;
using Microsoft.StreamProcessing.Internal;
using Microsoft.StreamProcessing.Internal.Collections;

namespace Microsoft.StreamProcessing
{
    [DataContract]
    internal sealed class FusePipe<TPayload, TResult> : UnaryPipe<Empty, TPayload, TResult>
    {
        [SchemaSerialization] private Func<InputBStream<TPayload>, BStreamable<TResult>> Transform;
        private readonly MemoryPool<Empty, TResult> pool;
        private BStreamable<TResult> bstream;
        private BState bstate;

        [DataMember] private StreamMessage<Empty, TResult> output;

        [Obsolete("Used only by serialization. Do not call directly.")]
        public FusePipe()
        {
        }

        public FusePipe(FuseStreamable<TPayload, TResult> stream, IStreamObserver<Empty, TResult> observer)
            : base(stream, observer)
        {
            Transform = stream.Transform;
            this.pool = MemoryManager.GetMemoryPool<Empty, TResult>(stream.Properties.IsColumnar);
            this.pool.Get(out this.output);
            this.output.Allocate();
            bstream = Transform(new InputBStream<TPayload>(stream.Period, stream.Offset));
            bstate = bstream.Init();
        }

        public override void ProduceQueryPlan(PlanNode previous)
            => this.Observer.ProduceQueryPlan(
                new FusePlanNode(previous, this, typeof(Empty), typeof(TPayload), typeof(TResult))
            );

        public override void OnNext(StreamMessage<Empty, TPayload> batch)
        {
            bstate = bstream.SetInput(batch, bstate);
            while (!bstream.IsDone(bstate))
            {
                if (bstream.IsReady(bstate))
                {
                    AddToBatch(
                        bstream.GetSyncTime(bstate), bstream.GetOtherTime(bstate), default,
                        bstream.GetPayload(bstate), bstream.GetHash(bstate));
                }

                bstream.Next(bstate);
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
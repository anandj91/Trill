using System;
using System.Runtime.Serialization;
using Microsoft.StreamProcessing.Internal;
using Microsoft.StreamProcessing.Internal.Collections;

namespace Microsoft.StreamProcessing
{
    [DataContract]
    internal sealed class FusePipe<TKey, TPayload, TResult> : UnaryPipe<TKey, TPayload, TResult>
    {
        [SchemaSerialization] private BStreamable<TResult> BStream;
        private readonly MemoryPool<TKey, TResult> pool;
        private readonly string errorMessages;

        [Obsolete("Used only by serialization. Do not call directly.")]
        public FusePipe()
        {
        }

        public FusePipe(FuseStreamable<TKey, TPayload, TResult> stream, IStreamObserver<TKey, TResult> observer)
            : base(stream, observer)
        {
            BStream = stream.BStream;
            this.pool = MemoryManager.GetMemoryPool<TKey, TResult>(stream.Properties.IsColumnar);
            this.errorMessages = stream.ErrorMessages;
        }

        public override void ProduceQueryPlan(PlanNode previous)
            => this.Observer.ProduceQueryPlan(
                new FusePlanNode(previous, this, typeof(TKey), typeof(TPayload), typeof(TResult))
            );

        public override unsafe void OnNext(StreamMessage<TKey, TPayload> batch)
        {
            this.pool.Get(out StreamMessage<TKey, TResult> outputBatch);

            var count = batch.Count;
            outputBatch.vsync = batch.vsync;
            outputBatch.vother = batch.vother;
            outputBatch.key = batch.key;
            outputBatch.hash = batch.hash;
            outputBatch.iter = batch.iter;
            this.pool.GetPayload(out outputBatch.payload);
            outputBatch.bitvector = batch.bitvector;

            var dest = outputBatch.payload.col;
            var src = batch.payload.col;
            fixed (long* bv = batch.bitvector.col)
            {
                for (int i = 0; i < count; i++)
                {
                    if ((bv[i >> 6] & (1L << (i & 0x3f))) == 0)
                    {
                    }
                }
            }

            outputBatch.Count = count;
            batch.payload.Return();
            batch.Return();
            this.Observer.OnNext(outputBatch);
        }

        public override int CurrentlyBufferedOutputCount => 0;

        public override int CurrentlyBufferedInputCount => 0;
    }
}
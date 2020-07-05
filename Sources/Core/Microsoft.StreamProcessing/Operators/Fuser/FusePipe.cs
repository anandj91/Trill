using System;
using System.Runtime.Serialization;
using Microsoft.StreamProcessing.Internal;
using Microsoft.StreamProcessing.Internal.Collections;

namespace Microsoft.StreamProcessing
{
    [DataContract]
    internal sealed class FusePipe<TPayload, TResult> : UnaryPipe<Empty, TPayload, TResult>
    {
        [SchemaSerialization] private Func<FOperation<TPayload>, FOperation<TResult>> Transform;
        private readonly MemoryPool<Empty, TResult> pool;
        private FWindowable<TResult> fwindow;
        private InputFWindow<TPayload> iwindow;

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
            var iop = new FInputOperation<TPayload>(stream.Period, stream.Offset);
            fwindow = Transform(iop).Compile(1);
            iwindow = iop.GetInputFWindow();
        }

        public override void ProduceQueryPlan(PlanNode previous)
            => this.Observer.ProduceQueryPlan(
                new FusePlanNode(previous, this, typeof(Empty), typeof(TPayload), typeof(TResult))
            );

        public override void OnNext(StreamMessage<Empty, TPayload> batch)
        {
            iwindow.SetBatch(batch);
            do
            {
                fwindow.Compute();
                for (int i = 0; i < fwindow.Length; i++)
                {
                    if (fwindow.BV[i])
                    {
                        AddToBatch(fwindow.Sync[i], fwindow.Other[i], default, fwindow.Payload[i], 0);   
                    }
                }
            } while (iwindow.Slide());

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
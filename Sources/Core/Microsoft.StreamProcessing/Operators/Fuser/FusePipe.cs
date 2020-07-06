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
            fwindow = Transform(iop).Compile(25);
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
                var len = fwindow.Compute();
                for (int i = 0; i < len; i++)
                {
                    int index = this.output.Count++;
                    if (!fwindow.BV[i]) this.output.bitvector.col[index >> 6] |= (1L << (index & 0x3f));
                    this.output.vsync.col[index] = fwindow.Sync[i];
                    this.output.vother.col[index] = fwindow.Other[i];
                    if (fwindow.Other[i] == StreamEvent.PunctuationOtherTime)
                    {
                        FlushContents();
                    }
                    else
                    {
                        this.output.payload.col[index] = fwindow.Payload[i];
                    }

                    if (this.output.Count == Config.DataBatchSize) FlushContents();
                }
            } while (iwindow.Slide());

            batch.payload.Return();
            batch.Return();
            FlushContents();
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
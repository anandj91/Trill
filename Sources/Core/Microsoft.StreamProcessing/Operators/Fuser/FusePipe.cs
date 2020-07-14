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
        private OutputFWindow<TResult> owindow;

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
            var fop = Transform(iop);
            fwindow = fop.Compile(0, fop.Size * 10);
            iwindow = iop.GetInputFWindow();
            owindow = new OutputFWindow<TResult>(fwindow);
            owindow.SetBatch(this.output);
        }

        public override void ProduceQueryPlan(PlanNode previous)
            => this.Observer.ProduceQueryPlan(
                new FusePlanNode(previous, this, typeof(Empty), typeof(TPayload), typeof(TResult))
            );

        public override void OnNext(StreamMessage<Empty, TPayload> batch)
        {
            iwindow.SetBatch(batch);
            owindow.SetBatch(this.output);

            int len = 0;
            while (owindow.Slide(owindow.SyncTime))
            {
                len = owindow.Compute();
                if (this.output.Count >= Config.DataBatchSize - owindow.Length)
                {
                    FlushContents();
                    owindow.SetBatch(this.output);
                }
            }

            batch.Release();
            batch.Return();
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
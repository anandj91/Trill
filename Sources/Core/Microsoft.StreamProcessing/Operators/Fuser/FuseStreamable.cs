using System.Diagnostics.Contracts;

namespace Microsoft.StreamProcessing
{
    internal sealed class FuseStreamable<TKey, TPayload, TResult> : UnaryStreamable<TKey, TPayload, TResult>
    {
        public BStreamable<TResult> BStream;

        public FuseStreamable(IStreamable<TKey, TPayload> source, BStreamable<TResult> bstream)
            : base(source, source.Properties.Fuse(bstream))
        {
            Contract.Requires(source != null);
            BStream = bstream;
            Initialize();
        }

        internal override IStreamObserver<TKey, TPayload> CreatePipe(IStreamObserver<TKey, TResult> observer)
        {
            return new FusePipe<TKey, TPayload, TResult>(this, observer);
        }

        protected override bool CanGenerateColumnar()
        {
            return false;
        }

        public override string ToString() => "Fuse";
    }
}
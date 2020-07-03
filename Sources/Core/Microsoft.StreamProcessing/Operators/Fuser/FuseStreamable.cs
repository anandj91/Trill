﻿using System;
using System.Diagnostics.Contracts;

namespace Microsoft.StreamProcessing
{
    internal sealed class FuseStreamable<TPayload, TState, TResult> : UnaryStreamable<Empty, TPayload, TResult>
    {
        public Func<InputBStream<Empty, TPayload>, BStreamable<TState, TResult>> Transform;
        public long Period;
        public long Offset;

        public FuseStreamable(IStreamable<Empty, TPayload> source,
            Func<InputBStream<Empty, TPayload>, BStreamable<TState, TResult>> transform,
            long period, long offset)
            : base(source, source.Properties.Fuse(transform, period, offset))
        {
            Contract.Requires(source != null);
            Transform = transform;
            Period = period;
            Offset = offset;
            Initialize();
        }

        internal override IStreamObserver<Empty, TPayload> CreatePipe(IStreamObserver<Empty, TResult> observer)
        {
            return new FusePipe<TPayload, TState, TResult>(this, observer);
        }

        protected override bool CanGenerateColumnar()
        {
            return false;
        }

        public override string ToString() => "Fuse";
    }
}
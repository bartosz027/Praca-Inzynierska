using System;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;

namespace OpusWrapper {

    internal sealed class SafeDecoderHandle : SafeHandle {
        public override bool IsInvalid => handle == IntPtr.Zero;

        private SafeDecoderHandle() : 
            base(IntPtr.Zero, true) {
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected override bool ReleaseHandle() {
            API.opus_decoder_destroy(handle);
            return true;
        }
    }

}
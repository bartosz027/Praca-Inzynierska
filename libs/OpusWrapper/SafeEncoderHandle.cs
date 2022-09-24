using System;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;

namespace OpusWrapper {

    internal sealed class SafeEncoderHandle : SafeHandle {
        public override bool IsInvalid => handle == IntPtr.Zero;

        private SafeEncoderHandle() : 
            base(IntPtr.Zero, true) {
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected override bool ReleaseHandle() {
            API.opus_encoder_destroy(handle);
            return true;
        }
    }

}
using System;
using System.IO;
using System.Text;

namespace Network.Server.Core {

    internal class TimestampTextWriter : TextWriter {
        public TimestampTextWriter() {
            _Output = Console.Out;
        }


        public override void Write(string message) {
            _Output.Write(String.Format("{0}: {1}", DateTime.Now.ToString("[HH:mm:ss]"), message));
        }

        public override void WriteLine(string message) {
            _Output.WriteLine(String.Format("{0}: {1}", DateTime.Now.ToString("[HH:mm:ss]"), message));
        }


        public override Encoding Encoding {
            get { return new ASCIIEncoding(); }
        }


        private readonly TextWriter _Output;
    }

}
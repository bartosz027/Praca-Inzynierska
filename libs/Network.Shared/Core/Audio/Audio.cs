using System;
using System.Linq;

using NAudio.Wave;
using NAudio.Wave.SampleProviders;

using OpusWrapper;

namespace Network.Shared.Core.Audio {

    public class Audio {
        public Audio() {
            // Opus
            _Encoder = new OpusEncoder(Application.VoIP, _SampleRate, _NumberOfChannels);
            _EncodedByteArray = new byte[_BufferMilliseconds * (_Bitrate / 8000)];

            _Decoder = new OpusDecoder(_SampleRate, _NumberOfChannels);
            _DecodedByteArray = new byte[_BufferMilliseconds * (_SampleRate * _BitsPerSample * _NumberOfChannels / 8000)];

            // NAudio
            _AudioBuffer = new BufferedWaveProvider(new WaveFormat(_SampleRate, _BitsPerSample, _NumberOfChannels));
            _AudioBuffer.BufferLength = 250 * (_SampleRate * _BitsPerSample * _NumberOfChannels / 8000);
            _AudioBuffer.DiscardOnBufferOverflow = true;

            var volumeSampleProvider = new VolumeSampleProvider(_AudioBuffer.ToSampleProvider());
            volumeSampleProvider.Volume = _Volume;

            _AudioSink = new WaveOut();
            _AudioSink.Init(volumeSampleProvider);

            _AudioSink.Volume = 1.0f;
            _AudioSink.Play();
        }


        public void StartVoiceRecording(EventHandler<WaveInEventArgs> callback) {
            _AudioSource = new WaveInEvent();
            _AudioSource.BufferMilliseconds = _BufferMilliseconds;

            _AudioSource.WaveFormat = new WaveFormat(_SampleRate, _BitsPerSample, _NumberOfChannels);
            _AudioSource.DataAvailable += callback;

            _AudioSource.StartRecording();
        }

        public void Play(byte[] data) {
            if (_HeadphonesMuted == false) {
                _AudioBuffer.AddSamples(data, 0, data.Length);
            }
        }


        public byte[] Encode(byte[] data) {
            int encoded_length = _Encoder.Encode(data, data.Length, _EncodedByteArray, _EncodedByteArray.Length);
            return _EncodedByteArray.Take(encoded_length).ToArray();
        }

        public byte[] Decode(byte[] data) {
            _Decoder.Decode(data, data.Length, _DecodedByteArray, _DecodedByteArray.Length);
            return _DecodedByteArray;
        }


        public static void MuteMicrophone() {
            _MicrophoneMuted = !_MicrophoneMuted;

            if (_MicrophoneMuted == false) {
                _AudioSource.StartRecording();
            }
            else {
                _AudioSource.StopRecording();
            }
        }

        public static void MuteMicrophone(bool flag) {
            if (_MicrophoneMuted == true && flag == false) {
                _AudioSource.StartRecording();
            }
            
            if (_MicrophoneMuted == false && flag == true) {
                _AudioSource.StopRecording();
            }

            _MicrophoneMuted = flag;
        }


        public static void MuteHeadphones() {
            _HeadphonesMuted = !_HeadphonesMuted;
        }

        public static void MuteHeadphones(bool flag) {
            _HeadphonesMuted = flag;
        }


        public static void SetVolume(float value) {
            _Volume = value;
        }

        // Opus
        private OpusEncoder _Encoder;
        private byte[] _EncodedByteArray;

        private OpusDecoder _Decoder;
        private byte[] _DecodedByteArray;

        private int _Bitrate = 96000;

        // Record voice
        private static WaveInEvent _AudioSource;
        private int _BufferMilliseconds = 40;

        // Play voice
        private WaveOut _AudioSink;
        private BufferedWaveProvider _AudioBuffer;

        // Settings
        private int _SampleRate = 48000;
        private int _BitsPerSample = 16;
        private int _NumberOfChannels = 1;

        private static bool _MicrophoneMuted = false, _HeadphonesMuted = false;
        private static float _Volume = 1.0f;
    }

}
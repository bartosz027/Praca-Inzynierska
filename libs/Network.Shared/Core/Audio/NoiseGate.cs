using System;
using System.Threading.Tasks;

namespace Network.Shared.Core.Audio {

    // File: NoiseGate.h
    // Source: https://github.com/jagger2048/Dynamics-processor
    public class NoiseGate {
        public NoiseGate() {
            SetNoiseGate(48000, 0.005, 0.25, 0.0005, -32);
        }

        public void SetNoiseGate(double sample_rate, double attack_time, double release_time, double hold_time, double threshold) {
            _SampleRate = sample_rate;
            _AttackTime = attack_time;
            _ReleaseTime = release_time;
            _HoldTime = hold_time;
            _Threshold = threshold;

            _HoldTimeCount = _HoldTime * _SampleRate;
            _LinearThreshold = Math.Pow(10.0f, _Threshold / 20.0f);

            _AT = Math.Exp(-Math.Log(9) / (_SampleRate * _AttackTime));
            _RT = Math.Exp(-Math.Log(9) / (_SampleRate * _ReleaseTime));
        }

        public byte[] ApplyNoiseGate(byte[] buffer) {
            double[] data = new double[buffer.Length / 2];

            for (int i = 0, j = 0; i < buffer.Length; i += 2, j++) {
                short sample = BitConverter.ToInt16(buffer, i);
                data[j] = (sample / 32768.0);
            }

            for (int i = 0; i < data.Length; i++) {
                double x_abs = (data[i] > 0) ? data[i] : -data[i];
                x_env = _RT * x_env + (1 - _AT) * ((x_abs - x_env > 0) ? x_abs - x_env : 0);

                if (x_env < _LinearThreshold) {
                    gca = 0;
                }
                else {
                    gca = 1;
                }

                if (gca < gs[0]) {
                    // Attack mode
                    _ReleaseCounter = 0;

                    if (++_AttackCounter < _HoldTimeCount) {
                        // Hold mode
                        gs[0] = gs[1];
                    }
                    else {
                        gs[0] = _AT * gs[1] + (1 - _AT) * gca;
                    }

                    gs[1] = gs[0];
                }
                else {
                    // Release mode
                    _AttackCounter = 0;

                    if (++_ReleaseCounter < _HoldTimeCount) {
                        // Hold mode
                        gs[0] = gs[1];
                    }
                    else {
                        gs[0] = _RT * gs[1] + (1 - _RT) * gca;
                    }

                    gs[1] = gs[0];
                }

                data[i] = gs[0] * data[i];
            }

            for (int i = 0, j = 0; i < data.Length; i++, j += 2) {
                short sample = Convert.ToInt16(data[i] * 32768.0);
                byte[] bytes = BitConverter.GetBytes(sample);

                buffer[j] = bytes[0];
                buffer[j + 1] = bytes[1];
            }

            return buffer;
        }

        private double _SampleRate;         // Sample rate (Hz) = 48000
        private double _AttackTime;         // Attack time (seconds) = 0.05
        private double _ReleaseTime;        // Release time (seconds) = 0.02
        private double _HoldTime;           // Hold time (second) = 0.0003
        private double _Threshold;          // Threshold (dB) = -24

        private double _HoldTimeCount;      // Hold time count (points)
        private double _LinearThreshold;    // Linear threshold

        private double _AT;                 // Attack time smoothing coefficient
        private double _RT;                 // Release time smoothing coefficient

        private double _AttackCounter = 0;  // Hold counter for attack time
        private double _ReleaseCounter = 0; // Hold counter for release time

        private double[] gs = { 0, 0 };

        private double x_env = 0;
        private double gca = 0;
    }

}
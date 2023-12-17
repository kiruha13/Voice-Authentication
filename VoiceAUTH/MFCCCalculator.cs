using NAudio.Wave;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;

namespace VoiceAUTH
{
    internal class MFCCCalculator
    {
        public double[][] GetMFCC(string filePath)
        {
            // Чтение аудио из файла WAV
            float[] audioData = ReadAudioFile(filePath);
            int sampleRate = 44100;
            int windowSize = 1024;
            int hopSize = 512;

            List<double[]> mfccList = new List<double[]>();

            // Инициализация объекта MelFrequencyCepstrum для извлечения MFCC
            MelFrequencyCepstrum melCepstrum = new MelFrequencyCepstrum(sampleRate, windowSize, 13);
           

            byte[] buffer = new byte[windowSize];
            int bytesRead;

            // Обработка аудиофайла с использованием окон и извлечение MFCC
            for (int i = 0; i + windowSize < audioData.Length; i += hopSize)
            {
                float[] audioFrame = new float[windowSize];
                Array.Copy(audioData, i, audioFrame, 0, windowSize);

                double[] mfccCoefficients = melCepstrum.Transform(audioFrame);
                mfccList.Add(mfccCoefficients);
            }

            return mfccList.ToArray();
        }

        public float[] ReadAudioFile(string filePath)
        {
            WaveFileReader waveFileReader = new WaveFileReader(filePath);
            int bytesPerSample = waveFileReader.WaveFormat.BitsPerSample / 8;
            int bytesToRead = (int)waveFileReader.Length;
            byte[] byteBuffer = new byte[bytesToRead];
            int bytesRead = waveFileReader.Read(byteBuffer, 0, bytesToRead);

            // Преобразование байтов в массив float для обработки аудио
            float[] floatBuffer = new float[bytesRead / bytesPerSample];
            for (int i = 0; i < bytesRead / bytesPerSample; i++)
            {
                if (bytesPerSample == 2) // 16 бит на сэмпл
                {
                    short sample = BitConverter.ToInt16(byteBuffer, i * 2);
                    floatBuffer[i] = sample / 32768f; // Преобразование в диапазон [-1, 1]
                }
                else if (bytesPerSample == 4) // 32 бита на сэмпл
                {
                    float sample = BitConverter.ToSingle(byteBuffer, i * 4);
                    floatBuffer[i] = sample;
                }
            }

            waveFileReader.Close();
            return floatBuffer;
        }

    }

    class MelFrequencyCepstrum
    {
        private readonly int sampleRate;
        private readonly int windowSize;
        private readonly int numCoefficients;
        private readonly int numFilters;
        private readonly double minFreq;
        private readonly double maxFreq;
        private readonly double[][] filterBank;

        public MelFrequencyCepstrum(int sampleRate, int windowSize, int numCoefficients)
        {
            this.sampleRate = sampleRate;
            this.windowSize = windowSize;
            this.numCoefficients = numCoefficients;
            this.numFilters = 20; // Number of Mel filters

            // Min and max frequencies for Mel filters
            this.minFreq = 133;
            this.maxFreq = (double) sampleRate/2;

            // Initialize Mel filter bank
            this.filterBank = CreateFilterBank();
        }

        private double[][] CreateFilterBank()
        {
            double[][] filterBank = new double[numFilters][];

            double[] melPoints = new double[numFilters + 2];
            melPoints[0] = HzToMel(minFreq);
            melPoints[melPoints.Length - 1] = HzToMel(maxFreq);

            // Equally spaced points in Mel scale
            for (int i = 1; i <= numFilters; i++)
            {
                melPoints[i] = i * (melPoints[melPoints.Length - 1] - melPoints[0]) / (numFilters + 1);
            }

            // Convert Mel points back to Hz scale
            double[] hzPoints = Array.ConvertAll(melPoints, MelToHz);

            for (int i = 0; i < numFilters; i++)
            {
                filterBank[i] = new double[windowSize / 2];

                for (int j = 0; j < windowSize / 2; j++)
                {
                    filterBank[i][j] = TriangularFilter(hzPoints[i], hzPoints[i + 1], hzPoints[i + 2], j * sampleRate / windowSize);
                }
            }

            return filterBank;
        }

        private double HzToMel(double hz)
        {
            return 2595 * Math.Log10(1 + hz / 700);
        }

        private double MelToHz(double mel)
        {
            return 700 * (Math.Pow(10, mel / 2595) - 1);
        }

        private double TriangularFilter(double f1, double f2, double f3, double freq)
        {
            return Math.Max(0, Math.Min((freq - f1) / (f2 - f1), (f3 - freq) / (f3 - f2)));
        }

        public double[] Transform(float[] audioFrame)
        {
            Complex32[] fftBuffer = new Complex32[windowSize];
            for (int i = 0; i < windowSize; i++)
            {
                fftBuffer[i] = new Complex32(audioFrame[i], 0);
            }

            Fourier.Forward(fftBuffer, FourierOptions.Matlab);

            double[] powerSpectrum = new double[windowSize / 2];
            for (int i = 0; i < windowSize / 2; i++)
            {
                powerSpectrum[i] = Math.Pow(fftBuffer[i].Magnitude, 2);
            }

            double[] melEnergies = new double[numFilters];
            for (int i = 0; i < numFilters; i++)
            {
                double energy = 0;
                for (int j = 0; j < windowSize / 2; j++)
                {
                    energy += powerSpectrum[j] * filterBank[i][j];
                }
                melEnergies[i] = Math.Log(energy + 1e-10);
            }

            double[] mfcc = new double[numCoefficients];
            for (int i = 0; i < numCoefficients; i++)
            {
                double sum = 0;
                for (int j = 0; j < numFilters; j++)
                {
                    sum += melEnergies[j] * Math.Cos((i * (j + 0.5) * Math.PI) / numFilters);
                }
                mfcc[i] = sum;
            }

            return mfcc;
        }
    }

}

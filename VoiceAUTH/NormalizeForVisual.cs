using MathNet.Numerics.IntegralTransforms;
using NAudio.Wave;
using System.Numerics;

namespace VoiceAUTH
{
    internal class NormalizeForVisual
    {
        static float[] signal;
        static float[] signal1;


        public Complex[] VisualNorm(string filePath)
        {
            try
            {
                // Здесь должен быть ваш код для загрузки файла и преобразования в дискретный сигнал
                using (var audioFile = new AudioFileReader(filePath))
                {
                    int sampleRate = audioFile.WaveFormat.SampleRate;
                    int channels = audioFile.WaveFormat.Channels;

                    // Получение общей длины сигнала в количестве сэмплов
                    long length = audioFile.Length / (audioFile.WaveFormat.BitsPerSample / 8);

                    // Создание массива для хранения сигнала
                    signal = new float[length];

                    // Чтение данных из файла и сохранение в массив
                    int samplesRead = audioFile.Read(signal, 0, (int)length);

                    int startSampleIndex, endSampleIndex;

                    FindNonSilentRegion(signal, out startSampleIndex, out endSampleIndex);

                    float[] recognizedSignal = new float[endSampleIndex - startSampleIndex];
                    Array.Copy(signal, startSampleIndex, recognizedSignal, 0, endSampleIndex - startSampleIndex);

                    NormalizeSignal(recognizedSignal);

                    signal1 = new float[recognizedSignal.Length];

                    for (int i = 0; i < recognizedSignal.Length; i++)
                    {
                        signal1[i] = recognizedSignal[i];
                    }
                }
            }
            catch (IndexOutOfRangeException ex) 
            {
                MessageBox.Show(ex.Message);
            }

            Complex[] fourierTransform = CalculateFourierTransform(signal1);

            return fourierTransform;
        }

        // Метод для определения границы распознаваемого фрагмента по околонулевым значениям амплитуды
        private void FindNonSilentRegion(float[] signal, out int startIndex, out int endIndex)
        {
            const float Threshold = 1.0f; // Threshold for silence

            startIndex = 0;
            endIndex = signal.Length - 1;

            // Find the first index where the amplitude exceeds the threshold
            for (int i = 0; i < signal.Length; i++)
            {
                if (Math.Abs(signal[i]) > Threshold)
                {
                    startIndex = i;
                    break;
                }
            }

            // Find the last index where the amplitude exceeds the threshold
            for (int i = signal.Length - 1; i >= 0; i--)
            {
                if (Math.Abs(signal[i]) > Threshold)
                {
                    endIndex = i;
                    break;
                }
            }
        }


        static void NormalizeSignal(float[] signal)
        {
            // Находим максимальное абсолютное значение в сигнале
            float max = 0;
            foreach (float sample in signal)
            {
                if (Math.Abs(sample) > max)
                {
                    max = Math.Abs(sample);
                }
            }

            // Нормализуем сигнал
            if (max != 0)
            {
                for (int i = 0; i < signal.Length; i++)
                {
                    signal[i] /= max;
                }
            }
        }

        // Функция для рассчета преобразования Фурье
        static Complex[] CalculateFourierTransform(float[] signal)
        {
            // Преобразование массива значений в комплексные числа
            Complex[] complexSignal = new Complex[signal.Length];
            for (int i = 0; i < signal.Length; i++)
            {
                complexSignal[i] = new Complex(signal[i], 0.0);
            }

            // Прямое преобразование Фурье
            Fourier.Forward(complexSignal, FourierOptions.Default);

            return complexSignal;
        }


    }
}


using MathNet.Numerics.IntegralTransforms;
using NAudio.Wave;
using System.Numerics;

namespace VoiceAUTH
{
    class SpeechProcessing
    {
        static float[] signal;
        static float[] signal1;
        int sampleRate;

        public double[] ProcessAudioAndGetMFCC(string filePath)
        {


            // Код для загрузки файла и преобразования в дискретный сигнал
            using (var audioFile = new AudioFileReader(filePath))
            {
                sampleRate = audioFile.WaveFormat.SampleRate;
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


            // Шаг 2: Применение преобразования Фурье
            Complex[] fourierTransform = CalculateFourierTransform(signal1);


            // Шаг 3: Создание гребенки фильтров с оконной функцией
            float[][] filterBank = CreateFilterBank(20, fourierTransform.Length, sampleRate);


            // Шаг 4: Вычисление энергии для каждого окна
            float[] windowEnergies = CalculateWindowEnergies(fourierTransform, filterBank);


            // Шаг 5: Применение Дискретного косинусного преобразования (DCT)
            double[] dctCoefficients = CalculateDCT(windowEnergies);


            // Шаг 6: Получение набора MFCC (Мел-кепстральных коэффициентов)
            double[] mfcc = GetMFCC(dctCoefficients);


            return mfcc;
        }

        // Метод для определения границы распознаваемого фрагмента по околонулевым значениям амплитуды
        private void FindNonSilentRegion(float[] signal, out int startIndex, out int endIndex)
        {
            const float Threshold = 0.1f; // Порог для определения тишины
            startIndex = 0;
            endIndex = signal.Length - 1;

            // Находим начальный индекс
            for (int i = 0; i < signal.Length; i++)
            {
                if (Math.Abs(signal[i]) > Threshold)
                {
                    startIndex = i;
                    break;
                }
            }

            // Находим конечный индекс
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

        // Функция для создания гребенки фильтров
        static float[][] CreateFilterBank(int numFilters, int fftSize, int sampleRate)
        {
            float minFreq = 0; // Минимальная частота в мел-шкале (обычно 0)
            float maxFreq = MelScaleToHz(HzToMelScale(sampleRate / 2)); // Максимальная частота в мел-шкале (до половины частоты дискретизации)

            // Равномерно распределить точки в мел-шкале
            float[] melPoints = new float[numFilters + 2];
            melPoints[0] = HzToMelScale(minFreq);
            melPoints[numFilters + 1] = HzToMelScale(maxFreq);

            for (int i = 1; i <= numFilters; i++)
            {
                float mel = i * (maxFreq - minFreq) / (numFilters + 1);
                melPoints[i] = HzToMelScale(mel);
            }

            // Преобразовать точки обратно в герцы
            float[] hzPoints = melPoints.Select(mel => MelScaleToHz(mel)).ToArray();

            float[][] filterBank = new float[numFilters][];

            for (int i = 0; i < numFilters; i++)
            {
                filterBank[i] = new float[fftSize / 2];

                float fPrev = hzPoints[i];
                float fCurr = hzPoints[i + 1];
                float fNext = hzPoints[i + 2];

                for (int j = 0; j < fftSize / 2; j++)
                {
                    float freq = j * sampleRate / fftSize;

                    if (freq > fPrev && freq < fCurr)
                        filterBank[i][j] = (freq - fPrev) / (fCurr - fPrev);
                    else if (freq > fCurr && freq < fNext)
                        filterBank[i][j] = (fNext - freq) / (fNext - fCurr);
                    else
                        filterBank[i][j] = 0;
                }
            }

            return filterBank;
        }

        // Функция для конвертации частоты из Гц в мел-шкалу
        static float HzToMelScale(float freq)
        {
            return (float)(2595 * Math.Log10(1 + freq / 700));
        }

        // Функция для конвертации частоты из мел-шкалы в Гц
        static float MelScaleToHz(float mel)
        {
            return (float)(700 * (Math.Pow(10, mel / 2595) - 1));
        }

        // Функция для вычисления энергии для каждого окна
        static float[] CalculateWindowEnergies(Complex[] signal, float[][] filterBank)
        {
            int numFilters = filterBank.Length;
            int signalLength = signal.Length;

            float[] windowEnergies = new float[numFilters];

            // Применяем гребенку фильтров к каждому окну сигнала и вычисляем энергию для каждого окна
            for (int i = 0; i < numFilters; i++)
            {
                float energy = 0f;

                // Применяем фильтр из гребенки к текущему окну сигнала
                for (int j = 0; j < signalLength; j++)
                {
                    if (j < filterBank[i].Length)
                    {
                        // Умножаем значение сигнала на коэффициент фильтра
                        float filteredValue = (float)(signal[j].Magnitude * filterBank[i][j]);

                        // Суммируем квадраты полученных значений для вычисления энергии
                        energy += filteredValue * filteredValue;
                    }

                }

                // Сохраняем энергию для текущего окна
                windowEnergies[i] = energy;
            }

            return windowEnergies;
        }


        // Функция для вычисления Дискретного косинусного преобразования
        static double[] CalculateDCT(float[] energies)
        {
            int N = energies.Length;
            double[] dctCoefficients = new double[N];

            for (int k = 0; k < N; k++)
            {
                double sum = 0f;
                for (int n = 0; n < N; n++)
                {
                    sum += energies[n] * Math.Cos((Math.PI * k / N) * (n + 0.5));
                }

                if (k == 0)
                {
                    dctCoefficients[k] = Math.Sqrt(1.0 / N) * sum;
                }
                else
                {
                    dctCoefficients[k] = Math.Sqrt(2.0 / N) * sum;
                }
            }

            return dctCoefficients;
        }


        // Функция для получения MFCC
        static double[] GetMFCC(double[] dctCoefficients)
        {
            // Количество коэффициентов MFCC
            int numMFCCCoefficients = 20; // Примерное количество коэффициентов MFCC

            // Возьмем первые numMFCCCoefficients коэффициентов DCT как MFCC
            double[] mfcc = new double[numMFCCCoefficients];

            // Заполнение MFCC первыми numMFCCCoefficients коэффициентами DCT
            Array.Copy(dctCoefficients, mfcc, numMFCCCoefficients);

            return mfcc;
        }

        public double CalculateEuclideanDistance(double[] mfcc1, double[] mfcc2)
        {
            if (mfcc1.Length != mfcc2.Length)
            {
                throw new ArgumentException("Arrays have different lengths");
            }

            double distance = 0.0;
            double maxPossibleValue = 100.0;

            for (int i = 0; i < mfcc1.Length; i++)
            {
                distance += Math.Pow((mfcc1[i] - mfcc2[i]), 2);
            }
            double euclideanDistance = Math.Sqrt(distance);
            double percentage = (100 - (euclideanDistance / maxPossibleValue) * 100.0);

            return percentage;
        }

        public double CalculatePearsonCorrelation(double[] array1, double[] array2)
        {
            if (array1.Length != array2.Length)
            {
                throw new ArgumentException("Arrays have different lengths");
            }

            double sumX = 0.0;
            double sumY = 0.0;
            double sumXY = 0.0;
            double sumXSquare = 0.0;
            double sumYSquare = 0.0;
            double nr = 0.0;
            double d = 0.0;
            double xMean = array1.Average();
            double yMean = array2.Average();

            int n = array1.Length;

            for (int i = 0; i < n; i++)
            {
                sumX += array1[i];
                sumY += array2[i];
                sumXY += array1[i] * array2[i];
                sumXSquare += array1[i] * array1[i];
                sumYSquare += array2[i] * array2[i];
                nr += (array1[i] - xMean) * (array2[i] - yMean);
                d += Math.Sqrt(Math.Pow((array1[i] - xMean), 2)) * Math.Sqrt(Math.Pow((array2[i] - yMean), 2));
            }

            double numerator = n * sumXY - sumX * sumY;
            double denominator = Math.Sqrt((n * sumXSquare - sumX * sumX) * (n * sumYSquare - sumY * sumY));

            if (d == 0)
            {
                // Защита от деления на ноль
                return 0.0;
            }

            double correlation = nr / d;

            double similarityPercentage = correlation * 100.0;

            return similarityPercentage;
        }


    }
}

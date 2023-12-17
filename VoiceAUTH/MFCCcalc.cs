using Accord.Audio;
using Accord.Math;
using Accord.Statistics.Analysis;
using NAudio.Wave;

namespace VoiceAUTH
{
    internal class MFCCcalc
    {
        List<string> mfccCoefficients = new List<string>();
        private double[][] reducedMfcc;

        public MFCCcalc(string filePath)
        {
            // Чтение файла в массив байтов
            FileStream stream = new FileStream(filePath, FileMode.Open);
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            stream.Close();

            // Преобразование массива байтов в массив float
            float[] floats = new float[bytes.Length / 4];
            for (int i = 0; i < floats.Length; i++)
            {
                floats[i] = BitConverter.ToSingle(bytes, 4 * i);
            }

            var signal = Signal.FromArray(floats, 44100);
            var mfcc = new MelFrequencyCepstrumCoefficient(20, 13, 133, 22000, 0.97, 44100);

            var mfccDescriptors = mfcc.Transform(signal);

            double[][] mfccDoubleArray = mfccDescriptors.Select(descriptor => descriptor.Descriptor.Select(value => (double)value).ToArray()).ToArray();
            var pca = new PrincipalComponentAnalysis();
            pca.Learn(mfccDoubleArray);
            int numberOfOutputs = 2;
            pca.NumberOfOutputs = numberOfOutputs;
            //reducedMfcc = pca.Transform(mfccDoubleArray);
            reducedMfcc = mfccDoubleArray;
            mfccCoefficients = mfccDescriptors.Select(descriptor => descriptor.ToString()).ToList();
        }


        public List<string> GetMFCCCoefficients()
        {
            return mfccCoefficients;
        }
        public double[][] GetReducedMFCC()
        {
            return reducedMfcc;
        }

        public double CalculateCosineSimilarity(double[][] mfcc1, double[][] mfcc2)
        {
            double similarity = 0.0;

            if (mfcc1.Length == mfcc2.Length)
            {
                double dotProduct = 0.0;
                double magnitudeMfcc1 = 0.0;
                double magnitudeMfcc2 = 0.0;

                for (int i = 0; i < mfcc1.Length; i++)
                {
                    for (int j = 0; j < mfcc1[i].Length; j++)
                    {
                        dotProduct += mfcc1[i][j] * mfcc2[i][j];
                        magnitudeMfcc1 += mfcc1[i][j] * mfcc1[i][j];
                        magnitudeMfcc2 += mfcc2[i][j] * mfcc2[i][j];
                    }
                }

                magnitudeMfcc1 = Math.Sqrt(magnitudeMfcc1);
                magnitudeMfcc2 = Math.Sqrt(magnitudeMfcc2);

                similarity = dotProduct / (magnitudeMfcc1 * magnitudeMfcc2);
            }

            return similarity;
        }


    }
}

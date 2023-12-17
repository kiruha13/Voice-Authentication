using NAudio.CoreAudioApi;
using NAudio.Utils;
using NAudio.Wave;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Numerics;
using System.Text;


namespace VoiceAUTH;

public partial class MainForm : Form
{
    double result1;

    private int buttonClickCount = 0;

    float[] limitedAmplitudes;

    private string login;

    private bool Enter_Param;

    public DB.ApplicationContext db;

    private double[] mfccResult;

    private double[] mfccResult1;

    private double[] mfccResult2;

    readonly double[] AudioValues;

    private int AudioRate;

    readonly WasapiCapture AudioDev;

    private WaveFileWriter writer;

    private string outputFile;

    Bitmap bmp;
    Bitmap bmp1;
    Bitmap bmp2;

    private int maxRecordingTime = 31; // Ограничение записи в секундах

    private WaveOut playbackDevice;

    private AudioFileReader audioFile;

    private double[] mfccDB;

    private double[] mfccDB2;

    private double[] mfccDB1;

    public MainForm(WasapiCapture captureDevice, string PersonLogin, bool param)
    {
        db = new DB.ApplicationContext();

        InitializeComponent();
        login = PersonLogin;
        AudioDev = captureDevice;
        Enter_Param = param;
        WaveFormat fmt = captureDevice.WaveFormat;
        AudioRate = fmt.SampleRate;
        AudioValues = new double[fmt.SampleRate * 10 / 1000];
        formsPlot2.Plot.Clear();
        formsPlot2.Plot.AddSignal(AudioValues, fmt.SampleRate / 1000);
        formsPlot2.Plot.YLabel("Level");
        formsPlot2.Plot.XLabel("Time (milliseconds)");
        formsPlot2.Plot.Title($"{fmt.Encoding} ({fmt.BitsPerSample}-bit) {fmt.SampleRate} KHz");
        formsPlot2.Plot.SetAxisLimitsY(-.5, .5);
        formsPlot2.Refresh();



        AudioDev.DataAvailable += WaveIn_DataAvailable;

    }
    private void Form1_Load(object sender, EventArgs e)
    {
        // Подготовка записи в файл
        AudioDev.DataAvailable += CaptureDevice_DataAvailable;
        if (Enter_Param)
        {
            outputFile = "..\\..\\..\\samples\\" + login + "2.wav";
            formsPlot1.Enabled = false;
            formsPlot3.Enabled = false;
            formsPlot4.Enabled = false;

        }
        else
        {
            outputFile = "..\\..\\..\\samples\\" + login + ".wav";
        }
        writer = new WaveFileWriter(outputFile, AudioDev.WaveFormat);
    }

    private void CaptureDevice_DataAvailable(object sender, WaveInEventArgs e)
    {
        // Запись данных в файл при доступности аудио
        writer?.Write(e.Buffer, 0, e.BytesRecorded);
    }

    private void Form1_FormClosed(object sender, FormClosedEventArgs e)
    {
        Debug.WriteLine($"Closing audio device: {AudioDev}");
        AudioDev.StopRecording();
        writer?.Dispose();
        AudioDev.Dispose();
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
        formsPlot2.RefreshRequest();
    }

    void WaveIn_DataAvailable(object? sender, WaveInEventArgs e)
    {
        int bytesPerSamplePerChannel = AudioDev.WaveFormat.BitsPerSample / 8;
        int bytesPerSample = bytesPerSamplePerChannel * AudioDev.WaveFormat.Channels;
        int bufferSampleCount = e.Buffer.Length / bytesPerSample;

        if (bufferSampleCount >= AudioValues.Length)
        {
            bufferSampleCount = AudioValues.Length;
        }

        if (bytesPerSamplePerChannel == 2 && AudioDev.WaveFormat.Encoding == WaveFormatEncoding.Pcm)
        {
            for (int i = 0; i < bufferSampleCount; i++)
                AudioValues[i] = BitConverter.ToInt16(e.Buffer, i * bytesPerSample);
        }
        else if (bytesPerSamplePerChannel == 4 && AudioDev.WaveFormat.Encoding == WaveFormatEncoding.Pcm)
        {
            for (int i = 0; i < bufferSampleCount; i++)
                AudioValues[i] = BitConverter.ToInt32(e.Buffer, i * bytesPerSample);
        }
        else if (bytesPerSamplePerChannel == 4 && AudioDev.WaveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
        {
            for (int i = 0; i < bufferSampleCount; i++)
                AudioValues[i] = BitConverter.ToSingle(e.Buffer, i * bytesPerSample);
        }
        else
        {
            throw new NotSupportedException(AudioDev.WaveFormat.ToString());
        }

    }

    private void button1_Click(object sender, EventArgs e)
    {
        buttonClickCount++;
        button2.Enabled = true;
        if (Enter_Param)
        {
            button7.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            try
            {
                AudioDev.StartRecording();
                timer2.Start();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message);
                AudioDev.StopRecording();
                writer.Dispose();
                timer2.Stop();
                Close();
            }
            
        }
        else
        {
            if (buttonClickCount == 1)
            {
                button5.Enabled = true;
            }

            if (buttonClickCount == 2)
            {
                button6.Enabled = true;
                button5.Enabled = false;

            }
            if (buttonClickCount == 3)
            {
                button7.Enabled = true;
                button5.Enabled = false;
                button6.Enabled = false;
            }
        }
        


    }

    private void button2_Click(object sender, EventArgs e)
    {
        // Остановить запись и сохранить файл
        AudioDev.StopRecording();
        writer.Dispose();
        timer2.Stop();
        //AudioDev.Dispose();
        progressBar1.Value = 0;

        MessageBox.Show($"Аудио сохранено!");
        SpeechProcessing speechProcessor = new SpeechProcessing();
        if (buttonClickCount == 1)
        {
            mfccResult = speechProcessor.ProcessAudioAndGetMFCC(outputFile);
        }
        if (buttonClickCount == 2)
        {
            mfccResult1 = speechProcessor.ProcessAudioAndGetMFCC(outputFile);
        }
        if (buttonClickCount == 3)
        {
            mfccResult2 = speechProcessor.ProcessAudioAndGetMFCC(outputFile);
        }

        NormalizeForVisual normalize = new NormalizeForVisual();
        Complex[] norm_signal = normalize.VisualNorm(outputFile);
        // Создание массива float для хранения амплитуд сигнала
        float[] amplitudes = new float[norm_signal.Length];
        for (int i = 0; i < norm_signal.Length; i++)
        {
            amplitudes[i] = (float)Complex.Abs(norm_signal[i]);
        }
        double sampleRate = AudioRate; // Пример частоты дискретизации в Гц (замените на вашу)
        double frequencyLimit = 400; // Ограничение частоты до 400 Гц

        int dataPointsToShow = (int)(frequencyLimit * amplitudes.Length / sampleRate);

        limitedAmplitudes = new float[dataPointsToShow];
        Array.Copy(amplitudes, limitedAmplitudes, dataPointsToShow);
        // Вычисление амплитуды для каждого элемента массива Complex[]


        if (buttonClickCount == 1 && Enter_Param == false)
        {
            formsPlot4.Plot.Clear();
            formsPlot4.Plot.YLabel("Spectral Power");
            formsPlot4.Plot.XLabel("Frequency (Hz)");
            //formsPlot4.Plot.SetAxisLimitsX(0.0,20.0);
            formsPlot4.Plot.AddSignal(limitedAmplitudes, 1.0, Color.Blue);
            formsPlot4.Render();
            bmp = formsPlot4.Plot.Render();
            bmp.Save("..\\..\\..\\samples\\" + login + "1.png", ImageFormat.Png);

        }
        if (buttonClickCount == 2 && Enter_Param == false)
        {
            formsPlot1.Plot.Clear();

            formsPlot1.Plot.YLabel("Spectral Power");
            formsPlot1.Plot.XLabel("Frequency (Hz)");

            formsPlot1.Plot.AddSignal(limitedAmplitudes, 1.0, Color.Blue);
            formsPlot1.Render();
            bmp1 = formsPlot1.Plot.Render();
            bmp1.Save("..\\..\\..\\samples\\" + login + "2.png", ImageFormat.Png);

        }
        if (buttonClickCount == 3 && Enter_Param == false)
        {
            formsPlot3.Plot.Clear();

            formsPlot3.Plot.YLabel("Spectral Power");
            formsPlot3.Plot.XLabel("Frequency (Hz)");

            formsPlot3.Plot.AddSignal(limitedAmplitudes, 1.0, Color.Blue);
            formsPlot3.Render();
            bmp2 = formsPlot3.Plot.Render();
            bmp2.Save("..\\..\\..\\samples\\" + login + "3.png", ImageFormat.Png);

        }
        if (Enter_Param)
        {
            var user = db.AudioVect.FirstOrDefault(u => u.Login == login);
            if (user != null)
            {
                mfccDB = DeserializeMFCCArray(user.VectorData);
                mfccDB1 = DeserializeMFCCArray(user.VectorData1);
                mfccDB2 = DeserializeMFCCArray(user.VectorData2);

            }
            double res1 = speechProcessor.CalculateEuclideanDistance(mfccResult, mfccDB);
            double res2 = speechProcessor.CalculateEuclideanDistance(mfccResult, mfccDB1);
            double res3 = speechProcessor.CalculateEuclideanDistance(mfccResult, mfccDB2);
            double[] distances = [res1, res2, res3];
            double result = CalculateAverage(distances);
            result1 = Math.Max(res3, Math.Max(res1, res2));
            label2.Visible = true;
            button4.Enabled = true;
            /*
             * double result1 = speechProcessor.CalculatePearsonCorrelation(mfccResult, mfccDB);
            foreach (double value in mfccDB)
            {
                Debug.WriteLine(value);
            }
            MessageBox.Show("Результат проверки: " + result + " " + result1);
            Close();
            */
        }
        else
        {
            if (buttonClickCount == 3)
            {
                var mfccData = new DB.AudioVect
                {
                    Login = login,
                    VectorData = SerializeMFCCArray(mfccResult),
                    VectorData1 = SerializeMFCCArray(mfccResult1),
                    VectorData2 = SerializeMFCCArray(mfccResult2),
                    ImageSpectr1 = ConvertBitmapToBytes(bmp),
                    ImageSpectr2 = ConvertBitmapToBytes(bmp1),
                    ImageSpectr3 = ConvertBitmapToBytes(bmp2)
                };
                db.AudioVect.Add(mfccData);
                db.SaveChanges();
                MessageBox.Show("Данные успешно сохранены!");
                Close();
            }
        }
        button3.Enabled = true;


    }

    static double CalculateAverage(double[] array)
    {
        double sum = 0.0;

        // Суммируем все элементы массива
        foreach (double num in array)
        {
            sum += num;
        }

        // Вычисляем среднее арифметическое
        double average = sum / array.Length;
        return average;
    }

    public byte[] ConvertBitmapToBytes(Bitmap bitmap)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            bitmap.Save(stream, ImageFormat.Png);
            return stream.ToArray();
        }
    }

    public byte[] SerializeMFCCArray(double[] mfccArray)
    {
        string serialized = JsonConvert.SerializeObject(mfccArray);
        return Encoding.UTF8.GetBytes(serialized);
    }

    public double[] DeserializeMFCCArray(byte[] bytes)
    {
        string deserialized = Encoding.UTF8.GetString(bytes);
        return JsonConvert.DeserializeObject<double[]>(deserialized);
    }

    private void timer2_Tick(object sender, EventArgs e)
    {
        // Обновление времени записи и прогресса ProgressBar
        if (writer != null)
        {
            TimeSpan elapsedTime = TimeSpan.FromSeconds(writer.TotalTime.TotalSeconds);
            label1.Text = elapsedTime.ToString(@"mm\:ss");

            int progressBarValue = (int)(elapsedTime.TotalSeconds / maxRecordingTime * 100);
            progressBar1.Value = progressBarValue;
        }
        if (writer?.TotalTime.TotalSeconds >= maxRecordingTime - 1)
        {
            timer2.Stop();
            button2_Click(sender, e);
        }
    }

    private void button3_Click(object sender, EventArgs e)
    {
        // Прослушивание только что записанного файла
        if (File.Exists(outputFile))
        {
            playbackDevice = new WaveOut();
            audioFile = new AudioFileReader(outputFile);
            playbackDevice.Init(audioFile);
            playbackDevice.Play();
        }
        else
        {
            MessageBox.Show("Файл не найден");
        }
    }

    private void button5_Click(object sender, EventArgs e)
    {
        AudioDev.StartRecording();
        timer2.Start();
        button5.Enabled = false;
    }

    private void button6_Click(object sender, EventArgs e)
    {
        Form1_Load(sender, e);
        AudioDev.StartRecording();
        timer2.Start();
        button6.Enabled = false;
    }

    private void button7_Click(object sender, EventArgs e)
    {
        Form1_Load(sender, e);
        AudioDev.StartRecording();
        timer2.Start();
        button7.Enabled = false;
    }

    private void button4_Click(object sender, EventArgs e)
    {
        if (result1 != 0.0)
        {
            label2.Text = "Результат проверки: " + (int)result1 + "%";
        }
        else
        {
            MessageBox.Show("Ошибка!");
        }
        button1.Enabled = false;
        button2.Enabled = false;
        button3.Enabled = false;
        button4.Enabled = false;
        button5.Enabled = false;
        button6.Enabled = false;
        button7.Enabled = false;

    }
}

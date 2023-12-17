namespace VoiceAUTH
{
    public partial class Register : Form
    {
        public DB.ApplicationContext db;
        string text;
        public Register()
        {
            db = new DB.ApplicationContext();
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            text = textBox1.Text.Trim();
            if (text == "") 
            {
                MessageBox.Show("Неверный ввод!");
            }
            else
            {
                if (db.CheckIfUserExists(text) == false)
                {
                    DeviceForm newForm = new DeviceForm(text, false);
                    newForm.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Пользователь с таким именем уже существует");
                }

            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            text = textBox1.Text.Trim();
            if (text == "")
            {
                MessageBox.Show("Неверный ввод!");
            }
            else
            {
                if (db.CheckIfUserExists(text))
                {
                    DeviceForm newForm = new DeviceForm(text, true);
                    newForm.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Пользователя с таким именем не существует");
                }
            }
        }
    }
}

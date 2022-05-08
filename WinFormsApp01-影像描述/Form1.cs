using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {


            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string cvApiUrl, cvApiKey, imagePath;
                label1.Text = imagePath = openFileDialog1.FileName;
                cvApiKey = "4aae1a0df7704727abb6809c9f7d2579";
                cvApiUrl = "https://kevincv20220517.cognitiveservices.azure.com/";

                try
                {
                    FileStream fs = File.Open(imagePath, FileMode.Open);    //開檔
                    ComputerVisionClient computerVisionClient =
                        new ComputerVisionClient
                        (new ApiKeyServiceClientCredentials(cvApiKey),
                        new System.Net.Http.DelegatingHandler[] { });
                    computerVisionClient.Endpoint = cvApiUrl;

                    ImageDescription result = await
                        computerVisionClient.DescribeImageInStreamAsync(fs);

                    if (result == null)
                    {
                        richTextBox2.Text = "無法分析";
                        return;
                    }
                    string str = $"影像描述：{result.Captions[0].Text}\n" +
                        $"描述信度：{result.Captions[0].Confidence}\n\n";

                    str += $"標籤項目：\n";
                    for (int i = 0; i < result.Tags.Count; i++)
                    {
                        str += $"\t{result.Tags[i]}\n";
                    }
                    richTextBox1.Text = str;

                    string jsonStr = JsonConvert.SerializeObject(result);
                    richTextBox2.Text = JObject.Parse(jsonStr).ToString();

                    fs.Close();
                    fs.Dispose();
                    pictureBox1.Image = Image.FromFile(imagePath);
                }
                catch (Exception ex)
                {
                    richTextBox2.Text = ex.Message;
                }

            }

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
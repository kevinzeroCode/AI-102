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
                    FileStream fs = File.Open(imagePath, FileMode.Open);    //�}��
                    ComputerVisionClient computerVisionClient =
                        new ComputerVisionClient
                        (new ApiKeyServiceClientCredentials(cvApiKey),
                        new System.Net.Http.DelegatingHandler[] { });
                    computerVisionClient.Endpoint = cvApiUrl;

                    //���w��ı�S�x

                    VisualFeatureTypes?[] visualFeatureTypes = new VisualFeatureTypes?[]
                    {
                        VisualFeatureTypes.ImageType,
                        VisualFeatureTypes.Color,
                        VisualFeatureTypes.Faces,
                        VisualFeatureTypes.Adult,
                        VisualFeatureTypes.Categories,
                        VisualFeatureTypes.Tags,
                        VisualFeatureTypes.Description,
                        VisualFeatureTypes.Objects
                    };

                    //ImageDescription result = await
                    //  computerVisionClient.DescribeImageInStreamAsync(fs);
                    //�v�����R������

                    ImageAnalysis result = await
                        computerVisionClient.AnalyzeImageInStreamAsync(fs, visualFeatureTypes);

                    if (result == null)
                    {
                        richTextBox2.Text = "�L�k���R";
                        return;
                    }
                    /*
                    string str = $"�v���y�z�G{result.Captions[0].Text}\n" +
                        $"�y�z�H�סG{result.Captions[0].Confidence}\n\n";

                    str += $"���Ҷ��ءG\n";
                    for (int i = 0; i < result.Tags.Count; i++)
                    {
                        str += $"\t{result.Tags[i]}\n";
                    }
                    */

                    string str = "";
                    for (int i = 0; i < result.Description.Captions.Count; i++)
                    {
                        str += $"�v���y�z:{result.Description.Captions[i].Text}\n"+
                            $"�y�z�H��:{result.Description.Captions[i].Confidence}\n";
                    }
                    str += $"�y����T\n";
                    for(int i=0; i<result.Faces.Count; i++)
                    {
                        str += $"\t{result.Faces[i].Gender}\t{result.Faces[i].Age}\n";
                    }
                    str += $"���Ҹ�T\n";
                    for (int i = 0; i < result.Tags.Count; i++)
                    {
                        str += $"\t{result.Tags[i].Name}\t{result.Tags[i].Confidence}\n";
                    }

                    str += $"���H��T�G{result.Adult.IsAdultContent}\t{result.Adult.AdultScore}\n";
                    str += $"�ൣ���y�G{result.Adult.IsRacyContent}\t{result.Adult.RacyScore}\n";
                    str += $"��{��T�G{result.Adult.IsGoryContent}\t{result.Adult.GoreScore}\n";
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
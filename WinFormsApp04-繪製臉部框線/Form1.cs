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

                    //指定視覺特徵

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
                    //影像分析的物件

                    ImageAnalysis result = await
                        computerVisionClient.AnalyzeImageInStreamAsync(fs, visualFeatureTypes);

                    if (result == null)
                    {
                        richTextBox2.Text = "無法分析";
                        return;
                    }
                    /*
                    string str = $"影像描述：{result.Captions[0].Text}\n" +
                        $"描述信度：{result.Captions[0].Confidence}\n\n";

                    str += $"標籤項目：\n";
                    for (int i = 0; i < result.Tags.Count; i++)
                    {
                        str += $"\t{result.Tags[i]}\n";
                    }
                    */

                    string str = "";
                    for (int i = 0; i < result.Description.Captions.Count; i++)
                    {
                        str += $"影像描述:{result.Description.Captions[i].Text}\n" +
                            $"描述信度:{result.Description.Captions[i].Confidence}\n";
                    }
                    str += $"臉部資訊\n";
                    for (int i = 0; i < result.Faces.Count; i++)
                    {
                        str += $"\t{result.Faces[i].Gender}\t{result.Faces[i].Age}\n";
                    }
                    str += $"標籤資訊\n";
                    for (int i = 0; i < result.Tags.Count; i++)
                    {
                        str += $"\t{result.Tags[i].Name}\t{result.Tags[i].Confidence}\n";
                    }

                    str += $"成人資訊：{result.Adult.IsAdultContent}\t{result.Adult.AdultScore}\n";
                    str += $"兒童不宜：{result.Adult.IsRacyContent}\t{result.Adult.RacyScore}\n";
                    str += $"血腥資訊：{result.Adult.IsGoryContent}\t{result.Adult.GoreScore}\n";

                    if (result.Categories[0].Detail != null)
                    {
                        //印出名人
                        if (result.Categories[0].Detail.Celebrities != null)
                        {
                            str += $"名人資訊如下：\n";
                            for (int i = 0; i < result.Categories[0].Detail.Celebrities.Count; i++)
                            {
                                str += $"\t{result.Categories[0].Detail.Celebrities[i].Name}" +
                                    $"\t{result.Categories[0].Detail.Celebrities[i].Confidence}\n";
                            }
                        }

                        //印出地標
                        if (result.Categories[0].Detail.Landmarks != null)
                        {
                            str += $"地標資訊如下：\n";
                            for (int i = 0; i < result.Categories[0].Detail.Landmarks.Count; i++)
                            {
                                str += $"\t{result.Categories[0].Detail.Landmarks[i].Name}" +
                                    $"\t{result.Categories[0].Detail.Landmarks[i].Confidence}\n";
                            }
                        }
                    }


                    richTextBox1.Text = str;

                    string jsonStr = JsonConvert.SerializeObject(result);
                    richTextBox2.Text = JObject.Parse(jsonStr).ToString();

                    fs.Close();
                    fs.Dispose();
                    pictureBox1.Image = Image.FromFile(imagePath);

                    if (result.Faces.Count() != 0)
                    {
                        pictureBox1.Refresh();
                        // 取得原圖和pictureBox的height, width
                        float floPhysicalWidth = pictureBox1.Image.PhysicalDimension.Width;   //原圖寬
                        float floPhysicalHeight = pictureBox1.Image.PhysicalDimension.Height;  //原圖高
                        int intVedioWidth = pictureBox1.Width;  //pictureBox1寬
                        int intVedioHeight = pictureBox1.Height; //picuteBox1高

                        //在pictureBox1影像中的所有人臉上畫出矩形
                        Graphics g = pictureBox1.CreateGraphics();
                        Pen p = new Pen(Color.Blue, 5);
                        int left, top, width, height;
                        for (int i = 0; i < result.Faces.Count; i++)
                        {
                            // 依比例找出實際pictureBox1要畫矩形的left, top, width, height
                            left = (int)(intVedioWidth * result.Faces[i].FaceRectangle.Left / floPhysicalWidth);
                            top = (int)(intVedioHeight * result.Faces[i].FaceRectangle.Top / floPhysicalHeight);
                            width = (int)(intVedioWidth * result.Faces[i].FaceRectangle.Height / floPhysicalWidth);
                            height = (int)(intVedioHeight * result.Faces[i].FaceRectangle.Width / floPhysicalHeight);
                            g.DrawRectangle(p, left, top, width, height);
                            g.DrawString(result.Faces[i].Age.ToString() + " " + result.Faces[i].Gender, new Font("Arial", 8), Brushes.Green, new Point(left, top - 40));
                        }

                        //畫名人的臉
                        for (int i = 0; i < result.Categories[0].Detail.Celebrities.Count(); i++)
                        {
                            // 依比例找出實際pictureBox1要畫矩形的left, top, width, height
                            left = (int)(intVedioWidth * result.Categories[0].Detail.Celebrities[i].FaceRectangle.Left / floPhysicalWidth);
                            top = (int)(intVedioHeight * result.Categories[0].Detail.Celebrities[i].FaceRectangle.Top / floPhysicalHeight);
                            width = (int)(intVedioWidth * result.Categories[0].Detail.Celebrities[i].FaceRectangle.Height / floPhysicalWidth);
                            height = (int)(intVedioHeight * result.Categories[0].Detail.Celebrities[i].FaceRectangle.Width / floPhysicalHeight);
                            g.DrawRectangle(new Pen(Color.Yellow, 3), left, top, width, height);
                            g.DrawString(result.Categories[0].Detail.Celebrities[i].Name, new Font("Arial", 8), Brushes.Red, new Point(left, top - 25));
                        }
                    }
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
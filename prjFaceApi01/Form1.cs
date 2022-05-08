using Newtonsoft.Json;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace prjFaceApi01
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {

                try
                {
                    string apiUrl, apiKey, imagePath;
                    apiUrl = "https://kevinfaceapi0507.cognitiveservices.azure.com/";
                    apiKey = "eca7a5f3a8804bb4b3ecbfce05b0e619";
                    label1.Text = imagePath = openFileDialog1.FileName;

                    FileStream fs = File.OpenRead(imagePath);

                    FaceClient faceClient = new FaceClient
                        (new ApiKeyServiceClientCredentials(apiKey),
                        new System.Net.Http.DelegatingHandler[] { });

                    faceClient.Endpoint = apiUrl;

                    IList<DetectedFace> detectedFaces = null;

                    detectedFaces = await faceClient.Face.DetectWithStreamAsync
                        (fs, detectionModel: DetectionModel.Detection03,
                         recognitionModel: RecognitionModel.Recognition04);

                    if (detectedFaces == null)
                    {
                        richTextBox2.Text = "辨識失敗";
                        return;
                    }

                    string str = $"影像中有 {detectedFaces.Count} 人\n";

                    for (int i = 0; i < detectedFaces.Count; i++)
                    {
                        str += $"\t第 {i + 1} 人臉部位置=>" +
                            $"Left={detectedFaces[i].FaceRectangle.Left}, " +
                            $"Top={detectedFaces[i].FaceRectangle.Top}, " +
                            $"Width={detectedFaces[i].FaceRectangle.Width}, " +
                            $"Height={detectedFaces[i].FaceRectangle.Height}\n ";
                    }

                    richTextBox1.Text = str;

                    richTextBox2.Text = JsonConvert.SerializeObject(detectedFaces);

                    fs.Close();
                    fs.Dispose();

                    pictureBox1.Image = Image.FromFile(imagePath);

                    pictureBox1.Refresh();
                    // 取得原圖和pictureBox1的寬高
                    float floPhysicalWidth = pictureBox1.Image.PhysicalDimension.Width;
                    float floPhysicalHeight = pictureBox1.Image.PhysicalDimension.Height;
                    int intVedioWidth = pictureBox1.Width;
                    int intVedioHeight = pictureBox1.Height;

                    //在pictureBox1的人臉上畫出矩形
                    Graphics g = pictureBox1.CreateGraphics();
                    Pen p = new Pen(Color.Red, 2);
                    int left, top, width, height;
                    for (int i = 0; i < detectedFaces.Count; i++)
                    {
                        // 依比例找出實際pictureBox1要畫矩形的範圍(left, top, width, height)
                        left = (int)(intVedioWidth * detectedFaces[i].FaceRectangle.Left / floPhysicalWidth);
                        top = (int)(intVedioHeight * detectedFaces[i].FaceRectangle.Top / floPhysicalHeight);
                        width = (int)(intVedioWidth * detectedFaces[i].FaceRectangle.Width / floPhysicalWidth);
                        height = (int)(intVedioHeight * detectedFaces[i].FaceRectangle.Height / floPhysicalHeight);
                        g.DrawRectangle(p, left, top, width, height);
                    }


                }
                catch (Exception ex)
                {
                    richTextBox2.Text = ex.Message;
                }

            }
        }

        private  void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
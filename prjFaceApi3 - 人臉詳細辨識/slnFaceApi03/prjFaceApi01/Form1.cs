using Newtonsoft.Json;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

using Emgu.CV;

namespace prjFaceApi01
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Capture cap = null;
        string appPath = "";
        string apiUrl, apiKey;

        //視窗載入時
        private void Form1_Load(object sender, EventArgs e)
        {

            apiUrl = "https://kevinfaceapi0507.cognitiveservices.azure.com/";
            apiKey = "eca7a5f3a8804bb4b3ecbfce05b0e619";

            appPath = Application.StartupPath;

            DirectoryInfo directoryInfo = new DirectoryInfo($"{appPath}/Images/");   
            FileInfo[] files = directoryInfo.GetFiles();    
            foreach (FileInfo file in files)
            {
                System.IO.File.Delete(file.FullName);
            }
            

            cap = new Capture(0);
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {

                try
                {
                    string  imagePath;

                    label1.Text = imagePath = openFileDialog1.FileName;

                    FileStream fs = File.OpenRead(imagePath);

                    FaceClient faceClient = new FaceClient
                        (new ApiKeyServiceClientCredentials(apiKey),
                        new System.Net.Http.DelegatingHandler[] { });

                    faceClient.Endpoint = apiUrl;

                    IList<DetectedFace> detectedFaces = null;

                    //指定臉部特徵
                    IList<FaceAttributeType> faceAttributeType = new List<FaceAttributeType>()
                    { FaceAttributeType.Accessories,
                    FaceAttributeType.Age,
                    FaceAttributeType.Gender,
                    FaceAttributeType.Emotion,
                    FaceAttributeType.FacialHair,
                    FaceAttributeType.Glasses,
                    FaceAttributeType.Hair,
                    FaceAttributeType.HeadPose,
                    FaceAttributeType.Makeup,
                    FaceAttributeType.Occlusion,
                    FaceAttributeType.Smile  };
                    /*
                    detectedFaces = await faceClient.Face.DetectWithStreamAsync
                        (fs, detectionModel: DetectionModel.Detection03,
                         recognitionModel: RecognitionModel.Recognition04);
                    */

                    detectedFaces = await faceClient.Face.DetectWithStreamAsync
                        (fs, detectionModel: DetectionModel.Detection01,
                        recognitionModel: RecognitionModel.Recognition04, 
                        returnFaceAttributes:faceAttributeType);

                    if (detectedFaces == null)
                    {
                        richTextBox2.Text = "辨識失敗";
                        return;
                    }

                    string str = $"影像中有 {detectedFaces.Count} 人\n";

                    for (int i = 0; i < detectedFaces.Count; i++)
                    {
                        str += $"\t第 {i + 1} 人臉部資訊=>" +
                            $"Left={detectedFaces[i].FaceRectangle.Left}, " +
                            $"Top={detectedFaces[i].FaceRectangle.Top}, " +
                            $"Width={detectedFaces[i].FaceRectangle.Width}, " +
                            $"Height={detectedFaces[i].FaceRectangle.Height}\n ";

                        str += $"\t性別：{detectedFaces[i].FaceAttributes.Gender}\n" +
                            $"\t年齡：{detectedFaces[i].FaceAttributes.Age}\n" +
                            $"\t配戴眼鏡：{detectedFaces[i].FaceAttributes.Glasses}\n" +
                            $"\t快樂程度：{detectedFaces[i].FaceAttributes.Emotion.Happiness}\n" +
                            $"\t驚喜程度：{detectedFaces[i].FaceAttributes.Emotion.Surprise}\n" +
                            $"\t生氣程度：{detectedFaces[i].FaceAttributes.Emotion.Anger}\n" +
                            $"\t悲傷程度：{detectedFaces[i].FaceAttributes.Emotion.Sadness}\n" +
                            $"\t中立程度：{detectedFaces[i].FaceAttributes.Emotion.Neutral}\n\n";

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

        private void timer2_Tick(object sender, EventArgs e)
        {
            pictureBox2.Image = cap.QueryFrame().Bitmap;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            timer2.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            timer2.Enabled = false;
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            //存圖
            pictureBox1.Image = cap.QueryFrame().Bitmap;
            string imagePath = $"{appPath}/Images/{Guid.NewGuid().ToString()}.jpg";
            FileInfo fileInfo = new FileInfo(imagePath);
            FileStream fileStream = fileInfo.Create();
            pictureBox1.Image.Save
                (fileStream,System.Drawing.Imaging.ImageFormat.Jpeg);
            fileStream.Close();


            try
            {
              

               

                FileStream fs = File.OpenRead(imagePath);

                FaceClient faceClient = new FaceClient
                    (new ApiKeyServiceClientCredentials(apiKey),
                    new System.Net.Http.DelegatingHandler[] { });

                faceClient.Endpoint = apiUrl;

                IList<DetectedFace> detectedFaces = null;

                //指定臉部特徵
                IList<FaceAttributeType> faceAttributeType = new List<FaceAttributeType>()
                    { FaceAttributeType.Accessories,
                    FaceAttributeType.Age,
                    FaceAttributeType.Gender,
                    FaceAttributeType.Emotion,
                    FaceAttributeType.FacialHair,
                    FaceAttributeType.Glasses,
                    FaceAttributeType.Hair,
                    FaceAttributeType.HeadPose,
                    FaceAttributeType.Makeup,
                    FaceAttributeType.Occlusion,
                    FaceAttributeType.Smile  };
                /*
                detectedFaces = await faceClient.Face.DetectWithStreamAsync
                    (fs, detectionModel: DetectionModel.Detection03,
                     recognitionModel: RecognitionModel.Recognition04);
                */

                detectedFaces = await faceClient.Face.DetectWithStreamAsync
                    (fs, detectionModel: DetectionModel.Detection01,
                    recognitionModel: RecognitionModel.Recognition04,
                    returnFaceAttributes: faceAttributeType);

                if (detectedFaces == null)
                {
                    richTextBox2.Text = "辨識失敗";
                    return;
                }

                string str = $"影像中有 {detectedFaces.Count} 人\n";

                for (int i = 0; i < detectedFaces.Count; i++)
                {
                    str += $"\t第 {i + 1} 人臉部資訊=>" +
                        $"Left={detectedFaces[i].FaceRectangle.Left}, " +
                        $"Top={detectedFaces[i].FaceRectangle.Top}, " +
                        $"Width={detectedFaces[i].FaceRectangle.Width}, " +
                        $"Height={detectedFaces[i].FaceRectangle.Height}\n ";

                    str += $"\t性別：{detectedFaces[i].FaceAttributes.Gender}\n" +
                        $"\t年齡：{detectedFaces[i].FaceAttributes.Age}\n" +
                        $"\t配戴眼鏡：{detectedFaces[i].FaceAttributes.Glasses}\n" +
                        $"\t快樂程度：{detectedFaces[i].FaceAttributes.Emotion.Happiness}\n" +
                        $"\t驚喜程度：{detectedFaces[i].FaceAttributes.Emotion.Surprise}\n" +
                        $"\t生氣程度：{detectedFaces[i].FaceAttributes.Emotion.Anger}\n" +
                        $"\t悲傷程度：{detectedFaces[i].FaceAttributes.Emotion.Sadness}\n" +
                        $"\t中立程度：{detectedFaces[i].FaceAttributes.Emotion.Neutral}\n\n";

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
}
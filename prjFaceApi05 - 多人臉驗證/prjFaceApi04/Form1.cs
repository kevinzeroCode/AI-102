using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace prjFaceApi04
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string apiUrl, apiKey;

        string[] faceIdAry1, faceIdAry2;

        //表單(視窗)載入
        private void Form1_Load(object sender, EventArgs e)
        {
            //請放入Face API的key和網址
            apiUrl = "https://kevinfaceapi0507.cognitiveservices.azure.com/";
            apiKey = "eca7a5f3a8804bb4b3ecbfce05b0e619";   
        }

        private async Task<string[]> GetFaceId (string _imgPath, string _apiUrl, string _apiKey)
        {
            FileStream fileStream = File.OpenRead(_imgPath);    
            FaceClient faceClient = new FaceClient
                ( new ApiKeyServiceClientCredentials(_apiKey), 
                new System.Net.Http.DelegatingHandler[] { });
            faceClient.Endpoint = _apiUrl;

            IList<DetectedFace> detectedFaces = await 
                faceClient.Face.DetectWithStreamAsync(fileStream , 
                   detectionModel:DetectionModel.Detection03,
                   recognitionModel:RecognitionModel.Recognition04);

            if (detectedFaces.Count > 0)
            {
                string[] faceIdAry = new string[detectedFaces.Count];
                for(int i= 0; i<detectedFaces.Count; i++)
                {
                    faceIdAry[i] = detectedFaces[i].FaceId.ToString();
                }
                return faceIdAry;
                //return detectedFaces[0].FaceId.ToString();
            }
            else
            {
                return null;
                //return "無法取得Face Id，請重新選一張照片";
            }
        }

        //開檔2
        private async void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox2.Image = new Bitmap(openFileDialog1.FileName);
                faceIdAry2 = await GetFaceId(openFileDialog1.FileName, apiUrl, apiKey);
                richTextBox2.Text = "";
                for (int i = 0; i < faceIdAry2.Length; i++)
                {
                    richTextBox2.Text += faceIdAry1[i] + "\n";
                }
                //richTextBox2.Text = await GetFaceId(openFileDialog1.FileName, apiUrl, apiKey);
            };
        }

        //臉部驗證
        private async void button3_Click(object sender, EventArgs e)
        {

            if(richTextBox1.Text!="" || richTextBox2.Text != "")
            {
                FaceClient faceClient = new FaceClient
                    (new ApiKeyServiceClientCredentials(apiKey),
                    new System.Net.Http.DelegatingHandler[] { });
                faceClient.Endpoint = apiUrl;

                int count = 0;
                for(int i = 0;i < faceIdAry1.Length;i++)
                {
                    for (int k = 0;k < faceIdAry2.Length; k++)
                    {
                        VerifyResult result = await faceClient.Face.VerifyFaceToFaceAsync
                            (
                               System.Guid.Parse(faceIdAry1[i]),
                               System.Guid.Parse(faceIdAry2[k])
                            );
                        if (result.IsIdentical == true)
                        {
                            count++;
                            break;
                        }
                    }
                }

                MessageBox.Show($"有 {count} 人同時出現在照片中");


                //比較Face Id
                //VerifyResult result = await faceClient.Face.VerifyFaceToFaceAsync
                //    (
                //      System.Guid.Parse(richTextBox1.Text),
                //      System.Guid.Parse(richTextBox2.Text)
                //    );
                //string msg = $"是否為同一人：{result.IsIdentical}\n" +
                //    $"相似度：{result.Confidence}";
                //MessageBox.Show (msg);
            }

        }

        //開檔1
        private async void button1_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog () == DialogResult.OK)
            {
                pictureBox1.Image = new Bitmap(openFileDialog1.FileName);
                faceIdAry1 = await GetFaceId(openFileDialog1.FileName, apiUrl, apiKey);
                richTextBox1.Text = "";
                for(int i = 0; i < faceIdAry1.Length; i++)
                {
                    richTextBox1.Text+=faceIdAry1 [i]+"\n";  
                }

                //richTextBox1 .Text = await GetFaceId(openFileDialog1.FileName, apiUrl, apiKey);
            };
        }
    }
}
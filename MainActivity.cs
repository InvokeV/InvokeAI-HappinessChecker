using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using System;
using Android.Provider;
using Android.Graphics;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using Android.Database;
using Android.Views;

namespace HappinessChecker
{
    [DataContract]
    class FaceRectangle
    {
        [DataMember]
        public float height { get; set; }
        [DataMember]
        public float left { get; set; }
        [DataMember]
        public float top { get; set; }
        [DataMember]
        public float width { get; set; }
    }

    [DataContract]
    class Scores
    {
        [DataMember]
        public Double anger { get; set; }
        [DataMember]
        public Double contempt { get; set; }
        [DataMember]
        public Double disgust { get; set; }
        [DataMember]
        public Double fear { get; set; }
        [DataMember]
        public Double happiness { get; set; }
        [DataMember]
        public Double neutral { get; set; }
        [DataMember]
        public Double sadness { get; set; }
        [DataMember]
        public Double surprise { get; set; }
    }

    [DataContract]
    class EmotionData
    {
        [DataMember]
        public FaceRectangle faceRectangle { get; set; }

        [DataMember]
        public Scores scores { get; set; }

        [DataMember]
        public Double ID { get; set; }
    }

    [DataContract]
    class Error
    {
        [DataMember]
        public string code { get; set; }
        [DataMember]
        public string message { get; set; }
    }

    [DataContract]
    class ErrorData
    {
        [DataMember]
        public Error error { get; set; }
    }

    //Androiddマニフェスト　CAMERA　WRITE_EXTERNAL_STRAGE INTERNET ACCESS_CORE_LOCATION ACCESS_FINE_LOCAYIONを許可
    [Activity(Label = "Happiness Checker", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        public Java.IO.File file;
        public Java.IO.File dir;
        public Bitmap bitmap;
        ImageView imageV1;
        public static readonly int PickImageId = 1000;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            var btnCamera = FindViewById<Android.Widget.ImageButton>(Resource.Id.btnCamera);
            btnCamera.Click += btnCamera_Click;

            var btnFolder = FindViewById<Android.Widget.ImageButton>(Resource.Id.btnFolder);
            btnFolder.Click += btnFolder_Click;

            var btnCognitive = FindViewById<Android.Widget.ImageButton>(Resource.Id.btnCognitive);
            btnCognitive.Click += btnCognitive_Click;

            imageV1 = FindViewById<ImageView>(Resource.Id.imageView1);

            dir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures);
        }

        private void btnCamera_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            file = new Java.IO.File(dir, String.Format("{0}.jpg", System.DateTime.Now.ToString("HHmmss")));
            intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(file));
            StartActivityForResult(intent, 0);
        }

        private void btnFolder_Click(object sender, EventArgs e)
        {
            var intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(intent, "Select Picture"), PickImageId);
        }

        private async void btnCognitive_Click(object sender, EventArgs e)
        {
            var dialog = new ProgressDialog(this);
            dialog.SetTitle("Happiness Checker");
            dialog.SetMessage("Recognizing");
            dialog.Indeterminate = false;
            dialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            dialog.Max = 100;
            dialog.IncrementProgressBy(0);
            dialog.Show();

            string json = await MakeRequest(file.Path);
            PersJson(json);

            dialog.Dismiss();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (resultCode == Result.Ok)
            {
                switch (requestCode)
                {
                    case 0:
                        base.OnActivityResult(requestCode, resultCode, data);
                        Intent intent = new Intent(Intent.ActionMediaScannerScanFile);
                        GetBitmap();
                        break;

                    case 1000:
                        if (data != null)
                        {
                            string imagePath = null;
                            string[] projection = new[] { Android.Provider.MediaStore.Audio.Media.InterfaceConsts.Data };
                            using (ICursor cursor = ManagedQuery(data.Data, projection, null, null, null))
                            {
                                if (cursor != null)
                                {
                                    int columnIndex = cursor.GetColumnIndexOrThrow(Android.Provider.MediaStore.Audio.Media.InterfaceConsts.Data);
                                    cursor.MoveToFirst();
                                    imagePath = cursor.GetString(columnIndex);
                                    file = new Java.IO.File(imagePath);
                                    GetBitmap();
                                }
                            }
                        }
                        break;
                }
            }
        }

        private void GetBitmap()
        {
            var uri = Android.Net.Uri.FromFile(file);
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InJustDecodeBounds = true;
            BitmapFactory.DecodeFile(file.Path, options);
            int imageWidth = options.OutWidth;
            int imageHeight = options.OutHeight;
            int viewWidth = imageV1.Width;
            int viewHeight = imageV1.Height;
            int scale = 1;
            if (imageHeight > viewHeight || imageWidth > viewWidth)
            {
                if (imageWidth > imageHeight)
                {
                    scale = imageHeight / viewHeight;
                }
                else
                {
                    scale = imageWidth / viewWidth;
                }
            }
            options.InJustDecodeBounds = false;
            options.InSampleSize = scale;

            using (Bitmap bitmap = BitmapFactory.DecodeFile(file.Path))
            {
                Matrix matrix = new Matrix();
                matrix.PostRotate(0);
                using (Bitmap bitmapRotate = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, true))
                {
                    imageV1.SetImageBitmap(bitmapRotate);
                }
            }
        }

        private void PersJson(string json)
        {
            if (json != "[]")
            {
                var settings = new DataContractJsonSerializerSettings();
                settings.UseSimpleDictionaryFormat = true;
                var serializer = new DataContractJsonSerializer(typeof(List<EmotionData>), settings);
                var ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
                var objEmotionData = (List<EmotionData>)serializer.ReadObject(ms);

                var paint = new Paint { Color = Color.Blue };
                paint.SetStyle(Paint.Style.Stroke);
                paint.StrokeWidth = 4;
                paint.TextSize = 30;
                BitmapFactory.Options options = new BitmapFactory.Options();
                options.InMutable = true;
                Bitmap bitmap = BitmapFactory.DecodeFile(file.Path, options);
                Canvas canvas = new Canvas(bitmap);
                String happinessValue = "0%";

                foreach (EmotionData data in objEmotionData)
                {
                    float top = data.faceRectangle.top;
                    float left = data.faceRectangle.left;
                    float width = data.faceRectangle.width;
                    float height = data.faceRectangle.height;
                    canvas.DrawCircle(left + width / 2, top + width / 2, width / 2, paint);

                    if (data.scores.happiness > 0.01)
                    {
                        happinessValue = ((data.scores.happiness) * 100).ToString("#") + "%";

                    }
                    else
                    {
                        happinessValue = "0%";
                    }

                    if (objEmotionData.Count == 1)
                    {
                        AlertDialog.Builder alert = new AlertDialog.Builder(this);
                        alert.SetTitle("Happiness Level");
                        alert.SetMessage(happinessValue);
                        alert.Show();
                    }
                    else
                    {
                        canvas.DrawText(happinessValue, left + width, top + width / 4, paint);
                    }
                }
                imageV1.SetImageBitmap(bitmap);

                if (objEmotionData.Count == 0)
                {
                    var errorSerializer = new DataContractJsonSerializer(typeof(ErrorData));
                    var errorMs = new MemoryStream(Encoding.UTF8.GetBytes(json));
                    var objErrorData = (ErrorData)errorSerializer.ReadObject(errorMs);
                    AlertDialog.Builder alert = new AlertDialog.Builder(this);
                    alert.SetTitle("Error");
                    alert.SetMessage(objErrorData.error.message);
                    alert.Show();
                }

            }
            else
            {
                System.Console.WriteLine("Error");
                Toast.MakeText(this, "Error", ToastLength.Long).Show();
            }

        }
      
        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            using (FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }

        static async Task<string> MakeRequest(string imageFilePath)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "your key");
            string uri = "https://westus.api.cognitive.microsoft.com/emotion/v1.0/recognize";

            HttpResponseMessage response;
            string responseContent;

            // ファイル参照
            byte[] byteData = GetImageAsByteArray(imageFilePath);
            var content = new ByteArrayContent(byteData);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            response = await client.PostAsync(uri, content);
            responseContent = await response.Content.ReadAsStringAsync();
            System.Console.WriteLine(responseContent);
            return responseContent;
        }
    }
}


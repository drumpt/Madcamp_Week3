
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;

public class PhoneCamera : MonoBehaviour
{

    private bool camAvailable;
    private WebCamTexture frontCam;
    private Texture defaultBackground;

    public RawImage background;
    public AspectRatioFitter fit;

    //for opencv
    /*Socket sock;
    byte[] receiverBuff;
    VideoCapture video;
    Mat frame;
    CascadeClassifier faceDetector;*/



    // Start is called before the first frame update
    void Start()
    {/*
        //for opencv
        faceDetector = new CascadeClassifier();
        faceDetector.Load(@"C:/Users/q/Downloads/haarcascade_frontalface_alt.xml");

        StartSocketConnection();
*/
        //

        defaultBackground = background.texture;
        WebCamDevice[] devices = WebCamTexture.devices;
        
        //must have least one camera
        if(devices.Length == 0)
        {
            Debug.Log("No Camera detected");
            camAvailable = false;
            return;
        }

        for(int i = 0; i < devices.Length; i++)
        {
            if (devices[i].isFrontFacing)
            {
                frontCam = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
            }
        }

        //must have least one front camera
        if(frontCam == null)
        {
            Debug.Log("Unable to find front camera");
            return;
        }

        frontCam.Play();
        background.texture = frontCam;

        camAvailable = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!camAvailable)
            return;
        float ratio = (float)frontCam.width / (float)frontCam.height;
       fit.aspectRatio = ratio;

        float scaleY = frontCam.videoVerticallyMirrored ? -1f : 1f;
        background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

        int orient = -frontCam.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);

        /*//for opencv
        Texture2D texture = new Texture2D(frontCam.width, frontCam.height, TextureFormat.RGBA32, false);
        frame = new Mat(texture.height, texture.width, MatType.CV_8UC4);
        texture2DToMat(frame, texture);
        OpenCvSharp.Rect[] faces = faceDetector.DetectMultiScale(frame);

        if (faces.Length >= 1) // 얼굴이 존재하는 경우
        {
            OpenCvSharp.Rect face = faces[0];

            // find the largest face in the frame
            for (int i = 0; i < faces.Length; i++)
            {
                if (faces[i].Width * faces[i].Height > face.Width * face.Height) face = faces[i];
            }
            //Console.Out.WriteLine(faces[0]);

            // For predicting facial expression (48 x 48 gray 사진들에 대해서 학습을 진행했기 때문에 48 x 48 grayscale의 사진으로 변환시킴
            Mat resizedGrayFace = new Mat();
            Cv2.CvtColor(new Mat(frame, face), resizedGrayFace, ColorConversionCodes.BGR2GRAY);
            Cv2.CvtColor(resizedGrayFace, resizedGrayFace, ColorConversionCodes.GRAY2BGR);
            Cv2.Resize(resizedGrayFace, resizedGrayFace, new Size(48, 48), 0, 0, InterpolationFlags.Area);

            byte[] buff = resizedGrayFace.ToBytes();
            ////Console.Out.WriteLine(resizedGrayFace.ToBytes().Length);
            sock.Send(buff, SocketFlags.None);
            int n = sock.Receive(receiverBuff);
            string emotion = Encoding.UTF8.GetString(receiverBuff, 0, n);
            Console.Out.WriteLine(emotion);

            switch (emotion)
            {
                case "angry":
                    Input.

            }
        }*/

    }

    //To Stop frontCam
    public void PauseFrontCam()
    {
        frontCam.Pause();
    }
    public void PlayFrontCam()
    {
        frontCam.Play();
    }

    //for opencv
    // start socket connection
    /*private void StartSocketConnection()
    {
        sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        var ep = new IPEndPoint(IPAddress.Parse("192.168.0.93"), 8080); // 방화벽을 해제해야 할 수도 있음
        sock.Connect(ep);
        receiverBuff = new byte[1024];
    }*/

}

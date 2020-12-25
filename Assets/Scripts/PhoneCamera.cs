using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using OpenCVForUnity;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ObjdetectModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;
// using System.Net;
// using System.Net.Sockets;

public class PhoneCamera : MonoBehaviour
{

    private bool camAvailable;
    private WebCamTexture frontCam;
    private Texture defaultBackground;

    public RawImage background;
    public AspectRatioFitter fit;

    private int inputSize = 48;
    private string cascadeClassifierPath = @"Assets/Resources/haarcascade_frontalface_default.xml";

    Mat frame;
    CascadeClassifier faceDetector;

    // Socket sock;
    // byte[] receiverBuff;
    // VideoCapture video;

    // Start is called before the first frame update
    void Start()
    {
        faceDetector = new CascadeClassifier(cascadeClassifierPath);

        // StartSocketConnection();

        defaultBackground = background.texture;
        WebCamDevice[] devices = WebCamTexture.devices;
        
        // need to have have least one camera
        if(devices.Length == 0)
        {
            Debug.Log("No Camera detected");
            camAvailable = false;
            return;
        }

        for(int i = 0; i < devices.Length; i++)
        {
            if(devices[i].isFrontFacing)
            {
                frontCam = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
            }
        }

        // need to have least one front camera
        if(frontCam == null)
        {
            Debug.Log("Unable to find front camera");
            return;
        }

        frontCam.requestedFPS = 60;
        frontCam.Play();
        background.texture = frontCam;

        camAvailable = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!camAvailable)
            return;

        float ratio = (float)frontCam.width / (float)frontCam.height;
        fit.aspectRatio = ratio;

        float scaleY = frontCam.videoVerticallyMirrored ? -1f : 1f;
        background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);
        int orient = -frontCam.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
        
        Texture2D texture = new Texture2D(frontCam.width, frontCam.height, TextureFormat.RGBA32, false);
        Color[] textureData = frontCam.GetPixels();
        texture.SetPixels(textureData);

        frame = new Mat(texture.height, texture.width, CvType.CV_8UC4);
        Utils.texture2DToMat(texture, frame);
        MatOfRect matOfRectFaces = new MatOfRect();
        faceDetector.detectMultiScale(frame, matOfRectFaces);
        OpenCVForUnity.CoreModule.Rect[] faces = matOfRectFaces.toArray();

        if(faces.Length >= 1) // there exist some faces
        {
            OpenCVForUnity.CoreModule.Rect face = faces[0];

            // find largest face in the frame
            for (int i = 0; i < faces.Length; i++)
            {
                if(faces[i].width * faces[i].height > face.width * face.height) face = faces[i];
            }

            Mat resizedGrayFace = new Mat();
            Imgproc.cvtColor(new Mat(frame, face), resizedGrayFace, Imgproc.COLOR_BGR2GRAY);
            Imgproc.cvtColor(resizedGrayFace, resizedGrayFace, Imgproc.COLOR_GRAY2BGR);
            Imgproc.resize(resizedGrayFace, resizedGrayFace, new Size(inputSize, inputSize), 0, 0, Imgproc.INTER_AREA); // resize to 48 x 48

            // byte[] buff = resizedGrayFace.ToBytes();
            // ////Console.Out.WriteLine(resizedGrayFace.ToBytes().Length);
            // sock.Send(buff, SocketFlags.None);
            // int n = sock.Receive(receiverBuff);
            // string emotion = Encoding.UTF8.GetString(receiverBuff, 0, n);
            string emotion = "happy";

            Debug.Log(emotion);
            // Console.Out.WriteLine(emotion);

            // switch(emotion)
            // {
            //     case "angry":
            //         Input.

            // }
        }
    }

    public void PauseFrontCam()
    {
        frontCam.Pause();
    }
    public void PlayFrontCam()
    {
        frontCam.Play();
    }

    // start socket connection
    /*private void StartSocketConnection()
    {
        sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        var ep = new IPEndPoint(IPAddress.Parse("192.168.0.93"), 8080); // 방화벽을 해제해야 할 수도 있음
        sock.Connect(ep);
        receiverBuff = new byte[1024];
    }*/

}
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections.Generic;
using OpenCVForUnity;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ObjdetectModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;
using Unity.Barracuda;

public class PhoneCamera : MonoBehaviour
{
    private bool camAvailable;
    private WebCamTexture frontCam;
    private Texture defaultBackground;
    public RawImage background;
    public AspectRatioFitter fit;

    Mat frame;
    CascadeClassifier faceDetector;
    private string cascadeClassifierPath = @"Assets/Resources/haarcascade_frontalface_default.xml";
    private string modelSourcePath = "new_model";
    private int inputSize = 48;

    Dictionary<int, string> indexToEmotions = new Dictionary<int, string>() {{0, "angry"},  {1, "happy"}, {2, "neutral"}, {3, "sad"}, {4, "surprise"}};
    string emotion;

    private Model model;
    private IWorker worker;
    private Tensor output = null;

    // Start is called before the first frame update
    void Start()
    {
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
        if(frontCam == null)
        {
            Debug.Log("Unable to find front camera");
            return;
        }

        camAvailable = true;
        frontCam.requestedFPS = 60;
        frontCam.Play();
        background.texture = frontCam;

        faceDetector = new CascadeClassifier(cascadeClassifierPath);
        model = ModelLoader.Load((NNModel)Resources.Load(modelSourcePath));
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.CSharp, model); // synchronized execution with CPU usage
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
        if (frame == null) return;
        faceDetector.detectMultiScale(frame, matOfRectFaces);

        OpenCVForUnity.CoreModule.Rect[] faces = matOfRectFaces.toArray();

        if(faces.Length >= 1) // there exist some faces
        {
            OpenCVForUnity.CoreModule.Rect face = faces[0];

            // find largest face in the frame
            for(int i = 0; i < faces.Length; i++)
            {
                if(faces[i].width * faces[i].height > face.width * face.height)
                {
                    face = faces[i];
                }
            }

            Mat resizedGrayFace = new Mat();
            Imgproc.cvtColor(new Mat(frame, face), resizedGrayFace, Imgproc.COLOR_BGR2GRAY);
            Imgproc.resize(resizedGrayFace, resizedGrayFace, new Size(inputSize, inputSize), 0, 0, Imgproc.INTER_AREA);
            Texture2D resizedGrayFaceTexture = new Texture2D(inputSize, inputSize, TextureFormat.RGB24, false);
            Utils.matToTexture2D(resizedGrayFace, resizedGrayFaceTexture);

            Tensor tensor = new Tensor(resizedGrayFaceTexture);
            if (worker == null) return;
            output = worker.Execute(tensor).PeekOutput();
            int index = output.ArgMax()[0];
            emotion = indexToEmotions[index];
            switch(emotion)
            {
                case "happy":
                    InputBroker.forcedKeyDowns = new List<string> {KeyCode.LeftArrow.ToString()};
                    InputBroker.forcedKeyUps = new List<string> {KeyCode.UpArrow.ToString(), KeyCode.DownArrow.ToString(), KeyCode.RightArrow.ToString()};
                    InputBroker.forcedKey = new List<string> {KeyCode.LeftArrow.ToString()};
                    break;
                case "angry":
                    InputBroker.forcedKeyDowns = new List<string> {KeyCode.UpArrow.ToString()};
                    InputBroker.forcedKeyUps = new List<string> {KeyCode.LeftArrow.ToString(), KeyCode.DownArrow.ToString(), KeyCode.RightArrow.ToString()};
                    InputBroker.forcedKey = new List<string> {KeyCode.UpArrow.ToString()};
                    break;
                case "sad":
                    InputBroker.forcedKeyDowns = new List<string> {KeyCode.DownArrow.ToString()};
                    InputBroker.forcedKeyUps = new List<string> {KeyCode.LeftArrow.ToString(), KeyCode.UpArrow.ToString(), KeyCode.RightArrow.ToString()};
                    InputBroker.forcedKey = new List<string> {KeyCode.DownArrow.ToString()};
                    break;
                case "surprise":
                    InputBroker.forcedKeyDowns = new List<string> {KeyCode.RightArrow.ToString()};
                    InputBroker.forcedKeyUps = new List<string> {KeyCode.LeftArrow.ToString(), KeyCode.UpArrow.ToString(), KeyCode.DownArrow.ToString()};
                    InputBroker.forcedKey = new List<string> {KeyCode.RightArrow.ToString()};
                    break;
                default: // neutral
                    InputBroker.forcedKeyDowns = new List<string> {};
                    InputBroker.forcedKeyUps = new List<string> {KeyCode.LeftArrow.ToString(), KeyCode.UpArrow.ToString(), KeyCode.DownArrow.ToString(), KeyCode.RightArrow.ToString()};
                    InputBroker.forcedKey = new List<string> {};
                    break;
            }
            output.Dispose();
        }
    }

    public void OnDisable()
    {
        output.Dispose();
        worker.Dispose();
    }

    public void PauseFrontCam()
    {
        frontCam.Pause();
    }
    public void PlayFrontCam()
    {
        frontCam.Play();
    }
}
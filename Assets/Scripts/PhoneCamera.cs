using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
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

    public const int FPS = 30;

    Mat frame;
    CascadeClassifier faceDetector;
    private string cascadeClassifierPath = "haarcascade_frontalface_default.xml";
    // private string cascadeClassifierPath = @"Assets/StreamingAssets/haarcascade_frontalface_default.xml";
    // private string cascadeClassifierPath = @"Assets/Resources/haarcascade_frontalface_default.xml";
    // private string cascadeClassifierPath = "haarcascade_frontalface_default";
    private string modelSourcePath = "facial_expression_model_sad_neutral_enhenced";
    // private string modelSourcePath = @"Assets/StreamingAssets/facial_expression_model_sad_neutral_enhenced"; // best model so far
    // private string modelSourcePath = "facial_expression_model_sad_neutral_enhenced"; // best model so far
    // private string modelSourcePath = "facial_expression_model_lightweighted";
    // private string modelSourcePath = "facial_expression_model_sad_enhenced";

    private int inputSize = 48;

    Dictionary<int, string> indexToEmotions = new Dictionary<int, string>() {{0, "angry"},  {1, "happy"}, {2, "neutral"}, {3, "sad"}, {4, "surprise"}};
    string emotion;

    private Model model;
    private IWorker worker;
    private Tensor output = null;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(Application.platform);
        Debug.Log(Application.streamingAssetsPath);
        if(Application.platform == RuntimePlatform.OSXEditor)
        {
            cascadeClassifierPath = Application.dataPath + "/StreamingAssets/" + cascadeClassifierPath;
        }
        else if(Application.platform == RuntimePlatform.Android)
        {
            cascadeClassifierPath = "jar:file://" + Application.dataPath + "!/assets/" + cascadeClassifierPath;
            byte[] FileBytes;
            if(cascadeClassifierPath.Contains("://"))
            {
                WWW www = new WWW(cascadeClassifierPath);
                while(!www.isDone) {}
                FileBytes = www.bytes;
            }
            else
            {
                FileBytes = File.ReadAllBytes(cascadeClassifierPath);
            }
            cascadeClassifierPath = string.Format("{0}/{1}", Application.persistentDataPath, "haarcascade_frontalface_default.xml");
            File.WriteAllBytes(cascadeClassifierPath, FileBytes);
        }
        else if(Application.platform == RuntimePlatform.IPhonePlayer)
        {
            cascadeClassifierPath = "file:" + Application.dataPath + "/Raw/" + cascadeClassifierPath;
        }
        else
        {
            cascadeClassifierPath = Application.dataPath + "/StreamingAssets/" + cascadeClassifierPath;
        }

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
        frontCam.requestedFPS = FPS;
        frontCam.Play();
        background.texture = frontCam;

        DateTimeOffset now = DateTimeOffset.UtcNow;
        long currentTime = now.ToUnixTimeMilliseconds();
        long previousTime = 0;
        // Debug.Log(currentTime);

        faceDetector = new CascadeClassifier(cascadeClassifierPath);

        now = DateTimeOffset.UtcNow;
        previousTime = currentTime;
        currentTime = now.ToUnixTimeMilliseconds();
        // Debug.Log("Start 1");
        // Debug.Log(currentTime - previousTime);

        // model = ModelLoader.Load((NNModel)modelSourcePath);
        model = ModelLoader.Load((NNModel)Resources.Load(modelSourcePath));

        now = DateTimeOffset.UtcNow;
        previousTime = currentTime;
        currentTime = now.ToUnixTimeMilliseconds();
        // Debug.Log("Start 2");
        // Debug.Log(currentTime - previousTime);

        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.CSharpBurst, model); // synchronized execution with CPU usage

        now = DateTimeOffset.UtcNow;
        previousTime = currentTime;
        currentTime = now.ToUnixTimeMilliseconds();
        // Debug.Log("Start 3");
        // Debug.Log(currentTime - previousTime);
    }

    // Update is called once per frame
    void Update()
    {
        if(!camAvailable)
            return;

        DateTimeOffset now = DateTimeOffset.UtcNow;
        long previousTime = 0;
        long currentTime = now.ToUnixTimeMilliseconds();
        // Debug.Log("Update 1");
        // Debug.Log(currentTime - previousTime);

        float ratio = (float)frontCam.width / (float)frontCam.height;
        fit.aspectRatio = ratio;

        float scaleY = frontCam.videoVerticallyMirrored ? -1f : 1f;
        background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);
        int orient = -frontCam.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);

        now = DateTimeOffset.UtcNow;
        previousTime = currentTime;
        currentTime = now.ToUnixTimeMilliseconds();
        // Debug.Log("Update 2");
        // Debug.Log(currentTime - previousTime);

        frame = new Mat(frontCam.height, frontCam.width, CvType.CV_8UC4);
        Utils.webCamTextureToMat(frontCam, frame);
        MatOfRect matOfRectFaces = new MatOfRect();
        try
        {
            faceDetector.detectMultiScale(frame, matOfRectFaces);            
        }
        catch {}

        now = DateTimeOffset.UtcNow;
        previousTime = currentTime;
        currentTime = now.ToUnixTimeMilliseconds();
        // Debug.Log("Update 3");
        // Debug.Log(currentTime - previousTime);

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
            Texture2D resizedGrayFaceTexture = new Texture2D(inputSize, inputSize, TextureFormat.Alpha8, false);
            Utils.matToTexture2D(resizedGrayFace, resizedGrayFaceTexture);

            now = DateTimeOffset.UtcNow;
            previousTime = currentTime;
            currentTime = now.ToUnixTimeMilliseconds();
            // Debug.Log("Update 4");
            // Debug.Log(currentTime - previousTime);

            Tensor tensor = new Tensor(resizedGrayFaceTexture);
            if (worker == null) return;

            now = DateTimeOffset.UtcNow;
            previousTime = currentTime;
            currentTime = now.ToUnixTimeMilliseconds();
            // Debug.Log("Update 5-1");
            // Debug.Log(currentTime - previousTime);

            output = worker.Execute(tensor).PeekOutput();

            now = DateTimeOffset.UtcNow;
            previousTime = currentTime;
            currentTime = now.ToUnixTimeMilliseconds();
            // Debug.Log("Update 5-2");
            // Debug.Log(currentTime - previousTime);

            int index = output.ArgMax()[0];

            now = DateTimeOffset.UtcNow;
            previousTime = currentTime;
            currentTime = now.ToUnixTimeMilliseconds();
            // Debug.Log("Update 5-3");
            // Debug.Log(currentTime - previousTime);

            emotion = indexToEmotions[index];
            Debug.Log(emotion);
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

            now = DateTimeOffset.UtcNow;
            previousTime = currentTime;
            currentTime = now.ToUnixTimeMilliseconds();
            // Debug.Log("Update 5");
            // Debug.Log(currentTime - previousTime);
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
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID
    using UnityEngine.Android;
#elif UNITY_IOS
    using UnityEngine.iOS;
#endif

public class CameraAuthorizer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #if PLATFORM_ANDROID
        if(!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }
        #elif PLATFORM_IOS
        if(!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }
        #endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
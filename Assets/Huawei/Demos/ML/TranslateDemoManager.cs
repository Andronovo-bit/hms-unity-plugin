using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateDemoManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("TranslateDemo Start");
        var instance = HMSTranslateMLKitManager.Instance;
        //instance.GetLocalAllLanguages();
        // instance.StartTranslate("How are you ?");

        instance.StartTranslate2("Where are you from ?");

        // instance.StartTranslateRemote("What is your name ?");
    }

    // Update is called once per frame
    void Update()
    {

    }
}

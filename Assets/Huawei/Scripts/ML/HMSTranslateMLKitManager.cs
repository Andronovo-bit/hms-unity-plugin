using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HmsPlugin;
using HuaweiMobileServices.Base;
using HuaweiMobileServices.ML.Common;
using HuaweiMobileServices.ML.DownloadModel;
using HuaweiMobileServices.ML.Translate;
using HuaweiMobileServices.ML.Translate.Cloud;
using HuaweiMobileServices.ML.Translate.Local;
using UnityEngine;
public class HMSTranslateMLKitManager : HMSManagerSingleton<HMSTranslateMLKitManager>
{

    private const string TAG = "[HMS] HMS HMSTranslateMLKitManager ";
    private readonly string API_KEY;

    private MLLocalTranslateSetting mlLocalTranslateSetting;
    private List<string> localAllLanguages;
    private string sourceLangCode = "en";
    private string targetLangCode = "tr";

    public HMSTranslateMLKitManager()
    {

        API_KEY = HMSMLKitSettings.Instance.Settings.Get(HMSMLKitSettings.MLKeyAPI);
        HMSManagerStart.Start(OnAwake, TAG);
    }
    private void OnAwake()
    {
        Init();
    }
    public void Init()
    {
        try
        {
            MLApplication.Instance.SetApiKey(API_KEY);
            // Create an offline translator.
            mlLocalTranslateSetting = new MLLocalTranslateSetting.Factory()
                // Set the source language code. The ISO 639-1 standard is used. This parameter is mandatory. If this parameter is not set, an error may occur.
                .SetSourceLangCode(sourceLangCode)
                // Set the target language code. The ISO 639-1 standard is used. This parameter is mandatory. If this parameter is not set, an error may occur.
                .SetTargetLangCode(targetLangCode)
                .Create();
            Debug.Log(TAG + "Init: " + mlLocalTranslateSetting.SourceLangCode + " " + mlLocalTranslateSetting.TargetLangCode);
            var test = mlLocalTranslateSetting.Equals(mlLocalTranslateSetting);
            Debug.Log(TAG + "Init: " + test);
            var test2 = mlLocalTranslateSetting.GetHashCode();
            Debug.Log(TAG + "Init: " + test2);
        }
        catch (Exception e)
        {
            Debug.LogError(TAG + e.Message);
        }
    }

    public void GetLocalAllLanguages()
    {
        MLTranslateLanguage.GetLocalAllLanguagesAsync().AddOnSuccessListener((result) =>
        {
            Debug.Log(TAG + "GetLocalAllLanguages: " + result.Count);
        }).AddOnFailureListener((exception) =>
        {
            Debug.LogError(TAG + "GetLocalAllLanguages: " + exception.WrappedCauseMessage);
        });

    }
    public void StartTranslate(string text) //Method2 user prepared model
    {
        // Set the model download policy.
        MLModelDownloadStrategy downloadStrategy = new MLModelDownloadStrategy.Factory()
            .NeedWifi() // It is recommended that you download the package in a Wi-Fi environment.
            .Create();
        // Create a download progress listener.
        var modelDownloadListener = new MLModelDownloadListener(new HMSTranslateDownloadModelListenerManager());

        MLLocalTranslator localTranslator = MLTranslatorFactory.Instance.GetLocalTranslator(mlLocalTranslateSetting);

        localTranslator.PraparedModel(downloadStrategy, modelDownloadListener).AddOnSuccessListener((result) =>
        {
            Debug.Log(TAG + "StartTranslate: PraparedModel: " + result);

            localTranslator.TranslateAsync(text).AddOnSuccessListener((result) =>
            {
                Debug.Log(TAG + "StartTranslate: TranslateAsync: " + result);
            }).AddOnFailureListener((exception) =>
            {
                Debug.LogError(TAG + "StartTranslate: TranslateAsync: " + exception.WrappedCauseMessage);
            });

        }).AddOnFailureListener((exception) =>
        {
            Debug.LogError(TAG + "StartTranslate: PraparedModel: " + exception.WrappedCauseMessage);
        });
    }
    public void StartTranslate2(string text) //Method1
    {
        // After the download is successful, translate text in the onSuccess callback.

        // Obtain the model manager.
        MLLocalModelManager manager = MLLocalModelManager.Instance;
        MLLocalTranslatorModel model = new MLLocalTranslatorModel.Factory(targetLangCode).Create();
        // Set the model download policy.
        MLModelDownloadStrategy downloadStrategy = new MLModelDownloadStrategy.Factory()
            .NeedWifi() // It is recommended that you download the package in a Wi-Fi environment.
        // Set the site region. Currently, the following values are supported: REGION_DR_CHINA, REGION_DR_GERMAN, REGION_DR_SINGAPORE, and REGION_DR_RUSSIA. (The site region must be the same as the access site selected in AppGallery Connect.)
            .SetRegion(MLModelDownloadStrategy.REGION_DR_GERMAN)
            .Create();
        // Create a download progress listener.
        var modelDownloadListener = new MLModelDownloadListener(new HMSTranslateDownloadModelListenerManager());
        // Download the model.
        manager.DownloadModel(model, downloadStrategy, modelDownloadListener).AddOnSuccessListener((result) =>
        {
            Debug.Log(TAG + "StartTranslate2: DownloadModel: " + result);
            // Create a local translator.
            MLLocalTranslator localTranslator = MLTranslatorFactory.Instance.GetLocalTranslator(mlLocalTranslateSetting);
            // Translate text.
            localTranslator.TranslateAsync(text).AddOnSuccessListener((result) =>
            {
                Debug.Log(TAG + "StartTranslate2: TranslateAsync: " + result);
            }).AddOnFailureListener((exception) =>
            {
                Debug.LogError(TAG + "StartTranslate2: TranslateAsync: " + exception.WrappedCauseMessage);
            });
        }).AddOnFailureListener((exception) =>
        {
            Debug.LogError(TAG + "StartTranslate2: DownloadModel: " + exception.WrappedCauseMessage);
        });
    }

    public void StartTranslateRemote(string text)
    {
        MLRemoteTranslateSetting mLRemoteTranslateSetting = new MLRemoteTranslateSetting.Factory()
            .SetSourceLangCode(sourceLangCode)
            .SetTargetLangCode(targetLangCode)
            .Create();

        MLRemoteTranslator mlRemoteTranslator = MLTranslatorFactory.Instance.GetRemoteTranslator(mLRemoteTranslateSetting);

        try
        {
            MLTranslateLanguage.GetCloudAllLanguagesAsync().AddOnSuccessListener((result) =>
            {
                Debug.Log(TAG + "StartTranslateRemote: GetCloudAllLanguagesAsync: " + result.Count);
            }).AddOnFailureListener((exception) =>
            {
                Debug.LogError(TAG + "StartTranslateRemote: GetCloudAllLanguagesAsync: " + exception.WrappedCauseMessage);
            });
        }
        catch (Exception e)
        {
            Debug.LogError(TAG + "StartTranslateRemote: " + e.Message);
        }

        mlRemoteTranslator.TranslateAsync(text).AddOnSuccessListener((result) =>
        {
            Debug.Log(TAG + "StartTranslateRemote: TranslateAsync: " + result);

            if (mlRemoteTranslator != null)
            {
                mlRemoteTranslator.Stop();
            }

        }).AddOnFailureListener((exception) =>
        {
            Debug.LogError(TAG + "StartTranslateRemote: TranslateAsync: " + exception.WrappedCauseMessage);
        });
    }

}

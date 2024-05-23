using System;
using UnityEngine;
using static HuaweiMobileServices.ML.DownloadModel.MLModelDownloadListener;

namespace HmsPlugin
{
    public class HMSTranslateDownloadModelListenerManager : IMLModelDownloadListener
    {
        public Action<long, long> OnProcessAction;
        public void OnProcess(long alreadyDownLength, long totalLength)
        {
            Debug.Log("Downloaded: " + alreadyDownLength + " Total: " + totalLength);
            OnProcessAction?.Invoke(alreadyDownLength, totalLength);
        }
    }
}

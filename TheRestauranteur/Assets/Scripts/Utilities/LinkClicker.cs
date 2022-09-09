using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

[RequireComponent(typeof(Button))]
public class LinkClicker : MonoBehaviour
{
    public void OpenLink(string _url)
    {
        #if UNITY_WEBGL
        OpenURLInExternalWindow(_url);
        #else
        Application.OpenURL(_url);
        #endif
    }

    public void ReloadLink(string _url)
    {
        #if UNITY_WEBGL
        OpenURLInWindow(_url);
        #else
        Application.OpenURL(_url);
        #endif
    }

    #if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void OpenURLInExternalWindow(string url);

    [DllImport("__Internal")]
    private static extern void OpenURLInWindow(string url);
    #endif
}

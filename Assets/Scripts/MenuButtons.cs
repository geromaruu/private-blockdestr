using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    private void Awake()
    {
        if (Application.isEditor == false)
          Debug.unityLogger.logEnabled = false;
    }

    public void LoadScene(string Name)
    {
        SceneManager.LoadScene(Name);
    }


}

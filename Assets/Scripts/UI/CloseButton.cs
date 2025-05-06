using UnityEngine;
using UnityEngine.UI; 

public class CloseButton : MonoBehaviour
{
    public Button myButton; 
    public GameObject gameOverPopup; 

    void Start()
    {
        myButton.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        gameOverPopup.SetActive(false);
    }


}
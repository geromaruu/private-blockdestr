using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

[CreateAssetMenu]
[System.Serializable]
public class SqaureTextureData : ScriptableObject
{
    [System.Serializable]   
    public class TextureData
    {
        public Sprite texture;
        public Config.SquareColour squareColour;
    }

    public int tresholdVal = 10;
    private const int StartTresholdVal = 100;
    public List<TextureData> activeSquareTextures;

    public Config.SquareColour currentColor;
    private Config.SquareColour _nextColor;

    public int GetCurrentColorIndex()
    {
        var currentIndex = 0;

        for (int index = 0; index < activeSquareTextures.Count; index++)
        {
            if (activeSquareTextures[index].squareColour == currentColor)
            {
                currentIndex = index;
            }

        }
        return currentIndex;    
    }

    public void UpdateColors(int current_score)
    {
        currentColor = _nextColor;
        var currentColorIndex = GetCurrentColorIndex();

        if (currentColorIndex == activeSquareTextures.Count - 1)
          _nextColor = activeSquareTextures[0].squareColour;
        else
         _nextColor = activeSquareTextures[currentColorIndex + 1].squareColour;

        tresholdVal = StartTresholdVal + current_score;
        
    }

    public void SetStartColor()
    {
        tresholdVal = StartTresholdVal;
        currentColor = activeSquareTextures[0].squareColour;
        _nextColor = activeSquareTextures[1].squareColour;
    }

    private void Awake()
    {
        SetStartColor();    
    }

    private void OnEnable()
    {
        SetStartColor();
    }
}


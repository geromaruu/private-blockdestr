using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BestScoreData
{
    public int score = 0;
}
public class Scores : MonoBehaviour
{
    public SqaureTextureData squareTextureData;
    public Text scoreText;
    public Text bestScoreText;
    private bool newBestScore_ = false;
    private BestScoreData bestScores_ = new BestScoreData();
    private int currentScores_;

    private string bestScoreKey_ = "bestscrdat";

    private void Awake()
    {
        if (BinaryDataStream.Exist(bestScoreKey_))
        {
            StartCoroutine(ReadDataFile());
        }
    }

    private IEnumerator ReadDataFile()
    {
        bestScores_ = BinaryDataStream.Read<BestScoreData>(bestScoreKey_);
        yield return new WaitForEndOfFrame();
        Debug.Log("Best Score - " + bestScores_.score);
        UpdateScoreText();
    }

    void Start()
    {

        currentScores_ = 0;
        newBestScore_ = false;
        squareTextureData.SetStartColor();
        UpdateScoreText();
    }

    private void OnEnable()
    {
        GameEvents.AddScores += AddScores;
        GameEvents.GameOver += SaveBestScores;
    }

    private void OnDisable()
    {
        GameEvents.AddScores -= AddScores;
        GameEvents.GameOver -= SaveBestScores;
    }

    public void SaveBestScores(bool newBestScores) 
    {
        BinaryDataStream.Save<BestScoreData>(bestScores_, bestScoreKey_);
    }
    private void AddScores(int scores)
    {
        currentScores_ += scores;

        if (currentScores_ >= bestScores_.score)
        {
            newBestScore_ = true;
            bestScores_.score = currentScores_;

        }
        UpdateSquareColor();
        UpdateScoreText();
    }

    private void UpdateSquareColor()
    {
        if (currentScores_ >= squareTextureData.tresholdVal)
        {
            squareTextureData.UpdateColors(currentScores_);
        }
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = currentScores_.ToString();

        if (bestScoreText != null)
            bestScoreText.text =  bestScores_.score.ToString();
    }

}

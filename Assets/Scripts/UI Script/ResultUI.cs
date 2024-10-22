using UnityEngine;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    [SerializeField] private Sprite blueWinSpr, redWinSpr;
    [SerializeField] private Image winImg;
    
    void Start()
    {
        winImg.sprite = (GameController.Winner == Winner.BluePlayer)
            ? blueWinSpr : redWinSpr;
    }
}

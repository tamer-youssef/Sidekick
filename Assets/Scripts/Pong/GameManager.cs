using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Ball")]
    public GameObject ball;

    [Header("Player 1")]
    public GameObject player1Paddle;
    public GameObject player1Goal;

    [Header("Player AI")]
    public GameObject playerAIPaddle;
    public GameObject playerAIGoal;

    [Header("Score UI")]
    public GameObject Player1Text;
    public GameObject PlayerAIText;

    private int Player1Score;
    private int PlayerAIScore;

    public void Player1Scored()
    {
        Player1Score++;
        Player1Text.GetComponent<TextMeshProUGUI>().text = Player1Score.ToString();
    }

    public void PlayerAIScored()
    {
        PlayerAIScore++;
        PlayerAIText.GetComponent<TextMeshProUGUI>().text = PlayerAIScore.ToString();
    }

    private void ResetPosition()
    {
        ball.GetComponent<PongBall>().Reset();
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject == player1Goal)
        {
            Debug.Log("goal");
            PlayerAIScored();
            ResetPosition();
        }
        else if (col.gameObject == playerAIGoal)
        {
            Debug.Log("goal");
            Player1Scored();
            ResetPosition();
        }
    }
}
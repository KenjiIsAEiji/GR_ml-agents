using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スコアエリア用クラス
/// </summary>
public class ScoreArea : MonoBehaviour
{
    public GameManager gameManager;
    public int agentId;

    void OnTriggerEnter(Collider other)
    {
        gameManager.EndEpisode(agentId);
    }
}

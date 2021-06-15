using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

/// <summary>
/// ゲーム管理クラス
/// </summary>

public class GameManager : MonoBehaviour
{
    private static GameManager mInstance;

    public static GameManager Instance {
        get{
            if(mInstance == null){
                GameObject obj = new GameObject("GameManager");
                mInstance = obj.AddComponent<GameManager>();
            }
            return mInstance;
        }
        set{
            
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        AgentReset();
    }
    
    public void AgentReset()
    {
        
    }

    public void EndEpisode(int agentId)
    {
        // 
        AgentReset();
    }
}

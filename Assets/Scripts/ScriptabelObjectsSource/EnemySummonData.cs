using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EnemySummonData", menuName = "Create EnemeySummonData")]
public class EnemySummonData : ScriptableObject
{   
    
    public GameObject enemeyPrefab;
    public int enemyId;
    public float health;
    public int score;
    public float speed;
    public int reward;
}

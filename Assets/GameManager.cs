using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void AddScore(int point)
    {
        Debug.Log($"+{point} điểm!");
        // TODO: cập nhật UI điểm
    }
}

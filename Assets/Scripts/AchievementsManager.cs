using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Auth;

public class AchievementsManager : Singleton<AchievementsManager>
{
    private DatabaseReference databaseReference;
    private int bestScore;
    private long playTime;
    public int BestScore { get { return bestScore; } set { bestScore = value; } }

    private FirebaseUser user;

    public void Init()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        user = FirebaseAuth.DefaultInstance.CurrentUser;
        GetAch();
    }
    public void GetAch()
    {   
        databaseReference.Child("users").Child(user.UserId).Child("achievements").GetValueAsync().ContinueWith(task =>
        {
            Debug.Log("ddd");
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {

                DataSnapshot snapshot = task.Result;
                playTime = (long)snapshot.Child("ach2").Child("currentValue").Value;
                Debug.Log(snapshot.Child("ach1").Child("name").Value);
                Debug.Log("ChilderenCount: "+snapshot.ChildrenCount);
            }
        });
    }

    public void CheckAchievement(int score)
    {
        if (PlayerPrefs.GetInt("Ach1") != 1 && score >= 5)
        {
            PlayerPrefs.SetInt("Ach1", 1);
            PlayerPrefs.Save();
            UIManager.Instance.testText.text = "업적1 달성";
        }

        BestScore = score;
        databaseReference.Child("users").Child(user.UserId).Child("achievements").Child("ach1").Child("currentValue").SetValueAsync(score);

        CheckClearAchievement("ach1");
    }

    public void CheckPlayTime()
    {
        playTime++;
        databaseReference.Child("users").Child(user.UserId).Child("achievements").Child("ach2").Child("currentValue").SetValueAsync(playTime);
        Debug.Log(playTime);
        CheckClearAchievement("ach2");
    }

    public void CheckClearAchievement(string ach)
    {
        //업적 달성 여부 체크
        databaseReference.Child("users").Child(user.UserId).Child("achievements").Child(ach).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                Debug.Log("업적 달성 여부 체크"+ach);
                DataSnapshot snapshot = task.Result;
                long reqValue = (long)snapshot.Child("reqValue").Value;
                long currentValue = (long)snapshot.Child("currentValue").Value;
                if (!(bool)snapshot.Child("isClear").Value && reqValue <= currentValue)
                {
                    databaseReference.Child("users").Child(user.UserId).Child("achievements").Child(ach).Child("isClear").SetValueAsync(true);
                    UIManager.Instance.testText.text = snapshot.Child("name").Value + "달성: " + snapshot.Child("reward").Value + "골드 획득";
                }
            }
        });
    }
}

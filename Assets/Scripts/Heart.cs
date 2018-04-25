using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Firebase.Database;
using Firebase.Auth;

public class Heart : Singleton<Heart> {
    public TextMeshProUGUI currentHeartUI;
    public TextMeshProUGUI chargingTimeUI;

    private bool isStart;
    private long currentHeart;

    private System.DateTime heartTime;
    private System.TimeSpan heartTimeSpan;

    private DatabaseReference databaseReference;

    

#if UNITY_EDITOR
    private User user;
#else
    private FirebaseUser user;
#endif

    public long CurrentHeart
    {
        get
        {
            return currentHeart;
        }

        set
        {
            if (currentHeart > value && currentHeart == Constants.MAX_HEART)
            {
                WriteHeartTime();
                heartTime = System.DateTime.Now;
                StartCoroutine("HeartTimer");
            }

            if(currentHeart < value && value >= Constants.MAX_HEART)
            {
                SetChargingTime("MAX");
                StopCoroutine("HeartTimer");
            }

            currentHeart = value;
            currentHeartUI.text = value.ToString();

            databaseReference.Child("users").Child(user.UserId).Child("heart").SetValueAsync(value);
        }
    }

    private void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
#if UNITY_EDITOR
        user = new User("username", "userEmail");
        user.UserId = "userId";
        databaseReference.Child("users").Child(user.UserId).Child("heart").SetValueAsync(Constants.MAX_HEART);
        WriteHeartTime();
#else
        user = FirebaseAuth.DefaultInstance.CurrentUser;
        
#endif
        isStart = true;
        databaseReference.Child("users").Child(user.UserId).Child("heart").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {

                DataSnapshot snapshot = task.Result;

                if (snapshot == null)
                {
                    databaseReference.Child("users").Child(user.UserId).Child("heart").SetValueAsync(Constants.MAX_HEART);
                    CurrentHeart = Constants.MAX_HEART;
                    return;
                }
                CurrentHeart = (long)snapshot.Value;
                TryIncreaseHeartByTime();
            }
        });
    }

    public void WriteHeartTime()
    {
        databaseReference.Child("users").Child(user.UserId).Child("heartTime").SetValueAsync((System.DateTime.Now - System.DateTime.MinValue).TotalSeconds);
    }

    public void TryIncreaseHeartByTime()
    {
        databaseReference.Child("users").Child(user.UserId).Child("heartTime").GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                // Handle the error...

            }
            else if (task.IsCompleted)
            {
                double dbTime = (double)task.Result.Value;
                double cTime = (System.DateTime.Now - System.DateTime.MinValue).TotalSeconds;

                if (cTime - dbTime >= Constants.CHARGING_TIME)
                {
                    
                    heartTime = heartTime.AddSeconds(Constants.CHARGING_TIME);
                    SetChargingTime(Constants.CHARGING_TIME - (int)(cTime-dbTime)% Constants.CHARGING_TIME);

                    int f = (int)(cTime - dbTime) / Constants.CHARGING_TIME;

                    if (currentHeart + f >= Constants.MAX_HEART)
                        CurrentHeart = Constants.MAX_HEART;
                    else
                    {
                        CurrentHeart += f;
                        databaseReference.Child("users").Child(user.UserId).Child("heartTime").SetValueAsync(dbTime + Constants.CHARGING_TIME *f);
                    }
                }

                if (isStart)
                {
                    isStart = false;
                    heartTime = System.DateTime.Now.AddSeconds(-(int)(cTime - dbTime) % Constants.CHARGING_TIME);
                    if (currentHeart < Constants.MAX_HEART)
                        StartCoroutine("HeartTimer");
                }
            }
        });
    }

    public void SetChargingTime(int value)
    {
        chargingTimeUI.text = (value / 60).ToString("D2") + ":" + (value % 60).ToString("D2");
    }

    public void SetChargingTime(string value)
    {
  
        chargingTimeUI.text = value;
    }

    IEnumerator HeartTimer()
    {
        while (true)
        {
            heartTimeSpan = System.DateTime.Now - heartTime;
            int sec = Constants.CHARGING_TIME - (int)heartTimeSpan.TotalSeconds;
            SetChargingTime(sec);
            if (sec <= 0)
            {
                TryIncreaseHeartByTime();
                Debug.Log("TryIncreaseHeartByTime");
            }

            yield return new WaitForSecondsRealtime(1);
        }
    }

    public bool TryDecreaseHeart()
    {
        //if (databaseReference.Child("users").Child(user.UserId).Child("heart").GetValueAsync().IsCompleted)
        //    currentHeart = (int)databaseReference.Child("users").Child(user.UserId).Child("heart").GetValueAsync().Result.Value;
        //else
        //    currentHeart = 0;
        if (currentHeart <= 0)
            return false;

        CurrentHeart--;
        return true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Auth;

public class AchievementsManager : Singleton<AchievementsManager>
{
    private DatabaseReference databaseReference;


    private FirebaseUser user;

    public void GetAch()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        user = FirebaseAuth.DefaultInstance.CurrentUser;
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

                Debug.Log(snapshot.HasChild("ach3"));
                
            }
        });
    }
}

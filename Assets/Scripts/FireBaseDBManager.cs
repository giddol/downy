﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Firebase.Auth;

public class User
{
    public string username;
    public string email;
    public string UserId;
    public int heart;
    public int bestScore;
    public string achievements; 

    public User()
    {
    }

    public User(string username, string email)
    {
        this.username = username;
        this.email = email;
    }

    public User(string username, string email, string userId)
    {
        this.username = username;
        this.email = email;
        UserId = userId;
        heart = Constants.MAX_HEART;
        bestScore = 0;
        achievements = "";
    }
}

public class FireBaseDBManager : Singleton<FireBaseDBManager>
{
    private DatabaseReference databaseReference;
    private FirebaseUser user;
    public UnityEngine.UI.Text text;
    // Use this for initialization
    void Start () {
        // Set these values before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://down-d257e.firebaseio.com/");
        FirebaseApp.DefaultInstance.SetEditorP12FileName("downy-test-74f7c5fd86e9.p12");
        FirebaseApp.DefaultInstance.SetEditorServiceAccountEmail("test-748@down-d257e.iam.gserviceaccount.com");
        FirebaseApp.DefaultInstance.SetEditorP12Password("notasecret");
        // Get the root reference location of the database.
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

        user = FirebaseAuth.DefaultInstance.CurrentUser;

        //WriteNewUser(user.UserId, user.DisplayName, user.Email);
    }

    private void WriteNewUser(string userId, string name, string email)
    {
        User user = new User(name, email);
        string json = JsonUtility.ToJson(user);

        databaseReference.Child("users").Child(userId).SetRawJsonValueAsync(json);

        Debug.Log("업데이트 완료");
    }


}

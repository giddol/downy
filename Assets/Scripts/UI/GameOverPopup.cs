using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPopup : MonoBehaviour
{
    [SerializeField]
    private Text _score = null;
    [SerializeField]
    private Text _best = null;

    [SerializeField]
    private GameObject _newBestScore = null;

    // Use this for initialization
    public void Show () {
        gameObject.SetActive(true);

        _newBestScore.SetActive(Manager.Instance.IsBestScore);
        _score.text = Manager.Instance.Score.ToString();
        _best.text = Manager.Instance.BestScore.ToString();
	}
	
	// Update is called once per frame
	public void OkButton () {
        Manager.Instance.Replay();
	}

    public void FbShare()
    {
        Manager.Instance.FbShare();
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIMapItem : MonoBehaviour {
    public GameObject levelArea;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetLevelAreaVisible(bool visible)
    {
        levelArea.SetActive(visible);
    }

    public void OnStartSingleGame()
    {
        int r = Random.Range(0, 2);
        if (r == 1)
            SceneManager.LoadScene("battleScene_dark");
        else
            SceneManager.LoadScene("battleScene_dark");
    }
}

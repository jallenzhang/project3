using UnityEngine;
using System.Collections;

public class UIBattle : MonoBehaviour {
    public GameObject btnGameStatus;

    enum GameStatus
    {
        Play,
        Pause,
    }

    private GameStatus m_status = GameStatus.Play;

    private const string m_pauseButtonStr = "UI_Battle_Button_Pause";
    private const string m_playButtonStr = "UI_Battle_Button_Play";
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnStatusChange()
    {
        switch(m_status)
        {
            case GameStatus.Play:
                m_status = GameStatus.Pause;
                btnGameStatus.GetComponent<UGUIImage>().ChangeImageByName(m_playButtonStr);
                UIPauseDialog.PopUp(new UIPauseDialogParam() { Callback = () => {
                    OnStatusChange(); 
                }
                });
                break;
            case GameStatus.Pause:
                m_status = GameStatus.Play;
                btnGameStatus.GetComponent<UGUIImage>().ChangeImageByName(m_pauseButtonStr);
                break;
        }
    }

    RaycastHit lastHit;
    GameObject hoveredObject;
    void OnGUI()
    {
        Ray ray = GetComponentInParent<Camera>().ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out lastHit, 1000, GetComponentInParent<Camera>().cullingMask))
        {
            hoveredObject = lastHit.collider.gameObject;
        }
        else
        {
            hoveredObject = null;
        }

        if (hoveredObject != null)
        {
            GUILayout.Label("Last Hit: " + hoveredObject.name);
        }
        else
        {
            GUILayout.Label(" ");
        }
    }
}

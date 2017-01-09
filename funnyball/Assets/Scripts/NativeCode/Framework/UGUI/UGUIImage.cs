using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class UGUIImage : MonoBehaviour {
    public string imageName;
    private Image m_image;
	// Use this for initialization
	void Start () {
        m_image = GetComponent<Image>();
        m_image.sprite = CreateImage(loadSprite(imageName));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ChangeImageByName(string name)
    {
        imageName = name;
        m_image.sprite = loadSprite(name);
    }

    private Sprite CreateImage(Sprite sprite)
    {
        gameObject.layer = LayerMask.NameToLayer("UI");
        gameObject.transform.parent = transform;
        gameObject.transform.localScale = Vector3.one;
        return sprite;
    }

    private Sprite loadSprite(string spriteName)
    {
        GameObject go = Resources.Load<GameObject>("Prefabs/UI/Sprite/" + spriteName);
        if (go != null)
        {
            return go.GetComponent<SpriteRenderer>().sprite;
        }
        return null;
    }
}

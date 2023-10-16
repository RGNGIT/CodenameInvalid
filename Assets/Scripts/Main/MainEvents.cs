using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainEvents : MonoBehaviour
{
    public GameObject Block;
    public Transform Content;

    private void Awake()
    {
        SceneManager.LoadScene((int)Constants.EAdditiveScene.Controls, LoadSceneMode.Additive);
        PlaceBlocks();
    }

    public void PlaceBlock() 
    {
        GameObject block = Instantiate(Block);
        block.transform.parent = Content.transform;
        block.transform.localPosition = new Vector2(0, -(1256f - 965f));
    }

    void PlaceBlocks()
    {
        PlaceBlock();
    }
}

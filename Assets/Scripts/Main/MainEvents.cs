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

    float y = -(1256f - 965f) - 1250;
    public void PlaceBlock(string header) 
    {
        GameObject block = Instantiate(Block);
        block.GetComponentInChildren<Main_BlockBase>().Header.text = header;
        block.transform.parent = Content.transform;
        block.transform.localPosition = new Vector2(0, y);
    }

    void PlaceBlocks()
    {
        for(int i = 0; i < 4; i++) 
        {
            PlaceBlock("Block " + i);
            y -= 635f;
        }
    }
}

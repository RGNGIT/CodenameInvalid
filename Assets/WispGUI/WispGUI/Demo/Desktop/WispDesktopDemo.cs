using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispDesktopDemo : MonoBehaviour
{
    public WispButton notepadBtn;
    public WispButton pictureBtn;
    public WispButton powerBtn;
    public WispButton calendarBtn;

    public GameObject notepadWindow;
    public GameObject pictureWindow;
    public GameObject powerWindow;
    public GameObject calendarWindow;

    private WispVisualComponent me;
    
    // Start is called before the first frame update
    void Start()
    {
        me = GetComponent<WispVisualComponent>();
        
        notepadBtn.AddOnClickAction(NotepadBtnOnClick);
        pictureBtn.AddOnClickAction(PictureBtnOnClick);
        powerBtn.AddOnClickAction(PowerBtnOnClick);
        calendarBtn.AddOnClickAction(CalendarBtnOnClick);
    }

    private void NotepadBtnOnClick()
    {
        GameObject go = Instantiate(notepadWindow);
        go.SetActive(true);
        go.GetComponent<WispVisualComponent>().SetParent(me, true, true);
        go.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
    }
    
    private void PictureBtnOnClick()
    {
        GameObject go = Instantiate(pictureWindow);
        go.SetActive(true);
        go.GetComponent<WispVisualComponent>().SetParent(me, true, true);
        go.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
    }

    private void PowerBtnOnClick()
    {
        GameObject go = Instantiate(powerWindow);
        go.SetActive(true);
        go.GetComponent<WispVisualComponent>().SetParent(me, true, true);
        go.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
    }

    private void CalendarBtnOnClick()
    {
        GameObject go = Instantiate(calendarWindow);
        go.SetActive(true);
        go.GetComponent<WispVisualComponent>().SetParent(me, true, true);
        go.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
    }
}
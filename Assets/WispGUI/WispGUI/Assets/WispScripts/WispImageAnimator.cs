using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WispImageAnimator : MonoBehaviour {

	public Sprite[] Sprites;
	public float FrameDuration;

	protected Image imageComponent;
	protected int currentFrame = 0;
	protected int lastFrame;
	protected float lastFrameTime;

	// Use this for initialization
	void Start () 
	{
		imageComponent = GetComponent<Image> ();
		lastFrame = Sprites.Length - 1;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Time.time > lastFrameTime + FrameDuration)
			LoadNextFrame ();
	}

	// ...
	protected void LoadNextFrame ()
	{
		if (currentFrame == lastFrame)
			currentFrame = 0;
		else
			currentFrame++;

		imageComponent.sprite = Sprites [currentFrame];

		lastFrameTime = Time.time;
	}
}
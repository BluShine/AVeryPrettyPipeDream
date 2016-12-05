/*
 * Copyright 2010 - David Koontz
 * Please submit bug reports and feature requests/suggestions to david@koontzfamily.org
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// <para>
/// Fills a GUITexture in a typewriter style.  You can assign a GUIText to be written to 
/// or if left blank the component will try to use an attached GUITexture.  After setting the
/// text, use the Write method to start the writing effect.
/// </para>
/// <para>
/// To accelerate the typing speed when a button is pressed simply increase the charactersPerSecond
/// attribute and then set it back when releasing the button.  You can use the Completed property to
/// determine if the writing process is still ongoing.
/// </para>
/// </summary>
public class Typewriter : MonoBehaviour {
	public const string VERSION = "1.3";

    public string initialText;
    public string[] additionalTextSections;
    public int textWidth = 60;
    public GUIText guiTextComponent;
    public TextMesh textMesh;

	// These two options are mutually exclusive, if the script waits for external
	// input, it will not automatically advance after a delay.
	public bool waitForExternalInputBetweenSections;
	public float delayBetweenTextSections;
    
	public bool breakOnWholeWords = true;
    public bool animated = true;
    public float charactersPerSecond = 20;
    public bool startAutomatically = false;
    public GameObject onCompleteTarget;
    public string onCompleteEvent;
    public string onCompleteArgs;
    
	public bool Completed { get { return finished; }}
	public bool Active { get { return animating; }}
	
	List<string> lines;
	bool animating = false;
	bool finished = false;
	bool usingGUIText = true;
	string[] textSections;
	bool waitingForExternalInput;
	
	public void Start() {
		//textSections = new string[1 + additionalTextSections.Length];
        //textSections[0] = initialText;
		
		System.Array.Copy(additionalTextSections, 0, textSections, 1, additionalTextSections.Length);
		
		if(startAutomatically) {
			Write();
		}
	}

    public void SetText(string text)
    {
        textSections = new string[1];
        textSections[0] = text;
    }
	
	// Starts the text writing to the assigned GUIText;
	public void Write() {
		if(null == guiTextComponent && null == textMesh) {
			guiTextComponent = GetComponent<GUIText>();
			if(null == guiTextComponent) {
				textMesh = GetComponent<TextMesh>();
				if(null == textMesh) {
					throw new System.InvalidOperationException("No GUIText or Textmesh detected on this GameObject and none were assigned.");
				}
				else {
					usingGUIText = false;
				}
			}
		}
		else {
			if(null == guiTextComponent) {
				usingGUIText = false;
			}
		}
		
		StartCoroutine("WriteSections");
	}
	
	public void Stop() {
		StopCoroutine("WriteSections");
		animating = false;
	}

	public void Advance() {
		waitingForExternalInput = false;
	}
	
	IEnumerator WriteSections() {
		foreach(var section in textSections) {
			if(string.IsNullOrEmpty(section)) {
				continue;
			}
			
			var formattedText = FormatText(section);
			
			if(animated) {
				animating = true;
				int currentLength = 1;
				
				while(animating && currentLength <= formattedText.Length) {
					if(usingGUIText) {
						guiTextComponent.text = formattedText.Substring(0, currentLength);
					}
					else {
						textMesh.text = formattedText.Substring(0, currentLength);
					}
					// This has to be calculated each time because the rate can change
					// during playback, such as with a "hold button to accelerate text"
					// type script.
					yield return new WaitForSeconds(1/charactersPerSecond);
					++currentLength;
				}
			}
			else {
				if(usingGUIText) {
					guiTextComponent.text = formattedText;
				}
				else {
					textMesh.text = formattedText;
				}
			}

			if(waitForExternalInputBetweenSections) {
				waitingForExternalInput = true;
				while(waitingForExternalInput) {
					yield return new WaitForEndOfFrame();
				}
			}
			else {
				yield return new WaitForSeconds(delayBetweenTextSections);
			}
		}
		
		if(onCompleteEvent.Length > 0) {
			if(onCompleteTarget != null) {
				onCompleteTarget.SendMessage(onCompleteEvent, onCompleteArgs);	
			}
			else {
				SendMessage(onCompleteEvent, onCompleteArgs);	
			}
		}
		
		finished = true;
	}
	
	string FormatText(string text) {
		int offset = 0;
		finished = false;
		lines = new List<string>();
		
		if(breakOnWholeWords) {
			int width;
			int adjustedWidth;
			bool endOfText = false;
			
			while(offset < text.Length) {
				width = textWidth;
				if(offset + width > text.Length) {
					width = text.Length - offset;
					endOfText = true;
				}
				
				adjustedWidth = width;
				while(!endOfText && text[offset+adjustedWidth-1] != ' ' && adjustedWidth > 0) {
					--adjustedWidth;
				}
				
				if(0 == adjustedWidth) {
					adjustedWidth = width;
				}
				lines.Add(text.Substring(offset, adjustedWidth));
				
				offset += adjustedWidth;
			}
		}
		else {
			while(offset < text.Length) {
				if(offset + textWidth > text.Length) {
					lines.Add(text.Substring(offset, text.Length - offset));
				}
				else {
					lines.Add(text.Substring(offset, textWidth));
				}
				offset += textWidth;
			}
		}
		
		StringBuilder sb = new StringBuilder();
		foreach(string s in lines) {
			sb.Append(s);
			sb.Append("\n");
		}
		return sb.Remove(sb.Length - 1, 1).ToString();
	}
}
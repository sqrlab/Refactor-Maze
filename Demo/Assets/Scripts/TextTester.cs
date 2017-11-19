﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using UnityEditor;

public class TextTester : MonoBehaviour {

	public Text textComponent; // filled in the inspector or somewhere else

	public List<string> ClickableWords = new List<string>(); 
	List<List<Rect>> rectangles = new List<List<Rect>> ();
	List<List<int>> wordIndexes = new List<List<int>> ();


	public Dictionary<int, string> replacedWords = new Dictionary<int, string> ();

	private TextGenerator generator;
	private void Start() 
	{
		if (textComponent == null) 
		{
			textComponent = GetComponent<Text> ();
		}
		if (textComponent == null)
		{
			Debug.LogError("textComponent was not specified");
			this.enabled = false;
			return;
		}
		//generator = textComponent.cachedTextGenerator;

		replacedWords.Add (1, "pi");

		generator = new TextGenerator (textComponent.text.Length);
		Vector2 extents = textComponent.gameObject.GetComponent<RectTransform>().rect.size;
		generator.Populate (textComponent.text, textComponent.GetGenerationSettings (extents));
		//for (int j = 0; j < ClickableWords.Count; j++) 
		{

			//Get indicies of clickable words
			for (int i = 0; i < ClickableWords.Count; i++) 
			{
				List<int> testIndex = new List<int> ();
				wordIndexes.Add (new List<int>());
				string theText = textComponent.text;
				int index = 0;
				while (theText.Contains (ClickableWords [i])) 
				{
					testIndex.Add (theText.IndexOf (ClickableWords [i]));
					wordIndexes [i].Add (theText.IndexOf (ClickableWords [i]));
					int offset = testIndex [index] + ClickableWords [i].Length + 1;
					int length = theText.Length - offset;

					if (index > 0) 
					{
						testIndex [index] += testIndex [index - 1] + ClickableWords [i].Length + 1;
						wordIndexes [i] [index] += wordIndexes [i] [index - 1] + ClickableWords [i].Length + 1;
					}
					index++;
					if (length > 0)
					{
						theText = theText.Substring (offset, length);
					} 
					else 
					{
						theText = "";
					}
				}
				int indexOfTextQuad = 0;
				rectangles.Add (new List<Rect> ());

				for (int c = 0; c < wordIndexes[i].Count; c++) 
				{

					indexOfTextQuad = wordIndexes[i][c];
					Vector2 upperLeft = new Vector2 (generator.verts [indexOfTextQuad * 4].position.x, generator.verts [indexOfTextQuad * 4].position.y);
					indexOfTextQuad = (wordIndexes[i][c]) + ClickableWords [i].Length;
					Vector2 bottomright = new Vector2 (generator.verts [indexOfTextQuad * 4].position.x, generator.verts [indexOfTextQuad * 4 + 2].position.y);

					/*Vector3 avgPos = (generator.verts [indexOfTextQuad].position +
					                generator.verts [indexOfTextQuad + 1].position +
					                generator.verts [indexOfTextQuad + 2].position +
					                generator.verts [indexOfTextQuad + 3].position) / 4f;

					Vector3 worldPos = textComponent.transform.TransformPoint (bottomright);
					//     worldPos /= canvas.scaleFactor*/

					// Debug.DrawLine(Vector3.zero, new Vector3(1, 0, 0), Color.red);
					Vector3 uleft = textComponent.transform.TransformPoint (upperLeft);
					Vector3 bright = textComponent.transform.TransformPoint (bottomright);
					Vector2 size = bright - uleft;
					Rect test = new Rect (uleft, size);
					rectangles [rectangles.Count - 1].Add (test);
				}
			}
		}
		for (int i = 0; i < rectangles.Count; i++)
		{
			for (int j = 0; j < rectangles [i].Count; j++) {
				Rect test = rectangles [i][j];

				Vector2 a = new Vector3 (test.x, test.y);
				Vector2 b = new Vector3 (test.x + test.width, test.y);
				Vector2 c = new Vector3 (test.x, test.y + test.height);
				Vector2 d = new Vector3 (test.x + test.width, test.y + test.height);

				Debug.DrawLine (a, b, Color.blue, 50f);
				Debug.DrawLine (b, d, Color.blue, 50f);
				Debug.DrawLine (d, c, Color.blue, 50f);
				Debug.DrawLine (c, a, Color.blue, 50f);
			}
		}
	}

	void Update()
	{
		if (Input.GetMouseButtonDown (0)) 
		{
			Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			for (int i = 0; i < rectangles.Count; i++)
			{
				for (int j = 0; j < rectangles [i].Count; j++) 
				{
					
					if (rectangles[i][j].Contains (clickPosition, true)) 
					{
						if (i == 1) 
						{
							for (int c = 0; c < wordIndexes [i].Count; c++)
							{
								//int index = wordIndexes [i] [c];//use index list
								int index = textComponent.text.IndexOf(ClickableWords[i]);
								string newString = textComponent.text.Substring(0, index);
								newString += replacedWords [i];
							//	print (newString);
								//newString += textComponent.text.Substring (index + ClickableWords[i].Length, ClickableWords[i].Length + 1);
								newString += textComponent.text.Substring 
									(index + ClickableWords[i].Length, 
										textComponent.text.Length - 
										(newString.Length + (ClickableWords[i].Length - replacedWords[i].Length)));

								textComponent.text = newString;
							}

						}
						print(ClickableWords[i]);
					}
				}
			}
		}
			
	}
}
//
//  CodeEditor.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/20/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;

public sealed class CodeEditor : MonoBehaviour
{
	public void OnEnable()
	{
		StateManager.ChangeState(GameState.Window);
	}

	public void OnDisable()
	{
		StateManager.ChangeState(GameState.Editing);
	}
}

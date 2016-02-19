//
//  TriggerData.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/19/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public sealed class TriggerData
{
	public string triggerCode;
	public List<string[]> functions;
}

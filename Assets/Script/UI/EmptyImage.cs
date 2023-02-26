using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmptyImage : MaskableGraphic
{

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();
	}

}

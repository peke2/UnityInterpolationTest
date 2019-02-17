using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hermite : MonoBehaviour {

	public Vector2 m_start = new Vector2(0,0);
	public Vector2 m_startControl = new Vector2(0, 1);

	public Vector2 m_end = new Vector2(1, 1);
	public Vector2 m_endControl = new Vector2(-1, 0);

	List<GameObject> m_points = new List<GameObject>();
	List<GameObject> m_controls = new List<GameObject>();

	void Start()
	{
		createLineMaterial();
		gatherPoints();
	}

	void Update()
	{
	}


	public Vector2 calc(Vector2 st, Vector2 ed, Vector2 cs, Vector2 ce, float t)
	{
		Vector2 p;
		float t2 = t * t;

		p = (2*st - 2*ed + cs + ce) * t2*t + (-3*st+3*ed-2*cs-ce)*t2 + cs * t + st;

		return p;
	}


	static Material lineMaterial;
	static void createLineMaterial()
	{
		if (lineMaterial)
		{
			return;
		}

		Shader shader = Shader.Find("Hidden/Internal-Colored");
		lineMaterial = new Material(shader);
		lineMaterial.hideFlags = HideFlags.HideAndDontSave;
		lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
		lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
		lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
		lineMaterial.SetInt("_ZWrite", 0);
	}

	private void OnRenderObject()
	{
		float t = 0;
		List<Vector3> list = new List<Vector3>();

		int ic = 0;
		for(var i=0; i<m_points.Count-1; i++)
		{
			list.Clear();

			Vector2 st = m_points[i].transform.position;
			Vector2 ed = m_points[i+1].transform.position;
			Vector2 cs = m_controls[ic].transform.position - m_points[i].transform.position;
			Vector2 ce = m_controls[ic+1].transform.position - m_points[i+1].transform.position;
			ic += 2;

			for (var j = 0; j <= 10; j++)
			{
				t = j / 10.0f;
				var p = calc(st, ed, cs, ce, t);

				list.Add(new Vector3(p.x, p.y, 0));
			}

			drawLine(list, new Color(1, 0, 0, 1));

			list.Clear();
			list.Add(new Vector3(st.x, st.y, 0));
			list.Add(new Vector3(st.x + cs.x, st.y + cs.y, 0));
			drawLine(list, new Color(0, 1, 0, 1));

			list.Clear();
			list.Add(new Vector3(ed.x, ed.y, 0));
			list.Add(new Vector3(ed.x + ce.x, ed.y + ce.y, 0));
			drawLine(list, new Color(0, 0, 1, 1));
		}
	}

	private void drawLine(List<Vector3> list, Color color)
	{
		var num = list.Count;

		lineMaterial.SetPass(0);
		GL.PushMatrix();
		//GL.MultMatrix(transform.localToWorldMatrix);
		GL.MultMatrix(Matrix4x4.identity);
		GL.Begin(GL.LINES);
		GL.Color(color);

		for(var i=0; i<num-1; i++)
		{
			var p0 = list[i];
			var p1 = list[i + 1];
			GL.Vertex3(p0.x, p0.y, p0.z);
			GL.Vertex3(p1.x, p1.y, p1.z);
		}
		GL.End();
		GL.PopMatrix();
	}

	void gatherPoints()
	{
		var points = GameObject.FindGameObjectsWithTag("Point");
		var controls = GameObject.FindGameObjectsWithTag("Control");
		foreach(var p in points)
		{
			m_points.Add(p);
		}
		foreach(var c in controls)
		{
			m_controls.Add(c);
		}

		m_points.Sort((a, b) => {
			var m = int.Parse(a.name);
			var n = int.Parse(b.name);
			if (m < n) return -1;
			if (m > n) return 1;
			return 0;
		});

		m_controls.Sort((a, b) => {
			var m = int.Parse(a.name);
			var n = int.Parse(b.name);
			if (m < n) return -1;
			if (m > n) return 1;
			return 0;
		});
	}


}

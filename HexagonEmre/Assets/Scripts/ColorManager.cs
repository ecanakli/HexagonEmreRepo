using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
	//Color Lists
	public List<Color> m_hexagonColor = new List<Color>();

	public int m_colorCount;
	private List<List<Color>> m_startColors = new List<List<Color>>();

	public static ColorManager _instance;

	//Singleton
	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
		}
	}

	//Creating Hexagon color at start
	public void CreateGridColors()
	{
		m_colorCount = SettingsManager._instance._colorCount;

		int t_gridWidth = SettingsManager._instance._gridWidth;
		int t_gridHeight = SettingsManager._instance._gridHeight;

		for (int x = 0; x < t_gridWidth; x++)
		{
			List<Color> m_verticalColors = new List<Color>();

			for (int y = 0; y < t_gridHeight; y++)
			{
				m_verticalColors.Add(new Color());
			}
			m_startColors.Add(m_verticalColors);
		}

		for (int x = 0; x < t_gridWidth; x++)
		{
			for (int y = 0; y < t_gridHeight; y++)
			{
				m_startColors[x][y] = m_hexagonColor[Random.Range(0, m_colorCount)];

				if (x - 1 > 0 && m_startColors[x - 1][y] == m_startColors[x][y] || y - 1 > 0 && m_startColors[x][y - 1] == m_startColors[x][y])
				{
					y--;
				}
			}
		}
	}

	//Get Hex Color After Matching Process
	public Color GetHexagonColor()
	{
		return m_hexagonColor[Random.Range(0, m_colorCount)];
	}

	//Get Base Color
	public Color GetStartColor(int x, int y)
	{
		return m_startColors[x][y];
	}
}
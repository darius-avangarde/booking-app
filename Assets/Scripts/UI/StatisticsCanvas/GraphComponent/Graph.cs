using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Graph : MonoBehaviour, IGraph
{
    [SerializeField]
    private GameObject barPrefab = null;
    [SerializeField]
    private Transform barsContainer = null;

    private List<float> data;
    public List<float> Data
    {
        get => data;
        set
        {
            data = value;
            InstantiateBars(value);
        }
    }

    public string XAxisLabel
    {
        get => xAxisText.text;
        set => xAxisText.text = value;
    }

    public string YAxisLabel
    {
        get => yAxisText.text;
        set => yAxisText.text = value;
    }

    [SerializeField]
    private Text xAxisText = null;
    [SerializeField]
    private Text yAxisText = null;
    
    private void InstantiateBars(List<float> data)
    {
        foreach (Transform child in barsContainer)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (var item in data)
        {
            GameObject bar = Instantiate(barPrefab, barsContainer);
            float barWidth = barsContainer.GetComponent<RectTransform>().rect.width / data.Count;
            float barHeight = barsContainer.GetComponent<RectTransform>().rect.height * item / 100;
            bar.GetComponent<RectTransform>().sizeDelta = new Vector2(barWidth, barHeight);
            Color barColor = new Color(
                  UnityEngine.Random.Range(0f, 1f),
                  UnityEngine.Random.Range(0f, 1f),
                  UnityEngine.Random.Range(0f, 1f)
              );
            bar.GetComponent<Image>().color = barColor;
        }
    }
}

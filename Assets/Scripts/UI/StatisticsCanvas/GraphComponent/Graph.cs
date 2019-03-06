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

    private List<string> xValue;
    public List<string> XValue
    {
        get => xValue;
        set
        {
            xValue = value;
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
            Destroy(child.gameObject);
        }

        float barWidth = (barsContainer.GetComponent<RectTransform>().rect.width - 10) / data.Count;
        float barsContainerHeight = barsContainer.GetComponent<RectTransform>().rect.height;

        for (int i = 0; i < data.Count; i++)
        {
            GameObject bar = Instantiate(barPrefab, barsContainer);
            float barHeight = barsContainerHeight * data[i];
            bar.GetComponent<RectTransform>().sizeDelta = new Vector2(barWidth, barHeight);
            Color barColor = new Color(
                  UnityEngine.Random.Range(0f, 1f),
                  UnityEngine.Random.Range(0f, 1f),
                  UnityEngine.Random.Range(0f, 1f)
              );
            bar.GetComponent<Image>().color = barColor;
            print(xValue.Count + "    " + data.Count);
            SetTextXValue(bar, i);
        }
    }

    private void SetTextXValue(GameObject bar, int itemIndex)
    {
        int maxItemsForShowingAbout = 20;
        if (xValue.Count < maxItemsForShowingAbout)
        {
            bar.GetComponentInChildren<Text>().text = xValue[itemIndex].ToString();
            bar.transform.GetChild(1).GetComponent<Text>().text = (int)(data[itemIndex] * 100) + "%";
        }
        else
        {
            bar.GetComponentInChildren<Text>().text = "";
            bar.transform.GetChild(1).GetComponent<Text>().text = "";
        }
    }
}

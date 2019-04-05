using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Graph : MonoBehaviour, IGraph
{
    [SerializeField]
    private GameObject barPrefab = null;
    [SerializeField]
    private Transform barsContainer = null;
    [SerializeField]
    private Text xAxisText = null;
    [SerializeField]
    private Text yAxisText = null;
    [SerializeField]
    private Text maxYText = null;

    private Color defaultBarColor = new Color(0.078f, 0.21f, 0.19f);
    private Color alternativeBarColor = new Color(0.11f, 0.47f, 0.4f);

    public bool HasAlternativeColors { get; set; }

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

    public List<string> XValue { get; set; }

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

    public string MaxYLabel
    {
        get => maxYText.text;
        set => maxYText.text = value;
    }

    private void InstantiateBars(List<float> data)
    {
        foreach (Transform child in barsContainer)
        {
            Destroy(child.gameObject);
        }

        Rect containerRect = barsContainer.GetComponent<RectTransform>().rect;
        float barWidth = containerRect.width / data.Count;

        for (int i = 0; i < data.Count; i++)
        {
            GameObject bar = Instantiate(barPrefab, barsContainer);
            float barHeight = containerRect.height * data[i];
            bar.GetComponent<RectTransform>().sizeDelta = new Vector2(barWidth, barHeight);
            bar.GetComponent<Image>().color = HasAlternativeColors ? (i % 2 == 0 ? alternativeBarColor : defaultBarColor)  : defaultBarColor;
            SetTextXValueOnInterval(bar, i);
        }
    }

    private void SetTextXValueOnInterval(GameObject bar, int itemIndex)
    {
        bar.GetComponentInChildren<Text>().text = "";

        bool isMaxDataCount = XValue.Count > 365;
        int textIntervalMax = 100;
        if (isMaxDataCount && itemIndex % textIntervalMax == 0)
        {
            bar.GetComponentInChildren<Text>().text = XValue[itemIndex].ToString();
        }

        bool isMedDataCount = XValue.Count < 365 && XValue.Count > 100;
        int textIntervalMed = 50;
        if (isMedDataCount && itemIndex % textIntervalMed == 0)
        {
            bar.GetComponentInChildren<Text>().text = XValue[itemIndex].ToString();
        }

        bool isMinDataCount = XValue.Count > 20 && XValue.Count < 100;
        int textIntervalMin = 5;
        if (isMinDataCount && itemIndex % textIntervalMin == 0)
        {
            bar.GetComponentInChildren<Text>().text = XValue[itemIndex].ToString();
        }

        if (XValue.Count < 20)
        {
            bar.GetComponentInChildren<Text>().text = XValue[itemIndex].ToString();
        }
    }
}

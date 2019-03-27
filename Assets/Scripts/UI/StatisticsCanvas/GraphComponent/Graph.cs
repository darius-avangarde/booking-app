﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Graph : MonoBehaviour, IGraph
{
    [SerializeField]
    private GameObject barPrefab = null;
    [SerializeField]
    private Transform barsContainer = null;

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
            bar.GetComponent<Image>().color = HasAlternativeColors ? (i % 2 == 0 ? alternativeBarColor : defaultBarColor)  : defaultBarColor;
            SetTextXValueOnInterval(bar, i);
        }
    }

    private void SetTextXValueOnInterval(GameObject bar, int itemIndex)
    {
        bar.GetComponentInChildren<Text>().text = "";
        bar.transform.GetChild(1).GetComponent<Text>().text = "";

        bool isMaxDataCount = xValue.Count > 365;
        int textIntervalMax = 100;
        if (isMaxDataCount && itemIndex % textIntervalMax == 0)
        {
            bar.GetComponentInChildren<Text>().text = xValue[itemIndex].ToString();
        }

        bool isMedDataCount = xValue.Count < 365 && xValue.Count > 100;
        int textIntervalMed = 50;
        if (isMedDataCount && itemIndex % textIntervalMed == 0)
        {
            bar.GetComponentInChildren<Text>().text = xValue[itemIndex].ToString();
        }

        bool isMinDataCount = xValue.Count > 20 && xValue.Count < 100;
        int textIntervalMin = 5;
        if (isMinDataCount && itemIndex % textIntervalMin == 0)
        {
            bar.GetComponentInChildren<Text>().text = xValue[itemIndex].ToString();
        }

        if (xValue.Count < 20)
        {
            bar.GetComponentInChildren<Text>().text = xValue[itemIndex].ToString();
            bar.transform.GetChild(1).GetComponent<Text>().text = (int)(data[itemIndex] * 100) + "%";
        }
    }
}

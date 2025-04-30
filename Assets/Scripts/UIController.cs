
using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    [Header("Date Slider UI")]
    [SerializeField] private Slider dateSlider;
    [SerializeField] private TextMeshProUGUI dateText;
    
    [Header("Window Size Slider UI")]
    [SerializeField] private Slider windowSizeSlider;
    [SerializeField] private TextMeshProUGUI windowSizeText;
    
    [Header("Threshold Slider UI")]
    [SerializeField] private Slider thresholdSlider;
    [SerializeField] private TextMeshProUGUI thresholdText;

    private GraphDataManager _graphDataManager;
    private GraphVisualizer _graphVisualizer;
    
    // Current selected values
    private string _currentDate;
    private float _currentThreshold;
    private int _currentWindowSize;
    
    // flags
    private bool _isInitialized = false;

    private void Awake()
    {
        // retrieve
        _graphVisualizer = GetComponent<GraphVisualizer>();
        _graphDataManager = GetComponent<GraphDataManager>();
        _graphDataManager.OnSetupComplete += InitializeUI;
    }

    private void InitializeUI()
    {
        List<string> dates = _graphDataManager.Dates;
        
        // initialize window size slider
        windowSizeSlider.minValue = 1;
        windowSizeSlider.maxValue = 100;
        windowSizeSlider.wholeNumbers = true;
        windowSizeSlider.value = 30;
        windowSizeSlider.onValueChanged.AddListener(OnWindowSizeSliderChanged);
        
        // initialize date slider
        dateSlider.minValue = 0;
        dateSlider.maxValue = dates.Count - 1;
        dateSlider.wholeNumbers = true;
        dateSlider.value = windowSizeSlider.value; // default to first portion
        dateSlider.onValueChanged.AddListener(OnDateSliderChanged);
        
        // initialize threshold slider
        thresholdSlider.minValue = 0.0f;
        thresholdSlider.maxValue = 1.0f;
        thresholdSlider.value = 0.5f;
        thresholdSlider.onValueChanged.AddListener(OnThresholdSliderChanged);
        
        // initial update
        UpdateWindowSizeDisplay((int)windowSizeSlider.value);
        UpdateDateDisplay((int)dateSlider.value);
        UpdateThresholdDisplay(thresholdSlider.value);

        _isInitialized = true;
        
        // first request
        _graphDataManager.RequestCorrMatrix(_currentDate, _currentWindowSize);
    }

    private void OnThresholdSliderChanged(float value)
    {
        UpdateThresholdDisplay(value);
    }

    private void OnDateSliderChanged(float v)
    {
        int value = (int)v;
        UpdateDateDisplay(value);
    }

    private void OnWindowSizeSliderChanged(float v)
    {
        int value = (int)v;
        UpdateWindowSizeDisplay(value);
    }

    private void UpdateThresholdDisplay(float value)
    {
        // update current threshold
        _currentThreshold = value;
        
        // update text
        thresholdText.text = $"Threshold: {_currentThreshold:F2}";
        
        // update visualizer
        _graphVisualizer.SetThreshold(value);
    }

    private void UpdateDateDisplay(int idx)
    {
        // update current date
        _currentDate = _graphDataManager.Dates[idx];
        
        // update text
        dateText.text = $"date: {_currentDate}";
        
        // request new data
        if (_isInitialized)
        {
            _graphDataManager.RequestCorrMatrix(_currentDate, _currentWindowSize);
        }
    }

    private void UpdateWindowSizeDisplay(int value)
    {
        // update current window size
        _currentWindowSize = value;
        
        // update text
        windowSizeText.text = $"Window Size: {_currentWindowSize}";
        
        // request new data
        if (_isInitialized)
        {
            _graphDataManager.RequestCorrMatrix(_currentDate, _currentWindowSize);
        }
    }
}

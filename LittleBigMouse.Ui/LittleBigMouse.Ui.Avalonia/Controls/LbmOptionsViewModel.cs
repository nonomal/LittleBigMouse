﻿using System.Collections.Generic;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using System.Windows.Input;
using HLab.Base.Avalonia.Extensions;
using HLab.Mvvm.ReactiveUI;
using LittleBigMouse.DisplayLayout.Monitors;
using LittleBigMouse.Ui.Avalonia.Main;
using ReactiveUI;

namespace LittleBigMouse.Ui.Avalonia.Controls;

public class LbmOptionsViewModel : ViewModel<ILayoutOptions>
{
    public IProcessesCollector ProcessesCollector { get; }

    public LbmOptionsViewModel(IProcessesCollector collector)
    {
        ProcessesCollector = collector;

        AddExcludedProcessCommand = ReactiveCommand.Create(AddExcludedProcess);
        RemoveExcludedProcessCommand = ReactiveCommand.Create(RemoveExcludedProcess);

        _adjustPointerAllowed = this
            .WhenAnyValue(e => e.Model.IsUnaryRatio, (bool r) => r)
            .Log(this, "_adjustPointerAllowed")
            .ToProperty(this, e => e.AdjustPointerAllowed)
            .DisposeWith(this);

        _adjustSpeedAllowed = this
            .WhenAnyValue(e => e.Model.IsUnaryRatio, (bool r) => r)
            .Log(this, "_adjustSpeedAllowed")
            .ToProperty(this, e => e.AdjustSpeedAllowed)
            .DisposeWith(this);

        _selectedAlgorithm = this.WhenAnyValue(e => e.Model.Algorithm)
            .Select(a => AlgorithmList.Find(e => e.Id == a)).ToProperty(this,nameof(SelectedAlgorithm));

        _selectedPriority = this.WhenAnyValue(e => e.Model.Priority)
            .Select(a => PriorityList.Find(e => e.Id == a)).ToProperty(this,nameof(SelectedPriority));
    }

    public ICommand RemoveExcludedProcessCommand { get; }

    void RemoveExcludedProcess()
    {
        if(string.IsNullOrEmpty(SelectedExcludedProcess)) return;
        if(Model == null) return;
        if(Model.ExcludedList.Contains(SelectedExcludedProcess)) Model.ExcludedList.Remove(SelectedExcludedProcess);
    }

    public ICommand AddExcludedProcessCommand { get; }
    void AddExcludedProcess()
    {
        if(string.IsNullOrEmpty(SelectedSeenProcess)) return;
        Model.ExcludedList.Add(SelectedSeenProcess);
    }

    /// <summary>
    /// Allow speed adjustment when all displays have a pixel to dip ratio of 1
    /// </summary>
    [DataMember]
    public bool AdjustSpeedAllowed => _adjustSpeedAllowed.Value;
    readonly ObservableAsPropertyHelper<bool> _adjustSpeedAllowed;

    /// <summary>
    /// Allow pointer adjustment when all displays have a pixel to dip ratio of 1
    /// </summary>
    [DataMember]
    public bool AdjustPointerAllowed => _adjustPointerAllowed.Value;
    readonly ObservableAsPropertyHelper<bool> _adjustPointerAllowed;

    public List<ListItem> AlgorithmList { get; } =
    [
        new("Strait", "Strait", "Simple and highly CPU-efficient transition."),
        new("Cross", "Corner crossing", "In direction-friendly manner, allows traversal through corners.")
    ];

    public List<ListItem> PriorityList { get; } =
    [
        new("Idle", "Idle", ""),
        new("Below", "Below", ""),
        new("Normal", "Normal", ""),
        new("Above", "Above", ""),
        new("High", "High", ""),
        new("Realtime", "Realtime", "")
    ];

    public ListItem? SelectedAlgorithm
    {
        get => _selectedAlgorithm.Value;
        set
        {
            if(Model == null) return;
            Model.Algorithm = value?.Id ?? "";
        }
    }
    readonly ObservableAsPropertyHelper<ListItem?> _selectedAlgorithm;

    public ListItem? SelectedPriority
    {
        get => _selectedPriority.Value;
        set
        {
            if(Model == null) return;
            Model.Priority = value?.Id ?? "";
        }
    }
    readonly ObservableAsPropertyHelper<ListItem?> _selectedPriority;

    public string SelectedExcludedProcess 
    { 
        get => _selectedExcludedProcess; 
        set => this.RaiseAndSetIfChanged(ref _selectedExcludedProcess, value);
    }
    string _selectedExcludedProcess = "";

    public string SelectedSeenProcess 
    { 
        get => _selectedSeenProcess; 
        set => this.RaiseAndSetIfChanged(ref _selectedSeenProcess, value);
    }
    string _selectedSeenProcess = "";



}
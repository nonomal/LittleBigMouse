﻿/*
  LittleBigMouse.Control.Core
  Copyright (c) 2021 Mathieu GRENET.  All right reserved.

  This file is part of LittleBigMouse.Control.Core.

    LittleBigMouse.Control.Core is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    LittleBigMouse.Control.Core is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MouseControl.  If not, see <http://www.gnu.org/licenses/>.

	  mailto:mathieu@mgth.fr
	  http://www.mgth.fr
*/

using Avalonia.Styling;
using HLab.Mvvm.ReactiveUI;
using HLab.Sys.Windows.Monitors;
using LittleBigMouse.DisplayLayout.Monitors;
using ReactiveUI;
using System.Reactive.Linq;
using System.Windows.Input;

namespace LittleBigMouse.Ui.Avalonia.Plugins.Default;

public class DefaultMonitorViewModel : ViewModel<PhysicalMonitor>
{
    public DefaultMonitorViewModel()
    {
        _inches = this.WhenAnyValue(
                e => e.Model.Diagonal,
                selector: d => (d / 25.4).ToString("##.#") + "\"")
            .ToProperty(this, _ => _.Inches);

        _attached = this.WhenAnyValue(e => e.Model.ActiveSource.Source.AttachedToDesktop)
            .ToProperty(this, e => e.Attached);

        DetachCommand = ReactiveCommand.Create(() =>
        {
            DetachFromDesktop();
        },this.WhenAnyValue(
            e => e.Attached, 
            e => e.Model.ActiveSource.Source.Primary,
            (attached,primary) => attached && !primary)
        .ObserveOn(RxApp.MainThreadScheduler));

        AttachCommand = ReactiveCommand.Create(() =>
        {
            AttachToDesktop();
        },this.WhenAnyValue(e => e.Attached, e => !e).ObserveOn(RxApp.MainThreadScheduler));
    }

    public bool Attached => _attached.Value;
    readonly ObservableAsPropertyHelper<bool> _attached;


    public string Inches => _inches.Value;
    readonly ObservableAsPropertyHelper<string> _inches;

    public ICommand AttachCommand { get; }
    public ICommand DetachCommand { get; }

    //static string GetDeviceString(string? deviceString)
    //{
    //    return deviceString != null ? string.Join(' ', deviceString.Split(' ').Where(l => !Brands.Contains(l.ToLower()))) : "";
    //}

    void DetachFromDesktop()
    {
        MonitorDeviceHelper.DetachFromDesktop(Model.ActiveSource.Source.DisplayName);
    }
    void AttachToDesktop()
    {
        MonitorDeviceHelper.AttachToDesktop(
            Model.ActiveSource.Source.DisplayName,
            Model.ActiveSource.Source.Primary,
            Model.ActiveSource.Source.InPixel.Bounds,
            Model.Orientation
            );
        //MonitorDeviceHelper.AttachToDesktop(Model.ActiveSource.Source.DeviceName);
    }

 }
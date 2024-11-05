// <copyright file="ViewLocator.cs" company="Osaze Ogieriakhi">
// Copyright (c) Osaze Ogieriakhi. All rights reserved.
// </copyright>

using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Spreadsheet_Osaze_Ogieriakhi.ViewModels;

namespace Spreadsheet_Osaze_Ogieriakhi;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? data)
    {
        if (data is null)
            return null;

        var name = data.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        var type = Type.GetType(name);

        if (type != null)
        {
            var control = (Control)Activator.CreateInstance(type)!;
            control.DataContext = data;
            return control;
        }

        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
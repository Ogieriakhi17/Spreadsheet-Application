// <copyright file="Program.cs" company="Osaze Ogieriakhi">
// Copyright (c) Osaze Ogieriakhi. All rights reserved.
// </copyright>

using Avalonia;
using Avalonia.ReactiveUI;
using System;

namespace Spreadsheet_Osaze_Ogieriakhi;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}
﻿using Avalonia.Controls;

namespace DotsGame.GUI
{
    public class ServiceLocator
    {
        internal static Window MainWindow { get; set; }

        internal static DotsFieldViewModel DotsFieldViewModel { get; set; }

        internal static GameTreeViewModel GameTreeViewModel { get; set; }

        internal static SgfCoreControViewModel BasicCoreControViewModel { get; set; } = new SgfCoreControViewModel();

        internal static Settings Settings { get; set; } = Settings.Load();
    }
}

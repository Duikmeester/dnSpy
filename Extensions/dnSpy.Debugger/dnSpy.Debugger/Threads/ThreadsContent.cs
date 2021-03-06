﻿/*
    Copyright (C) 2014-2016 de4dot@gmail.com

    This file is part of dnSpy

    dnSpy is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    dnSpy is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with dnSpy.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using dnSpy.Contracts.Controls;
using dnSpy.Contracts.Files.Tabs;
using dnSpy.Contracts.Metadata;
using dnSpy.Contracts.Themes;
using dnSpy.Contracts.Utilities;
using dnSpy.Debugger.CallStack;

namespace dnSpy.Debugger.Threads {
	interface IThreadsContent : IUIObjectProvider {
		void OnShow();
		void OnClose();
		void OnVisible();
		void OnHidden();
		void Focus();
		ListView ListView { get; }
		IThreadsVM ThreadsVM { get; }
	}

	[Export(typeof(IThreadsContent))]
	sealed class ThreadsContent : IThreadsContent {
		public object UIObject => threadsControl;
		public IInputElement FocusedElement => threadsControl.ListView;
		public FrameworkElement ScaleElement => threadsControl;
		public ListView ListView => threadsControl.ListView;
		public IThreadsVM ThreadsVM => vmThreads;

		readonly ThreadsControl threadsControl;
		readonly IThreadsVM vmThreads;
		readonly Lazy<IStackFrameManager> stackFrameManager;
		readonly IFileTabManager fileTabManager;
		readonly Lazy<IModuleLoader> moduleLoader;
		readonly IModuleIdProvider moduleIdProvider;

		[ImportingConstructor]
		ThreadsContent(IWpfCommandManager wpfCommandManager, IThreadsVM threadsVM, IThemeManager themeManager, Lazy<IStackFrameManager> stackFrameManager, IFileTabManager fileTabManager, Lazy<IModuleLoader> moduleLoader, IModuleIdProvider moduleIdProvider) {
			this.stackFrameManager = stackFrameManager;
			this.fileTabManager = fileTabManager;
			this.moduleLoader = moduleLoader;
			this.threadsControl = new ThreadsControl();
			this.vmThreads = threadsVM;
			this.moduleIdProvider = moduleIdProvider;
			this.threadsControl.DataContext = this.vmThreads;
			this.threadsControl.ThreadsListViewDoubleClick += ThreadsControl_ThreadsListViewDoubleClick;
			themeManager.ThemeChanged += ThemeManager_ThemeChanged;

			wpfCommandManager.Add(ControlConstants.GUID_DEBUGGER_THREADS_CONTROL, threadsControl);
			wpfCommandManager.Add(ControlConstants.GUID_DEBUGGER_THREADS_LISTVIEW, threadsControl.ListView);
		}

		void ThreadsControl_ThreadsListViewDoubleClick(object sender, EventArgs e) {
			bool newTab = Keyboard.Modifiers == ModifierKeys.Shift || Keyboard.Modifiers == ModifierKeys.Control;
			SwitchToThreadThreadsCtxMenuCommand.GoTo(moduleIdProvider, fileTabManager, moduleLoader.Value, stackFrameManager.Value, threadsControl.ListView.SelectedItem as ThreadVM, newTab);
		}

		void ThemeManager_ThemeChanged(object sender, ThemeChangedEventArgs e) => vmThreads.RefreshThemeFields();
		public void Focus() => UIUtilities.FocusSelector(threadsControl.ListView);
		public void OnClose() => vmThreads.IsEnabled = false;
		public void OnShow() => vmThreads.IsEnabled = true;
		public void OnHidden() => vmThreads.IsVisible = false;
		public void OnVisible() => vmThreads.IsVisible = true;
	}
}

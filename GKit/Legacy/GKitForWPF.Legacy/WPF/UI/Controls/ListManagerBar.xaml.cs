﻿using GKitForWPF.UI.Converters;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace GKitForWPF.UI.Controls {
	public partial class ListManagerBar : UserControl {
		public static readonly DependencyProperty CreateItemButtonVisibleProperty = DependencyProperty.RegisterAttached(nameof(CreateItemButtonVisible), typeof(bool), typeof(ListManagerBar), new PropertyMetadata(true));
		public static readonly DependencyProperty CreateFolderButtonVisibleProperty = DependencyProperty.RegisterAttached(nameof(CreateFolderButtonVisible), typeof(bool), typeof(ListManagerBar), new PropertyMetadata(true));
		public static readonly DependencyProperty CopyItemButtonVisibleProperty = DependencyProperty.RegisterAttached(nameof(CopyItemButtonVisible), typeof(bool), typeof(ListManagerBar), new PropertyMetadata(true));
		public static readonly DependencyProperty RemoveItemButtonVisibleProperty = DependencyProperty.RegisterAttached(nameof(RemoveItemButtonVisible), typeof(bool), typeof(ListManagerBar), new PropertyMetadata(true));

		public bool CreateItemButtonVisible {
			get {
				return (bool)GetValue(CreateItemButtonVisibleProperty);
			}
			set {
				SetValue(CreateItemButtonVisibleProperty, value);
			}
		}
		public bool CreateFolderButtonVisible {
			get {
				return (bool)GetValue(CreateFolderButtonVisibleProperty);
			}
			set {
				SetValue(CreateFolderButtonVisibleProperty, value);
			}
		}
		public bool CopyItemButtonVisible {
			get {
				return (bool)GetValue(CopyItemButtonVisibleProperty);
			}
			set {
				SetValue(CopyItemButtonVisibleProperty, value);
			}
		}
		public bool RemoveItemButtonVisible {
			get {
				return (bool)GetValue(RemoveItemButtonVisibleProperty);
			}
			set {
				SetValue(RemoveItemButtonVisibleProperty, value);
			}
		}

		public ActionEvent CreateItemButtonClick = new ActionEvent();
		public ActionEvent CreateFolderButtonClick = new ActionEvent();
		public ActionEvent CopyItemButtonClick = new ActionEvent();
		public ActionEvent RemoveItemButtonClick = new ActionEvent();

		public ListManagerBar() {
			InitializeComponent();

			if (this.IsDesignMode())
				return;

			InitBindings();
			RegisterEvents();
		}
		private void InitBindings() {
			BoolToVisibilityConverter boolToVisibilityConverter = new BoolToVisibilityConverter();

			CreateItemButton.SetBinding(Button.VisibilityProperty, new Binding(nameof(CreateItemButtonVisible)) { Source = this, Mode = BindingMode.OneWay, Converter = boolToVisibilityConverter });
			CreateFolderButton.SetBinding(Button.VisibilityProperty, new Binding(nameof(CreateFolderButtonVisible)) { Source = this, Mode = BindingMode.OneWay, Converter = boolToVisibilityConverter });
			CopyItemButton.SetBinding(Button.VisibilityProperty, new Binding(nameof(CopyItemButtonVisible)) { Source = this, Mode = BindingMode.OneWay, Converter = boolToVisibilityConverter });
			RemoveItemButton.SetBinding(Button.VisibilityProperty, new Binding(nameof(RemoveItemButtonVisible)) { Source = this, Mode = BindingMode.OneWay, Converter = boolToVisibilityConverter });
		}
		private void RegisterEvents() {
			CreateItemButton.RegisterClickEvent(CreateItemButtonClick);
			CreateFolderButton.RegisterClickEvent(CreateFolderButtonClick);
			CopyItemButton.RegisterClickEvent(CopyItemButtonClick);
			RemoveItemButton.RegisterClickEvent(RemoveItemButtonClick);
		}
	}
}
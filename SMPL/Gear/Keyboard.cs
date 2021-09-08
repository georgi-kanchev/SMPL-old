﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SMPL.Gear
{
	public static class Keyboard
	{
		public enum Key
		{
			/// <summary>
			/// Unhandled key
			/// </summary>
			Unknown = -1,
			A = 0, B = 1, C = 2, D = 3, E = 4, F = 5, G = 6, H = 7, I = 8, J = 9, K = 10, L = 11, M = 12, N = 13, O = 14,
			P = 15, Q = 16, R = 17, S = 18, T = 19, U = 20, V = 21, W = 22, X = 23, Y = 24, Z = 25,
			_0 = 26, _1 = 27, _2 = 28, _3 = 29, _4 = 30, _5 = 31, _6 = 32, _7 = 33, _8 = 34, _9 = 35,
			Escape = 36, LeftControl = 37, LeftShift = 38, LeftAlt = 39,
			/// <summary>
			/// The left OS specific key.<br></br><br></br>
			/// Windows, Linux: <typeparamref name="window"/><br></br>
			/// MacOS X: <typeparamref name="apple"/><br></br>
			/// etc.
			/// </summary> 
			LeftSystem = 40,
			RightControl = 41, RightShift = 42, RightAlt = 43,
			/// <summary>
			/// The right OS specific key.<br></br><br></br>
			/// Windows, Linux: <typeparamref name="window"/><br></br>
			/// MacOS X: <typeparamref name="apple"/><br></br>
			/// etc.
			/// </summary> 
			RightSystem = 44, Menu = 45, LeftBracket = 46, RightBracket = 47, Semicolon = 48, Comma = 49, Period = 50,
			Dot = 50, Quote = 51, Slash = 52, Backslash = 53, Tilde = 54, Equal = 55, Dash = 56, Space = 57, Enter = 58,
			Return = 58, Backspace = 59, Tab = 60, PageUp = 61, PageDown = 62, End = 63, Home = 64, Insert = 65, Delete = 66,
			/// <summary>
			/// The Num[+] key
			/// </summary>
			Add = 67,
			/// <summary>
			/// The Num[-] key
			/// </summary>
			Minus = 68,
			/// <summary>
			/// The Num[*] key
			/// </summary>
			Multiply = 69,
			/// <summary>
			/// The Num[/] key
			/// </summary>
			Divide = 70,
			LeftArrow = 71, RightArrow = 72, UpArrow = 73, DownArrow = 74, Numpad0 = 75, Numpad1 = 76, Numpad2 = 77,
			Numpad3 = 78, Numpad4 = 79, Numpad5 = 80, Numpad6 = 81, Numpad7 = 82, Numpad8 = 83, Numpad9 = 84, F1 = 85,
			F2 = 86, F3 = 87, F4 = 88, F5 = 89, F6 = 90, F7 = 91, F8 = 92, F9 = 93, F10 = 94, F11 = 95, F12 = 96, F13 = 97,
			F14 = 98, F15 = 99, Pause = 100,
		}
		public static Key[] KeysPressed { get { return keysHeld.ToArray(); } }
		internal static List<Key> keysHeld = new();
		public static bool KeyIsPressed(Key key) => keysHeld.Contains(key);

		private static event Events.ParamsOne<Key> OnKeyPress, OnKeyHold, OnKeyRelease;
		private static event Events.ParamsFour<string, bool, bool, bool> OnTextInput;
		private static event Events.ParamsThree<string, string, string> OnLanguageChange;

		public static class CallWhen
		{
			public static void KeyPress(Action<Key> method, uint order = uint.MaxValue) =>
				OnKeyPress = Events.Add(OnKeyPress, method, order);
			public static void KeyHold(Action<Key> method, uint order = uint.MaxValue) =>
				OnKeyHold = Events.Add(OnKeyHold, method, order);
			public static void KeyRelease(Action<Key> method, uint order = uint.MaxValue) =>
				OnKeyRelease = Events.Add(OnKeyRelease, method, order);
			//string textSymbol, bool isBackspace, bool isEnter, bool isTab
			public static void TextInput(Action<string, bool, bool, bool> method, uint order = uint.MaxValue) =>
				OnTextInput = Events.Add(OnTextInput, method, order);
			//string englishName, string nativeName, string languageCode
			public static void LanguageChange(Action<string, string, string> method, uint order = uint.MaxValue) =>
				OnLanguageChange = Events.Add(OnLanguageChange, method, order);
		}

		internal static void Update()
		{
			for (int i = 0; i < keysHeld.Count; i++) OnKeyHold?.Invoke(keysHeld[i]);
		}
		internal static void Initialize()
		{
			Window.window.KeyPressed += new EventHandler<SFML.Window.KeyEventArgs>(OnKeyPress_);
			Window.window.KeyReleased += new EventHandler<SFML.Window.KeyEventArgs>(OnKeyRelease_);
			Window.window.SetKeyRepeatEnabled(false);
			Window.form.KeyPress += new KeyPressEventHandler(OnTextInput_);
			Window.form.InputLanguageChanged += new InputLanguageChangedEventHandler(OnLanguageChange_);
		}

		internal static void CancelInput()
		{
			for (int i = 0; i < keysHeld.Count; i++) OnKeyRelease?.Invoke(keysHeld[i]);
			keysHeld.Clear();
		}
		internal static void OnKeyPress_(object sender, EventArgs e)
		{
			var keyArgs = (SFML.Window.KeyEventArgs)e;
			var key = (Key)keyArgs.Code;
			keysHeld.Add(key);
			OnKeyPress?.Invoke(key);
		}
		internal static void OnKeyRelease_(object sender, EventArgs e)
		{
			var keyArgs = (SFML.Window.KeyEventArgs)e;
			var key = (Key)keyArgs.Code;
			keysHeld.Remove(key);
			OnKeyRelease?.Invoke(key);
		}
		internal static void OnTextInput_(object sender, EventArgs e)
		{
			var keyArgs = (KeyPressEventArgs)e;
			var keyStr = keyArgs.KeyChar.ToString();
			keyStr = keyStr.Replace('\r', '\n');
			if (keyStr == "\b") keyStr = "";
			var isBackSpace = keyStr == "\b";
			var isEnter = keyStr == Environment.NewLine;
			var isTab = keyStr == "\t";
			OnTextInput?.Invoke(keyStr, isBackSpace, isEnter, isTab);
		}
		internal static void OnLanguageChange_(object sender, EventArgs e)
		{
			var langArgs = (InputLanguageChangedEventArgs)e;
			var culture = langArgs.InputLanguage.Culture;
			OnLanguageChange?.Invoke(culture.EnglishName, culture.NativeName, culture.Name);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Core.UnitTests;
using NUnit.Framework;

namespace Microsoft.Maui.Controls.Xaml.UnitTests
{
	public partial class Gh7097 : Gh7097Base
	{
		public Gh7097() => InitializeComponent();
		public Gh7097(bool useCompiledXaml) : base(useCompiledXaml)
		{
			//this stub will be replaced at compile time
		}

		[TestFixture]
		class Tests
		{
			IReadOnlyList<string> _flags;
			[SetUp]
			public void Setup()
			{
				Device.PlatformServices = new MockPlatformServices();
				_flags = Device.Flags;
				Device.SetFlags(new List<string>(Device.Flags ?? new List<string>()) { "CollectionView_Experimental" }.AsReadOnly());
			}

			[TearDown]
			public void TearDown()
			{
				Device.PlatformServices = null;
				Device.SetFlags(_flags);
			}

			[Test]
			public void CanXReferenceRoot([Values(false, true)] bool useCompiledXaml)
			{
				var layout = new Gh7097(useCompiledXaml)
				{
					BindingContext = new
					{
						Button1Command = new MockCommand(),
						Button2Command = new MockCommand(),
					}
				};
				var cv = layout.Content as CollectionView;
				var content = cv.ItemTemplate.CreateContent() as StackLayout;
				var btn1 = content.Children[0] as Button;
				Assert.That(btn1.Command, Is.TypeOf<MockCommand>());
			}

			[Test]
			//this was later reported as https://github.com/xamarin/Microsoft.Maui.Controls/issues/7286
			public void RegisteringXNameOnSubPages([Values(false, true)] bool useCompiledXaml)
			{
				var layout = new Gh7097(useCompiledXaml);
				var s = layout.FindByName("self");
				Assert.That(layout.self, Is.Not.Null);
				Assert.That(layout.collectionview, Is.Not.Null);
			}

			class MockCommand : ICommand
			{
#pragma warning disable 0067
				public event EventHandler CanExecuteChanged;
#pragma warning restore 0067
				public bool CanExecute(object parameter) => true;
				public void Execute(object parameter) => throw new NotImplementedException();
			}
		}
	}
}

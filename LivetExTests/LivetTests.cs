using LivetEx.Messaging;
using LivetEx.Triggers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace LivetEx.Tests {
	[TestClass()]
	public class LivetTests {

		List<string> AssertList = new List<string>();
		Messenger messenger = new Messenger();

		public LivetTests() {
			var triggers = Interaction.GetTriggers( new WindowEx( AssertList ) );
			triggers.Add( new MessageTrigger( messenger ) {
				Actions = {
					new CallMethodAction() { MethodName = "Test" },
					new CallMethodAction() { MethodName = "Test", MethodParameter = "string" },
					new CallMethodAction() { MethodName = "Test", MethodParameter = 999 },
					new CallMethodAction() { MethodName = "Test", MethodParameter = "obj" },

					new CallMethodAction() { MethodName = "TestResult" },
					new CallMethodAction() { MethodName = "TestResult", MethodParameter = "string" },
					new CallMethodAction() { MethodName = "TestResult", MethodParameter = 999 },
					new CallMethodAction() { MethodName = "TestResult", MethodParameter = "obj" },

					new CallMethodAction() { MethodName = "TestResultInt" },
					new CallMethodAction() { MethodName = "TestResultInt", MethodParameter = "string" },
					new CallMethodAction() { MethodName = "TestResultInt", MethodParameter = 999 },
					new CallMethodAction() { MethodName = "TestResultInt", MethodParameter = "obj" },

					new CallMethodAction() { MethodName = "TestResultObject" },
					new CallMethodAction() { MethodName = "TestResultObject", MethodParameter = "string" },
					new CallMethodAction() { MethodName = "TestResultObject", MethodParameter = 999 },
					new CallMethodAction() { MethodName = "TestResultObject", MethodParameter = "obj" },
				}
			} );
		}


		[TestCleanup()]
		public void TestCleanup() {
			AssertList.Clear();
		}

		[TestMethod()]
		public void MessageTest() {
			messenger.Raise( new Message() );

			var TrueList = new List<string>(){
				 "Test()" ,
				 "Test( string )" ,
				 "Test( int )" ,
				 "Test( string )" ,
				 "TestResult()" ,
				 "TestResult( string ): string" ,
				 "TestResult( int ): 999" ,
				 "TestResult( string ): obj" ,
				 "TestResultInt()" ,
				 "TestResultInt( string ): string" ,
				 "TestResultInt( int ): 999" ,
				 "TestResultInt( string ): obj" ,
				 "TestResultObject()" ,
				 "TestResultObject( string ): string" ,
				 "TestResultObject( int ): 999" ,
				 "TestResultObject( string ): obj" ,
			};

			Assert.IsTrue( AssertList.SequenceEqual( TrueList ) );

		}

		[TestMethod()]
		public void ResponsiveMessageTest() {

			var r = messenger.GetResponse( new ResponsiveMessage() );

			var r2 = messenger.GetResponse( new ResponsiveMessage<object>() );

			var TrueList = new List<string>(){
				 "Test()" ,
				 "Test( string )" ,
				 "Test( int )" ,
				 "Test( string )" ,
				 "TestResult()" ,
				 "TestResult( string ): string" ,
				 "TestResult( int ): 999" ,
				 "TestResult( string ): obj" ,
				 "TestResultInt()" ,
				 "TestResultInt( string ): string" ,
				 "TestResultInt( int ): 999" ,
				 "TestResultInt( string ): obj" ,
				 "TestResultObject()" ,
				 "TestResultObject( string ): string" ,
				 "TestResultObject( int ): 999" ,
				 "TestResultObject( string ): obj" ,

				 "Test()" ,
				 "Test( string )" ,
				 "Test( int )" ,
				 "Test( string )" ,
				 "TestResult()" ,
				 "TestResult( string ): string" ,
				 "TestResult( int ): 999" ,
				 "TestResult( string ): obj" ,
				 "TestResultInt()" ,
				 "TestResultInt( string ): string" ,
				 "TestResultInt( int ): 999" ,
				 "TestResultInt( string ): obj" ,
				 "TestResultObject()" ,
				 "TestResultObject( string ): string" ,
				 "TestResultObject( int ): 999" ,
				 "TestResultObject( string ): obj" ,
			};

			Assert.IsTrue( AssertList.SequenceEqual( TrueList ) );
		}

		[TestMethod()]
		public void MethodCacheTest() {
			var messengerAction = new Messenger();
			var messengerFunc = new Messenger();

			var triggers = Interaction.GetTriggers( new WindowEx( AssertList ) );
			triggers.Add( new MessageTrigger( messengerAction ) {
				Actions = {
					new CallMethodAction() { MethodName = "Test" },
				}
			} );
			triggers.Add( new MessageTrigger( messengerFunc ) {
				Actions = {
					new CallMethodAction() { MethodName = "Test", MethodParameterType = typeof(string) },
				}
			} );

			messengerAction.Raise( new Message() );
			messengerFunc.Raise( new Message() );

			Task.WaitAll( MethodBinder.Tasks.ToArray() );
			Task.WaitAll( MethodBinderWithArgument.Tasks.ToArray() );

			messengerAction.Raise( new Message() );
			messengerFunc.Raise( new Message() );

			messengerAction.Raise( new Message() );
			messengerFunc.Raise( new Message() );

			var TrueList = new List<string>(){
				 "Test()" ,
				 "Test( string )" ,
				 "Test()" ,
				 "Test( string )" ,
				 "Test()" ,
				 "Test( string )" ,
			};

			Assert.IsTrue( AssertList.SequenceEqual( TrueList ) );

		}


		[TestMethod()]
		public void CallMethodMessageTest() {
			messenger.Raise( new CallActionMessage() );

			var TrueList = new List<string>(){
				 "Test()" ,
				 "Test( string )" ,
				 "Test( int )" ,
				 "Test( string )" ,
				 "TestResult()" ,
				 "TestResult( string ): string" ,
				 "TestResult( int ): 999" ,
				 "TestResult( string ): obj" ,
				 "TestResultInt()" ,
				 "TestResultInt( string ): string" ,
				 "TestResultInt( int ): 999" ,
				 "TestResultInt( string ): obj" ,
				 "TestResultObject()" ,
				 "TestResultObject( string ): string" ,
				 "TestResultObject( int ): 999" ,
				 "TestResultObject( string ): obj" ,
			};

			Assert.IsTrue( AssertList.SequenceEqual( TrueList ) );

			// DirectMessage
		}


		[TestMethod()]
		public void CallMethodMessageForMultiMessageActionTest() {

			var window = new WindowEx( AssertList ) { Width = 50, Height = 50 };

			var messenger = new Messenger();

			var triggers = Interaction.GetTriggers( window );
			triggers.Add( new MessageTrigger( messenger ) {
				Actions = {
					new MultiMessageAction(),
				}
			} );

			window.Show();


			Assert.ThrowsException<ArgumentNullException>( () => {
				messenger.Raise( new CallActionMessage() );
			} );

			var result = new DebugList<object>();

			//-----------------
			{
				messenger.Raise( new CallActionMessage( "Test" ) );
				messenger.Raise( new CallActionMessage( "TestResult" ) );
				messenger.Raise( new CallActionMessage( "TestResultInt" ) );
				messenger.Raise( new CallActionMessage( "TestResultObject" ) );
			}
			{
				messenger.Raise( new CallActionMessage<string>( "Test", "str" ) );
				messenger.Raise( new CallActionMessage<int>( "Test", 100 ) );
				messenger.Raise( new CallActionMessage<object>( "Test", "obj" ) );

				messenger.Raise( new CallActionMessage<object>( "Test", null ) );
				messenger.Raise( new CallActionMessage<string>( "Test", null ) );
				messenger.Raise( new CallActionMessage<object>( "Test", 200 ) );
			}
			{
				messenger.Raise( new CallActionMessage<string>( "TestResult", "str" ) );
				messenger.Raise( new CallActionMessage<object>( "TestResult", "obj" ) );
				messenger.Raise( new CallActionMessage<int>( "TestResult", 100 ) );

				messenger.Raise( new CallActionMessage<string>( "TestResult", null ) );
				messenger.Raise( new CallActionMessage<object>( "TestResult", null ) );
				messenger.Raise( new CallActionMessage<object>( "TestResult", 200 ) );
			}
			{
				messenger.Raise( new CallActionMessage<string>( "TestResultInt", "str" ) );
				messenger.Raise( new CallActionMessage<object>( "TestResultInt", "obj" ) );
				messenger.Raise( new CallActionMessage<int>( "TestResultInt", 100 ) );

				messenger.Raise( new CallActionMessage<string>( "TestResultInt", null ) );
				messenger.Raise( new CallActionMessage<object>( "TestResultInt", null ) );
				messenger.Raise( new CallActionMessage<object>( "TestResultInt", 200 ) );
			}
			{
				messenger.Raise( new CallActionMessage<string>( "TestResultObject", "str" ) );
				messenger.Raise( new CallActionMessage<object>( "TestResultObject", "obj" ) );
				messenger.Raise( new CallActionMessage<int>( "TestResultObject", 100 ) );

				messenger.Raise( new CallActionMessage<string>( "TestResultObject", null ) );
				messenger.Raise( new CallActionMessage<object>( "TestResultObject", null ) );
				messenger.Raise( new CallActionMessage<object>( "TestResultObject", 200 ) );
			}
			//---------------

			result.Push = messenger.GetResponse( new CallFuncMessage<object>( "Test" ) );
			result.Push = messenger.GetResponse( new CallFuncMessage<object>( "TestResult" ) );
			result.Push = messenger.GetResponse( new CallFuncMessage<object>( "TestResultInt" ) );
			result.Push = messenger.GetResponse( new CallFuncMessage<object>( "TestResultObject" ) );

			result.Push = messenger.GetResponse( new CallFuncMessage<string, object>( "Test", "str" ) );
			result.Push = messenger.GetResponse( new CallFuncMessage<int, object>( "Test", 100 ) );
			result.Push = messenger.GetResponse( new CallFuncMessage<object, object>( "Test", "obj" ) );

			result.Push = messenger.GetResponse( new CallFuncMessage<object, object>( "Test", null ) );
			result.Push = messenger.GetResponse( new CallFuncMessage<string, object>( "Test", null ) );
			result.Push = messenger.GetResponse( new CallFuncMessage<object, object>( "Test", 200 ) );


			result.Push = messenger.GetResponse( new CallFuncMessage<string, string>( "TestResult", "str" ) );
			result.Push = messenger.GetResponse( new CallFuncMessage<object, string>( "TestResult", "obj" ) );
			result.Push = messenger.GetResponse( new CallFuncMessage<int, string>( "TestResult", 100 ) );

			result.Push = messenger.GetResponse( new CallFuncMessage<string, string>( "TestResult", null ) );
			result.Push = messenger.GetResponse( new CallFuncMessage<object, string>( "TestResult", null ) );
			result.Push = messenger.GetResponse( new CallFuncMessage<object, string>( "TestResult", 200 ) );


			result.Push = messenger.GetResponse( new CallFuncMessage<string, int>( "TestResultInt", "str" ) );
			result.Push = messenger.GetResponse( new CallFuncMessage<object, int>( "TestResultInt", "obj" ) );
			result.Push = messenger.GetResponse( new CallFuncMessage<int, int>( "TestResultInt", 100 ) );

			result.Push = messenger.GetResponse( new CallFuncMessage<string, int>( "TestResultInt", null ) );
			result.Push = messenger.GetResponse( new CallFuncMessage<object, int>( "TestResultInt", null ) );
			result.Push = messenger.GetResponse( new CallFuncMessage<object, int>( "TestResultInt", 200 ) );


			result.Push = messenger.GetResponse( new CallFuncMessage<string, object>( "TestResultObject", "str" ) );
			result.Push = messenger.GetResponse( new CallFuncMessage<object, object>( "TestResultObject", "obj" ) );
			result.Push = messenger.GetResponse( new CallFuncMessage<int, object>( "TestResultObject", 100 ) );

			result.Push = messenger.GetResponse( new CallFuncMessage<string, object>( "TestResultObject", null ) );
			result.Push = messenger.GetResponse( new CallFuncMessage<object, object>( "TestResultObject", null ) );
			result.Push = messenger.GetResponse( new CallFuncMessage<object, object>( "TestResultObject", 200 ) );

			//------------------

			var ResultTrueList = new List<Object>() {
					null,
					"TestResult",
					1,
					"TestResultObject",
					null,
					null,
					null,
					null,
					null,
					null,
					"TestResult: str",
					"TestResult: obj",
					"TestResult: 100",
					"TestResult: ",
					"TestResult: ",
					"TestResult: 200",
					2,
					2,
					100,
					2,
					4,
					200,
					"TestResultObject: str",
					"TestResultObject: obj",
					100,
					"TestResultObject: ",
					"TestResultObject: ",
					200,
			};

			var TrueList = new List<string>(){
				"Test()",
				"TestResult()",
				"TestResultInt()",
				"TestResultObject()",
				"Test( string )",
				"Test( int )",
				"Test( string )",
				"Test( object )",
				"Test( string )",
				"Test( int )",
				"TestResult( string ): str",
				"TestResult( string ): obj",
				"TestResult( int ): 100",
				"TestResult( string ): ",
				"TestResult( object ): ",
				"TestResult( int ): 200",
				"TestResultInt( string ): str",
				"TestResultInt( string ): obj",
				"TestResultInt( int ): 100",
				"TestResultInt( string ): ",
				"TestResultInt( object ): ",
				"TestResultInt( int ): 200",
				"TestResultObject( string ): str",
				"TestResultObject( string ): obj",
				"TestResultObject( int ): 100",
				"TestResultObject( string ): ",
				"TestResultObject( object ): ",
				"TestResultObject( int ): 200",
				"Test()",
				"TestResult()",
				"TestResultInt()",
				"TestResultObject()",
				"Test( string )",
				"Test( int )",
				"Test( string )",
				"Test( object )",
				"Test( string )",
				"Test( int )",
				"TestResult( string ): str",
				"TestResult( string ): obj",
				"TestResult( int ): 100",
				"TestResult( string ): ",
				"TestResult( object ): ",
				"TestResult( int ): 200",
				"TestResultInt( string ): str",
				"TestResultInt( string ): obj",
				"TestResultInt( int ): 100",
				"TestResultInt( string ): ",
				"TestResultInt( object ): ",
				"TestResultInt( int ): 200",
				"TestResultObject( string ): str",
				"TestResultObject( string ): obj",
				"TestResultObject( int ): 100",
				"TestResultObject( string ): ",
				"TestResultObject( object ): ",
				"TestResultObject( int ): 200",
			};

			Assert.IsTrue( AssertList.SequenceEqual( TrueList ) );
			Assert.IsTrue( result.SequenceEqual( ResultTrueList ) );

		}



		[TestMethod()]
		public void DirectMessageTest() {

			var window = new WindowEx( AssertList ) { Width = 50, Height = 50 };

			var messenger = new Messenger();

			var triggers = Interaction.GetTriggers( window );
			triggers.Add( new MessageTrigger( messenger ) {
				Actions = {
					new WindowActionMessageAction(){
						DirectMessage = new DirectMessage(){
							Message = new WindowActionMessage(){ Action = WindowAction.None },
							CallbackMethodTarget = window,
							CallbackMethodName = "CallbackMethodName",
						}
					},
				}
			} );

			window.Show();



			messenger.Raise( new Message() );

		}

	}

	class DebugList<T> : List<T> {

		public T Push {
			set {
				this.Add( value );
			}
		}

	}

	class WindowEx : Window {
		public WindowEx( List<string> assertList ) {
			this.AssertList = assertList;
		}

		readonly List<string> AssertList;

		#region CallMethod

		void Test() {
			AssertList.Add( "Test()" );
		}

		void Test( object value ) {
			AssertList.Add( "Test( object )" );
		}

		void Test( string value ) {
			AssertList.Add( "Test( string )" );
		}

		void Test( int value ) {
			AssertList.Add( "Test( int )" );
		}



		string TestResult() {
			AssertList.Add( "TestResult()" );
			return "TestResult";
		}

		string TestResult( string value ) {
			AssertList.Add( "TestResult( string ): " + value );
			return "TestResult: " + value;
		}
		string TestResult( int value ) {
			AssertList.Add( "TestResult( int ): " + value );
			return "TestResult: " + value;
		}
		string TestResult( object value ) {
			AssertList.Add( "TestResult( object ): " + value );
			return "TestResult: " + value;
		}


		int TestResultInt() {
			AssertList.Add( "TestResultInt()" );
			return 1;
		}

		int TestResultInt( string value ) {
			AssertList.Add( "TestResultInt( string ): " + value );
			return 2;
		}
		int TestResultInt( int value ) {
			AssertList.Add( "TestResultInt( int ): " + value );
			return value;
		}
		int TestResultInt( object value ) {
			AssertList.Add( "TestResultInt( object ): " + value );
			return 4;
		}


		object TestResultObject() {
			AssertList.Add( "TestResultObject()" );
			return "TestResultObject";
		}

		object TestResultObject( string value ) {
			AssertList.Add( "TestResultObject( string ): " + value );
			return "TestResultObject: " + value;
		}
		object TestResultObject( int value ) {
			AssertList.Add( "TestResultObject( int ): " + value );
			return value;
		}
		object TestResultObject( object value ) {
			AssertList.Add( "TestResultObject( object ): " + value );
			return "TestResultObject: " + value;
		}



		#endregion


		public void CallbackMethodName( WindowActionMessage m ) {

		}

	}
}
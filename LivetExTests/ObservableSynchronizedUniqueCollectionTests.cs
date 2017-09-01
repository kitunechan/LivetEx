using System.Security.Cryptography.X509Certificates;
using System.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LivetEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivetEx.Tests {
	[TestClass()]
	public class ObservableSynchronizedUniqueCollectionTests {
		[TestMethod()]
		public void ObservableSynchronizedUniqueCollectionTest() {
			var intList = new ObservableSynchronizedUniqueCollection<int>();
			var stringList = new ObservableSynchronizedUniqueCollection<string>();
		}

		[TestMethod()]
		public void ObservableSynchronizedUniqueCollectionTest1() {
			var source = Enumerable.Range( 0, 100 );

			var intList = new ObservableSynchronizedUniqueCollection<int>( source );
			var stringList = new ObservableSynchronizedUniqueCollection<string>( source.Select( x => x.ToString() ) );
		}

		[TestMethod()]
		public void ObservableSynchronizedUniqueCollectionTest2() {
			var source = Enumerable.Range( 0, 100 ).Select( x => Tuple.Create( x, "str" + x ) );

			var intList = new ObservableSynchronizedUniqueCollection<int>( new EqualityComparer<int, int>( x => x ) );
			var stringList = new ObservableSynchronizedUniqueCollection<string>( new EqualityComparer<string, string>( x => x ) );
		}

		[TestMethod()]
		public void ObservableSynchronizedUniqueCollectionTest3() {
			var source = Enumerable.Range( 0, 100 ).Select( x => Tuple.Create( x, "str" + x ) );

			var intList = new ObservableSynchronizedUniqueCollection<Tuple<int, string>>( source, new EqualityComparer<Tuple<int, string>, int>( x => x.Item1 ) );
			var stringList = new ObservableSynchronizedUniqueCollection<Tuple<int, string>>( source, new EqualityComparer<Tuple<int, string>, string>( x => x.Item2 ) );
		}

		[TestMethod()]
		public void AddTest() {

			var intList = new ObservableSynchronizedUniqueCollection<int>();

			intList.Add( 1 );
			intList.Add( 2 );
			intList.Add( 3 );

			intList.Add( 3 );

			Assert.IsTrue( intList.Count == 3 );

			var stringList = new ObservableSynchronizedUniqueCollection<string>();

			stringList.Add( "1" );
			stringList.Add( "2" );
			stringList.Add( "3" );

			stringList.Add( "3" );

			Assert.IsTrue( stringList.Count == 3 );
		}

		[TestMethod()]
		public void InsertTest() {

			var intList = new ObservableSynchronizedUniqueCollection<int> {
				1,
				2,
				3
			};

			var stringList = new ObservableSynchronizedUniqueCollection<string> {
				"1",
				"2",
				"3",
			};

			intList.Insert( 1, 999 );
			intList.Insert( 0, 1 );
			Assert.IsTrue( intList.Count == 4 && intList[1] == 999 );

			stringList.Insert( 1, "999" );
			stringList.Insert( 1, "1" );

			Assert.IsTrue( stringList.Count == 4 && stringList[1] == "999" );

		}

		[TestMethod()]
		public void RemoveAtTest() {

			var intList = new ObservableSynchronizedUniqueCollection<int> { 1, 2, 3 };
			var stringList = new ObservableSynchronizedUniqueCollection<string> { "1", "2", "3", };

			intList.RemoveAt( 0 );
			Assert.IsTrue( intList.Count == 2 );

			stringList.RemoveAt( 0 );
			Assert.IsTrue( stringList.Count == 2 );

			Assert.ThrowsException<ArgumentOutOfRangeException>( () => intList.RemoveAt( 999 ) );
			Assert.ThrowsException<ArgumentOutOfRangeException>( () => stringList.RemoveAt( 999 ) );
		}

		[TestMethod()]
		public void RemoveTest() {
			var intList = new ObservableSynchronizedUniqueCollection<int> { 1, 2, 3 };
			var stringList = new ObservableSynchronizedUniqueCollection<string> { "1", "2", "3", };

			Assert.IsTrue( intList.Remove( 1 ) && intList.Count == 2 );
			Assert.IsTrue( stringList.Remove( "1" ) && stringList.Count == 2 );

			Assert.IsFalse( intList.Remove( 999 ) );
			Assert.IsTrue( intList.Count == 2 );

			Assert.IsFalse( stringList.Remove( "999" ) );
			Assert.IsTrue( stringList.Count == 2 );
		}

		[TestMethod()]
		public void ClearTest() {
			var intList = new ObservableSynchronizedUniqueCollection<int> { 1, 2, 3 };
			var stringList = new ObservableSynchronizedUniqueCollection<string> { "1", "2", "3", };

			intList.Clear();
			Assert.IsFalse( intList.Any() );

			stringList.Clear();
			Assert.IsFalse( stringList.Any() );
		}

		[TestMethod()]
		public void IndexOfTest() {
			var intList = new ObservableSynchronizedUniqueCollection<int> { 1, 2, 3 };
			var stringList = new ObservableSynchronizedUniqueCollection<string> { "1", "2", "3", };

			Assert.IsTrue( intList.IndexOf( 999 ) == -1 );
			Assert.IsTrue( stringList.IndexOf( "999" ) == -1 );

			Assert.IsTrue( intList.IndexOf( 1 ) == 0 );
			Assert.IsTrue( stringList.IndexOf( "1" ) == 0 );
		}

		[TestMethod()]
		public void ContainsTest() {
			var intList = new ObservableSynchronizedUniqueCollection<int> { 1, 2, 3 };
			var stringList = new ObservableSynchronizedUniqueCollection<string> { "1", "2", "3", };


			Assert.IsFalse( intList.Contains( 999 ) );
			Assert.IsFalse( stringList.Contains( "999" ) );

			Assert.IsTrue( intList.Contains( 1 ) );
			Assert.IsTrue( stringList.Contains( "1" ) );
		}

		[TestMethod()]
		public void MoveTest() {
			var intList = new ObservableSynchronizedUniqueCollection<int> { 1, 2, 3 };
			var stringList = new ObservableSynchronizedUniqueCollection<string> { "1", "2", "3", };

			intList.Move( 2, 0 );
			Assert.IsTrue( intList[0] == 3 );

			stringList.Move( 2, 0 );
			Assert.IsTrue( stringList[0] == "3" );

			Assert.IsTrue( intList.Count == 3 );
			Assert.IsTrue( stringList.Count == 3 );
		}

		[TestMethod()]
		public void CopyToTest() {
			var intList = new ObservableSynchronizedUniqueCollection<int> { 1, 2, 3 };
			var stringList = new ObservableSynchronizedUniqueCollection<string> { "1", "2", "3", };

			var array = new int[3];

			intList.CopyTo( array, 0 );
			Assert.IsTrue( array[0] == 1 );
			Assert.IsTrue( array[1] == 2 );
			Assert.IsTrue( array[2] == 3 );

			var values = new string[3];

			stringList.CopyTo( values, 0 );
			Assert.IsTrue( values[0] == "1" );
			Assert.IsTrue( values[1] == "2" );
			Assert.IsTrue( values[2] == "3" );
		}

		[TestMethod()]
		public void GetEnumeratorTest() {
			var intList = new ObservableSynchronizedUniqueCollection<int> { 1, 2, 3 };
			var stringList = new ObservableSynchronizedUniqueCollection<string> { "1", "2", "3", };

			var intEnum = intList.GetEnumerator();

			Assert.IsTrue( intEnum.MoveNext() );
			Assert.IsTrue( intEnum.Current == 1 );
			Assert.IsTrue( intEnum.MoveNext() );
			Assert.IsTrue( intEnum.Current == 2 );
			Assert.IsTrue( intEnum.MoveNext() );
			Assert.IsTrue( intEnum.Current == 3 );
			Assert.IsFalse( intEnum.MoveNext() );

			var strEnum = stringList.GetEnumerator();

			Assert.IsTrue( strEnum.MoveNext() );
			Assert.IsTrue( strEnum.Current == "1" );
			Assert.IsTrue( strEnum.MoveNext() );
			Assert.IsTrue( strEnum.Current == "2" );
			Assert.IsTrue( strEnum.MoveNext() );
			Assert.IsTrue( strEnum.Current == "3" );
			Assert.IsFalse( strEnum.MoveNext() );

		}

		[TestMethod()]
		public void CopyToTest1() {

			var intList = new ObservableSynchronizedUniqueCollection<int> { 1, 2, 3 };
			var stringList = new ObservableSynchronizedUniqueCollection<string> { "1", "2", "3", };

			Array array = new int[3];

			intList.CopyTo( array, 0 );
			Assert.IsTrue( (int)array.GetValue(0) == 1 );
			Assert.IsTrue( (int)array.GetValue(1) == 2 );
			Assert.IsTrue( (int)array.GetValue( 2 ) == 3 );

			Array values = new string[3];

			stringList.CopyTo( values, 0 );
			Assert.IsTrue( (string)values.GetValue(0) == "1" );
			Assert.IsTrue( (string)values.GetValue(1) == "2" );
			Assert.IsTrue( (string)values.GetValue( 2 ) == "3" );

		}

		[TestMethod()]
		public void DisposeTest() {
			var intList = new ObservableSynchronizedUniqueCollection<int> { 1, 2, 3 };
			var stringList = new ObservableSynchronizedUniqueCollection<string> { "1", "2", "3", };

			intList.Dispose();
			intList.Dispose();
			intList.Dispose();

			stringList.Dispose();
			stringList.Dispose();
			stringList.Dispose();
		}
	}
}
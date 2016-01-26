using System;
using NUnit.Framework;
using LightInject;
namespace AndroidTests
{
	[TestFixture]
	public class TestsSample
	{
		
		[SetUp]
		public void Setup ()
		{
			ServiceContainer container = new ServiceContainer();
			container.GetInstance<string> ();
		}

		
		[TearDown]
		public void Tear ()
		{
		}

		[Test]
		public void Pass ()
		{
			Console.WriteLine ("test1");
			Assert.True (true);
		}

		[Test]
		public void Fail ()
		{
			Assert.False (true);
		}

		[Test]
		[Ignore ("another time")]
		public void Ignore ()
		{
			Assert.True (false);
		}

		[Test]
		public void Inconclusive ()
		{
			Assert.Inconclusive ("Inconclusive");
		}
	}
}


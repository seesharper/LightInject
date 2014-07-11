# LightMock #

**LightMock** is a simple mocking library that can be used on platforms that does not allow dynamic code generation.


## Installing ##

<div class="nuget-badge" >
   <p>
         <code>PM&gt; Install-Package LightMock </code>
   </p>
</div>

This adds a reference to the **LightMock** in the target project.

The **LightMock** library is a a portable class library that makes it possible to use this across all platforms including iOS and Android.


## Creating a mock object ##
   
Even though the mock objects are created manually, they can be reused in many scenarios.

As an an example we will use this simple interface.

    public interface IFoo
    {
        void Execute(string value);
        string Execute();        
    }

The mock object implementing this interface looks like this

   	public class FooMock : IFoo
    {
        private readonly IInvocationContext<IFoo> context;
        
        public FooMock(IInvocationContext<IFoo> context)
        {
            this.context = context;
        }

        public void Execute(string value)
        {
            context.Invoke(f => f.Execute(value));
        }

        public string Execute()
        {
            return context.Invoke(f => f.Execute());
        }        
    } 

> Note: Only mocked methods needs to be implemented. Other methods that does not get invoked during testing can just throw a NotImplementedException.

## Assertions ##

The *Assert* method is used to verify that the given method has been executed the expected number of times using the expected arguments.   


	//Arrange
	var mockContext = new MockContext<IFoo>();
	var fooMock = new FooMock(mockContext);            

	//Act
	fooMock.Execute("SomeValue");            

	//Assert
	mockContext.Assert(f => f.Execute("SomeValue"));

> Note: Not specifying the expected number of invocations, means at least once.

If we don't care about the actual argument value, we can use a special class called *The*.

    var mockContext = new MockContext<IFoo>();
    var fooMock = new FooMock(mockContext);

    fooMock.Execute("SomeValue");

    mockContext.Assert(f => f.Execute(The<string>.IsAnyValue), Invoked.Once);                        	

We call also use this class to perform custom verification.

    var mockContext = new MockContext<IFoo>();
    var fooMock = new FooMock(mockContext);

    fooMock.Execute("SomeValue");
                        
    mockContext.Assert(f => f.Execute(The<string>.Is(s => s.StartsWith("Some"))), Invoked.Once);

## Arrangements ##

We can use arrangements to add behavior to the mock object.

For instance we can set up the mock object to return a value.

	var mockContext = new MockContext<IFoo>();
	var fooMock = new FooMock(mockContext);
	mockContext.Arrange(f => f.Execute()).Returns("SomeValue");
	
	var result = fooMock.Execute();
	
	Assert.AreEqual("SomeValue", result); 


Throw an exception

	var mockContext = new MockContext<IFoo>();
	var fooMock = new FooMock(mockContext);
	mockContext.Arrange(f => f.Execute("SomeValue")).Throws<InvalidOperationException>();
	fooMock.Execute("SomeValue");

Execute a callback

	var mockContext = new MockContext<IFoo>();
	var fooMock = new FooMock(mockContext);
	string callBackResult = null;
	mockContext.Arrange(f => f.Execute(The<string>.IsAnyValue)).Callback<string>(s => callBackResult = s);

	fooMock.Execute("SomeValue");

	Assert.AreEqual("SomeValue", callBackResult);




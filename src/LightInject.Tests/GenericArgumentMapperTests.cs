
namespace LightInject.Tests

{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Linq;
    using SampleLibrary;
    using Xunit;

    public class GenericArgumentMapperTests
    {
        [Fact]
        public void Map_GenericTypeDefinitionToImplementingType_IsValid()
        {        
            var result = CreateMapper().Map(typeof (IFoo<>), typeof (Foo<>));
            Assert.True(result.IsValid);
        }       

        [Fact]
        public void Map_ImplementedInterfaceToImplementingType_IsValid()
        {
            var result = CreateMapper().Map(typeof(Foo<>).GetInterfaces().Single(), typeof(Foo<>));
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Map_ClosedGenericToOpenGenericImplementingType_IsValid()
        {            
            var result = CreateMapper().Map(typeof (IFoo<string>), typeof (Foo<>));
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Map_MissingArgument_IsNotValid()
        {
            var result = CreateMapper().Map(typeof(ICollection), typeof(Collection<>));
            Assert.False(result.IsValid);
        }
        
        [Fact]
        public void Map_PartiallyClosedGeneric_ReturnsArgumentForUnclosedParameter()
        {
            var result = CreateMapper().Map(typeof(IFoo<string,int>), typeof(HalfClosedFoo<>));
            Assert.True(result.GetMappedArguments().Single() == typeof(int));
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Map_PartiallyClosedGenericFromOpenGenericServiceType_IsValid()
        {
            var result = CreateMapper().Map(typeof(IFoo<,>), typeof(HalfClosedFoo<>));           
            Assert.True(result.IsValid);
        }


        [Fact]
        public void Map_PartiallyClosedGenericWithNestedArgument_ReturnsArgumentForUnClosedParameter()
        {
            var result = CreateMapper().Map(typeof(IFoo<Lazy<int>, string>), typeof(HalfClosedFooWithNestedGenericParameter<>));
            Assert.True(result.GetMappedArguments().Single() == typeof(int));
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Map_PartyiallyClosedFooWithNestedArgumentFromOpenGenericServiceType_IsValid()
        {
            var result = CreateMapper().Map(typeof(IFoo<,>), typeof(HalfClosedFooWithNestedGenericParameter<>));
            Assert.True(result.IsValid);
        }




        [Fact]
        public void Map_PartiallyClosedGenericWithDoubleNestedArgument_ReturnsArgumentForUnClosedParameter()
        {
            var result = CreateMapper().Map(typeof(IFoo<Lazy<Nullable<int>>, string>), typeof(HalfClosedFooWithDoubleNestedGenericParameter<>));
            Assert.True(result.GetMappedArguments().Single() == typeof(int));
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Map_PartialClosedAbstractClass_ReturnsArgumentForUnClosedParameter()
        {
            var result = CreateMapper().Map(typeof(Foo<string, int>), typeof(HalfClosedFooInhertingFromBaseClass<>));
            Assert.True(result.GetMappedArguments().Single() == typeof(int));
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Map_ConcreteClass_Map_PartialClosedAbstractClass_ReturnsArgumentForUnClosedParameter()
        {
            var result = CreateMapper().Map(typeof(Foo<int>), typeof(Foo<>));
            Assert.True(result.GetMappedArguments().Single() == typeof(int));
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Map_InheritedFromOpenGeneric_ReturnsArgumentForUnClosedParameter()
        {
            var result = CreateMapper().Map(typeof(GenericFoo<int>), typeof(AnotherGenericFoo<>));
            Assert.True(result.GetMappedArguments().Single() == typeof(int));
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Map_NonRelatedInterface_ThrowsException()
        {
            var exception = Assert.Throws<InvalidOperationException>(() => CreateMapper().Map(typeof(IFoo<>), typeof(Bar<>)));
            Assert.StartsWith("The generic type definition", exception.Message);
        }

        [Fact]
        public void Map_NonRelatedBaseType_ThrowsException()
        {
            var exception = Assert.Throws<InvalidOperationException>(() => CreateMapper().Map(typeof(Foo<>), typeof(Bar<>)));
            Assert.StartsWith("The generic type definition", exception.Message);
        }


        private static GenericArgumentMapper CreateMapper()
        {
            GenericArgumentMapper mapper = new GenericArgumentMapper();
            return mapper;
        }
    }
}
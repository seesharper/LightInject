﻿namespace LightInject.Interception.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;

    using Microsoft.VisualStudio.TestTools.UnitTesting;


    [TestClass]    
    public class ProxyBuilderVerificationTests : ProxyBuilderTests
    {
        internal override IProxyBuilder CreateProxyBuilder()
        {
            var proxyBuilder = new ProxyBuilder();
            var field = typeof(ProxyBuilder).GetField(
                "typeBuilderFactory", BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(proxyBuilder, new VerifiableTypeBuilderFactory());
            return proxyBuilder;
        }
    }
}
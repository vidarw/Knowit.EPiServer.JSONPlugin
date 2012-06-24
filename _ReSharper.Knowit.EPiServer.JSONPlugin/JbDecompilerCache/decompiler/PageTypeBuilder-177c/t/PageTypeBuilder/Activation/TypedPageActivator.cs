// Type: PageTypeBuilder.Activation.TypedPageActivator
// Assembly: PageTypeBuilder, Version=2.0.0.0, Culture=neutral, PublicKeyToken=6fb8762af0e6dbed
// Assembly location: C:\Users\wrdx\Documents\GitHub\Knowit.EPiServer.JSONPlugin\packages\PageTypeBuilder.2.0\lib\net35\PageTypeBuilder.dll

using Castle.DynamicProxy;
using EPiServer.Core;
using PageTypeBuilder;
using PageTypeBuilder.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PageTypeBuilder.Activation
{
  public class TypedPageActivator
  {
    private ProxyGenerator _generator;
    private ProxyGenerationOptions _options;
    private IInterceptor[] _interceptors;

    public TypedPageActivator()
      : this(TypedPageActivator.CreateDefaultProxyInterceptors(), TypedPageActivator.CreateDefaultProxyGenerationOptions(), TypedPageActivator.CreateDefaultProxyGenerator())
    {
    }

    public TypedPageActivator(IInterceptor[] interceptors)
      : this(interceptors, TypedPageActivator.CreateDefaultProxyGenerationOptions())
    {
    }

    public TypedPageActivator(ProxyGenerationOptions options)
      : this(TypedPageActivator.CreateDefaultProxyInterceptors(), options, TypedPageActivator.CreateDefaultProxyGenerator())
    {
    }

    public TypedPageActivator(IInterceptor[] interceptors, ProxyGenerationOptions options)
      : this(interceptors, options, TypedPageActivator.CreateDefaultProxyGenerator())
    {
    }

    public TypedPageActivator(IInterceptor[] interceptors, ProxyGenerationOptions options, ProxyGenerator generator)
    {
      this._generator = generator;
      this._options = options;
      this._interceptors = interceptors;
    }

    protected static IInterceptor[] CreateDefaultProxyInterceptors()
    {
      return new IInterceptor[1]
      {
        (IInterceptor) new PageTypePropertyInterceptor()
      };
    }

    protected static ProxyGenerationOptions CreateDefaultProxyGenerationOptions()
    {
      return new ProxyGenerationOptions((IProxyGenerationHook) new PageTypePropertiesProxyGenerationHook());
    }

    protected static ProxyGenerator CreateDefaultProxyGenerator()
    {
      return new ProxyGenerator();
    }

    public virtual TypedPageData CreateAndPopulateTypedInstance(PageData originalPage, Type typedType)
    {
      TypedPageData instance = this.CreateInstance(typedType);
      TypedPageData.PopuplateInstance(originalPage, instance);
      PropertyInfo[] privateProperties = TypeExtensions.GetPublicOrPrivateProperties(instance.GetType());
      this.CreateAndPopulateNestedPropertyGroupInstances(instance, (object) instance, (IEnumerable<PropertyInfo>) privateProperties, string.Empty);
      return instance;
    }

    public virtual TypedPageData CreateInstance(Type typedType)
    {
      return this.CreateInstance(typedType, new object[0]);
    }

    protected virtual TypedPageData CreateInstance(Type typedType, object[] ctorArguments)
    {
      return (TypedPageData) this._generator.CreateClassProxy(typedType, new Type[0], this._options, ctorArguments, this._interceptors);
    }

    public virtual PageTypePropertyGroup CreatePropertyGroupInstance(Type typedType)
    {
      return this.CreatePropertyGroupInstance(typedType, new object[0]);
    }

    protected virtual PageTypePropertyGroup CreatePropertyGroupInstance(Type typedPropertyGroup, object[] ctorArguments)
    {
      return (PageTypePropertyGroup) this._generator.CreateClassProxy(typedPropertyGroup, new Type[0], this._options, ctorArguments, this._interceptors);
    }

    internal void CreateAndPopulateNestedPropertyGroupInstances(TypedPageData typedPage, object classInstance, IEnumerable<PropertyInfo> properties, string hierarchy)
    {
      foreach (PropertyInfo propertyInfo in Enumerable.Where<PropertyInfo>(properties, (Func<PropertyInfo, bool>) (current => current.PropertyType.BaseType == typeof (PageTypePropertyGroup))))
      {
        PageTypePropertyGroup propertyGroupInstance = this.CreatePropertyGroupInstance(propertyInfo.PropertyType);
        string hierarchy1 = PageTypePropertyGroupHierarchy.ResolvePropertyName(hierarchy, propertyInfo.Name);
        propertyGroupInstance.PopuplateInstance(typedPage, hierarchy1);
        propertyInfo.SetValue(classInstance, (object) propertyGroupInstance, (object[]) null);
      }
    }
  }
}

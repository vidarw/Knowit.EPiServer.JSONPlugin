// Type: PageTypeBuilder.PageTypeResolver
// Assembly: PageTypeBuilder, Version=2.0.0.0, Culture=neutral, PublicKeyToken=6fb8762af0e6dbed
// Assembly location: C:\Users\wrdx\Documents\GitHub\Knowit.EPiServer.JSONPlugin\packages\PageTypeBuilder.2.0\lib\net35\PageTypeBuilder.dll

using EPiServer.Core;
using PageTypeBuilder.Activation;
using System;
using System.Collections.Generic;

namespace PageTypeBuilder
{
  public class PageTypeResolver
  {
    private Dictionary<int, Type> _typeByPageTypeID = new Dictionary<int, Type>();
    private Dictionary<Type, int> _pageTypeIDByType = new Dictionary<Type, int>();
    private static PageTypeResolver _instance;

    public static PageTypeResolver Instance
    {
      get
      {
        if (PageTypeResolver._instance == null)
          PageTypeResolver._instance = new PageTypeResolver();
        return PageTypeResolver._instance;
      }
      internal set
      {
        PageTypeResolver._instance = value;
      }
    }

    public TypedPageActivator Activator { get; set; }

    protected internal PageTypeResolver()
    {
      this.Activator = new TypedPageActivator();
    }

    protected internal virtual void AddPageType(int pageTypeID, Type pageTypeType)
    {
      if (this.AlreadyAddedToTypeByPageTypeID(pageTypeID, pageTypeType))
        this._typeByPageTypeID.Add(pageTypeID, pageTypeType);
      if (!this.AlreadyAddedToPageTypeIDByType(pageTypeID, pageTypeType))
        return;
      this._pageTypeIDByType.Add(pageTypeType, pageTypeID);
    }

    private bool AlreadyAddedToTypeByPageTypeID(int pageTypeID, Type pageTypeType)
    {
      if (this._typeByPageTypeID.ContainsKey(pageTypeID))
        return this._typeByPageTypeID[pageTypeID] != pageTypeType;
      else
        return true;
    }

    private bool AlreadyAddedToPageTypeIDByType(int pageTypeID, Type pageTypeType)
    {
      if (this._pageTypeIDByType.ContainsKey(pageTypeType))
        return this._pageTypeIDByType[pageTypeType] != pageTypeID;
      else
        return true;
    }

    public virtual Type GetPageTypeType(int pageTypeID)
    {
      Type type = (Type) null;
      if (this._typeByPageTypeID.ContainsKey(pageTypeID))
        type = this._typeByPageTypeID[pageTypeID];
      return type;
    }

    public virtual int? GetPageTypeID(Type type)
    {
      int? nullable = new int?();
      if (this._pageTypeIDByType.ContainsKey(type))
        nullable = new int?(this._pageTypeIDByType[type]);
      return nullable;
    }

    public virtual PageData ConvertToTyped(PageData page)
    {
      Type pageTypeType = this.GetPageTypeType(page.PageTypeID);
      if (pageTypeType == null)
        return page;
      else
        return (PageData) this.Activator.CreateAndPopulateTypedInstance(page, pageTypeType);
    }
  }
}

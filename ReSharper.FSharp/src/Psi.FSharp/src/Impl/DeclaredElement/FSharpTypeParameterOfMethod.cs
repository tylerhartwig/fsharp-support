﻿using System.Collections.Generic;
using System.Xml;
using JetBrains.Annotations;
using JetBrains.Metadata.Reader.API;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using JetBrains.Util.DataStructures;

namespace JetBrains.ReSharper.Psi.FSharp.Impl.DeclaredElement
{
  internal class FSharpTypeParameterOfMethod : ITypeParameter
  {
    [NotNull] private readonly FSharpMethod myMethod;

    public FSharpTypeParameterOfMethod([NotNull] FSharpMethod method, [NotNull] string name, int index)
    {
      myMethod = method;
      Index = index;
      ShortName = name;
    }

    public IPsiServices GetPsiServices()
    {
      return myMethod.GetPsiServices();
    }

    public IList<IDeclaration> GetDeclarations()
    {
      return EmptyList<IDeclaration>.Instance;
    }

    public IList<IDeclaration> GetDeclarationsIn(IPsiSourceFile sourceFile)
    {
      return EmptyList<IDeclaration>.Instance;
    }

    public DeclaredElementType GetElementType()
    {
      return CLRDeclaredElementType.TYPE_PARAMETER;
    }

    public XmlNode GetXMLDoc(bool inherit)
    {
      return null;
    }

    public XmlNode GetXMLDescriptionSummary(bool inherit)
    {
      return null;
    }

    public bool IsValid()
    {
      return true;
    }

    public bool IsSynthetic()
    {
      return false;
    }

    public HybridCollection<IPsiSourceFile> GetSourceFiles()
    {
      return HybridCollection<IPsiSourceFile>.Empty;
    }

    public bool HasDeclarationsIn(IPsiSourceFile sourceFile)
    {
      return false;
    }

    public string ShortName { get; }
    public bool CaseSensitiveName => true;
    public PsiLanguageType PresentationLanguage => FSharpLanguage.Instance;

    public ITypeElement GetContainingType()
    {
      throw new System.NotImplementedException();
    }

    public ITypeMember GetContainingTypeMember()
    {
      return myMethod;
    }

    public IPsiModule Module => myMethod.Module;
    public ISubstitution IdSubstitution => EmptySubstitution.INSTANCE;

    public IList<ITypeParameter> TypeParameters => EmptyList<ITypeParameter>.Instance;

    public IList<IAttributeInstance> GetAttributeInstances(bool inherit)
    {
      return EmptyList<IAttributeInstance>.Instance;
    }

    public IList<IAttributeInstance> GetAttributeInstances(IClrTypeName clrName, bool inherit)
    {
      return EmptyList<IAttributeInstance>.Instance;
    }

    public bool HasAttributeInstance(IClrTypeName clrName, bool inherit)
    {
      return false;
    }

    public IClrTypeName GetClrName()
    {
      return EmptyClrTypeName.Instance;
    }

    public IList<IDeclaredType> GetSuperTypes()
    {
      throw new System.NotImplementedException();
    }

    public IEnumerable<ITypeMember> GetMembers()
    {
      return EmptyList<ITypeMember>.InstanceList;
    }

    public INamespace GetContainingNamespace()
    {
      return myMethod.GetContainingType()?.GetContainingNamespace()
             ?? Module.GetPsiServices().Symbols.GetSymbolScope(LibrarySymbolScope.FULL, true).GlobalNamespace;
    }

    public IPsiSourceFile GetSingleOrDefaultSourceFile()
    {
      throw new System.NotImplementedException();
    }

    public IList<ITypeElement> NestedTypes => EmptyList<ITypeElement>.InstanceList;
    public IEnumerable<IConstructor> Constructors => EmptyList<IConstructor>.Instance;
    public IEnumerable<IOperator> Operators => EmptyList<IOperator>.Instance;
    public IEnumerable<IMethod> Methods => EmptyList<IMethod>.Instance;
    public IEnumerable<IProperty> Properties => EmptyList<IProperty>.Instance;
    public IEnumerable<IEvent> Events => EmptyList<IEvent>.Instance;
    public IEnumerable<string> MemberNames => EmptyList<string>.InstanceList;

    public int Index { get; }
    public TypeParameterVariance Variance => TypeParameterVariance.INVARIANT;
    public bool IsValueType => true; // todo
    public bool IsClassType => true; // todo
    public bool HasDefaultConstructor => false;
    public IList<IType> TypeConstraints => EmptyList<IType>.Instance;
    public ITypeParametersOwner Owner => myMethod;
    public ITypeElement OwnerType => myMethod.GetContainingType();
    public IMethod OwnerMethod => myMethod;
  }
}
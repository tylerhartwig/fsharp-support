﻿using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.FSharp.Impl.Tree;
using JetBrains.ReSharper.Psi.FSharp.Tree;
using JetBrains.ReSharper.Psi.FSharp.Util;
using JetBrains.ReSharper.Psi.Impl.Special;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using JetBrains.Util.dataStructures;
using Microsoft.FSharp.Compiler.SourceCodeServices;

namespace JetBrains.ReSharper.Psi.FSharp.Impl.DeclaredElement
{
  internal class FSharpMethod : FSharpMemberBase<MemberDeclaration>, IMethod
  {
    public FSharpMethod([NotNull] ITypeMemberDeclaration declaration, [CanBeNull] FSharpMemberOrFunctionOrValue mfv,
      IFSharpTypeParametersOwnerDeclaration typeDeclaration) : base(declaration, mfv)
    {
      if (mfv == null || typeDeclaration == null)
      {
        TypeParameters = EmptyList<ITypeParameter>.Instance;
        ReturnType = TypeFactory.CreateUnknownType(Module);
        Parameters = EmptyList<IParameter>.Instance;
        ShortName = declaration.DeclaredName;
        return;
      }

      var mfvTypeParams = mfv.GenericParameters;
      var typeParams = new FrugalLocalList<ITypeParameter>();
      var outerTypeParamsCount = typeDeclaration.TypeParameters.Count;
      for (var i = outerTypeParamsCount; i < mfvTypeParams.Count; i++)
        typeParams.Add(new FSharpTypeParameterOfMethod(this, mfvTypeParams[i].DisplayName, i - outerTypeParamsCount));
      TypeParameters = typeParams.ToList();

      ReturnType = FSharpElementsUtil.GetType(mfv.ReturnParameter.Type, declaration, TypeParameters, Module) ??
                   TypeFactory.CreateUnknownType(Module);

      var methodParams = new FrugalLocalList<IParameter>();
      foreach (var paramsGroup in mfv.CurriedParameterGroups)
      foreach (var param in paramsGroup)
      {
        methodParams.Add(new Parameter(this, methodParams.Count, ParameterKind.VALUE,
          FSharpElementsUtil.GetType(param.Type, declaration, TypeParameters, Module),
          param.DisplayName));
      }
      Parameters = methodParams.ToList();
      ShortName = mfv.CompiledName;
    }

    public override string ShortName { get; }
    public override IType ReturnType { get; }
    public override IList<IParameter> Parameters { get; }

    public override DeclaredElementType GetElementType()
    {
      return CLRDeclaredElementType.METHOD;
    }

    public bool IsPredefined => false;
    public bool IsIterator => false;
    public IAttributesSet ReturnTypeAttributes => EmptyAttributesSet.Instance;
    public bool IsExplicitImplementation => false;
    public IList<IExplicitImplementation> ExplicitImplementations => EmptyList<IExplicitImplementation>.Instance;
    public bool CanBeImplicitImplementation => false;
    public IList<ITypeParameter> TypeParameters { get; }
    public bool IsExtensionMethod => false;
    public bool IsAsync => false;
    public bool IsVarArg => false;
    public bool IsXamlImplicitMethod => false;
  }
}
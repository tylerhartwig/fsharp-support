﻿using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.FSharp.Impl.Tree;
using JetBrains.ReSharper.Psi.FSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using Microsoft.FSharp.Compiler.SourceCodeServices;

namespace JetBrains.ReSharper.Psi.FSharp.Impl.DeclaredElement
{
  internal abstract class FSharpMemberBase<TDeclaration> : FSharpTypeMember<TDeclaration>, IParametersOwner
    where TDeclaration : FSharpDeclarationBase, IFSharpDeclaration, IAccessRightsOwnerDeclaration,
    IModifiersOwnerDeclaration
  {
    protected FSharpMemberBase([NotNull] ITypeMemberDeclaration declaration,
      [CanBeNull] FSharpMemberOrFunctionOrValue mfv) : base(declaration)
    {
      IsStatic = !mfv?.IsInstanceMember ?? false;
      IsOverride = mfv?.IsOverrideOrExplicitInterfaceImplementation ?? false;
    }

    public InvocableSignature GetSignature(ISubstitution substitution)
    {
      return new InvocableSignature(this, substitution);
    }

    public IEnumerable<IParametersOwnerDeclaration> GetParametersOwnerDeclarations()
    {
      return EmptyList<IParametersOwnerDeclaration>.Instance;
    }

    public virtual IList<IParameter> Parameters => EmptyList<IParameter>.Instance;
    public bool IsRefReturn => false;
    public abstract override string ShortName { get; }
    public abstract IType ReturnType { get; }

    public override bool IsStatic { get; }
    public override bool IsOverride { get; }
  }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ploeh.Albedo
{
    public class TypeElementMaterializer<T> : IReflectionElementMaterializer<T>
    {
        public IEnumerable<IReflectionElement> Materialize(IEnumerable<T> source)
        {
            return source
                .OfType<Type>()
                .Select(t => new TypeElement(t))
                .Cast<IReflectionElement>();
        }
    }
}

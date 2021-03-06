﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Albedo
{
    /// <summary>
    /// Contains extension methods for dealing with <see cref="IReflectionElement"/>
    /// instances, both producing and consuming them.
    /// </summary>
    public static class ReflectionElementEnvy
    {
        /// <summary>
        /// Gets 'proper' methods from the <paramref name="type"/>; that is, methods excluding
        /// property accessors, and matching the provided <paramref name="bindingAttr"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> from which methods are selected</param>
        /// <param name="bindingAttr">The <see cref="BindingFlags"/> used to determine which
        /// methods to return.</param>
        /// <returns>The sequence of <see cref="IReflectionElement"/> instances representing
        /// the matching methods found on the <paramref name="type"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly",
           MessageId = "Attr", Justification = "This is the standard naming pattern for a BindingFlags parameter, used in the .NET Framework.")]
        public static IEnumerable<IReflectionElement> GetProperMethods(
            this Type type, BindingFlags bindingAttr)
        {
            if (type == null) throw new ArgumentNullException("type");
            return type
                .GetMethods(bindingAttr)
                .Except(type
                    .GetProperties()
                    .SelectMany(pi => pi.GetAccessors()))
                .Select(mi => new MethodInfoElement(mi))
                .Cast<IReflectionElement>();
        }

        /// <summary>
        /// Gets 'proper' methods from the <paramref name="type"/>; that is, methods excluding
        /// property accessors. Public | Static | Instance methods are returned.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> from which methods are selected</param>
        /// <returns>The sequence of <see cref="IReflectionElement"/> instances representing
        /// the public static or public instance methods on the <paramref name="type"/>.</returns>
        public static IEnumerable<IReflectionElement> GetProperMethods(this Type type)
        {
            return type.GetProperMethods(
                BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        }

        /// <summary>
        /// Accepts the <see cref="IReflectionVisitor{T}"/> visitor on each of the
        /// <paramref name="elements"/> in the sequence.
        /// </summary>
        /// <typeparam name="T">
        /// The type of observation or result which the <see cref="IReflectionVisitor{T}"/>
        /// instance produces when visiting nodes.
        /// </typeparam>
        /// <param name="elements">
        /// The sequence of <see cref="IReflectionElement"/> instances upon which 
        /// the <see cref="IReflectionElement.Accept{T}"/> method will be called.
        /// </param>
        /// <param name="visitor">The <see cref="IReflectionVisitor{T}"/> instance.</param>
        /// <returns>
        /// A (potentially) new <see cref="IReflectionVisitor{T}"/> instance which can be
        /// used to continue the visiting process with potentially updated observations.
        /// </returns>
        public static IReflectionVisitor<T> Accept<T>(
            this IEnumerable<IReflectionElement> elements,
            IReflectionVisitor<T> visitor)
        {
            if (elements == null) throw new ArgumentNullException("elements");
            return new CompositeReflectionElement(elements.ToArray()).Accept(visitor);
        }

        /// <summary>
        /// Gets the matching properties and fields from the <paramref name="type"/>, and
        /// returns them as a sequence of <see cref="IReflectionElement"/> instances.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> which properties and fields are
        /// obtained from.</param>
        /// <param name="bindingAttr">The <see cref="BindingFlags"/> used determine which
        /// properties and fields to get.</param>
        /// <returns>The sequence of <see cref="IReflectionElement"/> instances representing
        /// the matching properties and fields from the <paramref name="type"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly",
            MessageId = "Attr", Justification = "This is the standard naming pattern for a BindingFlags parameter, used in the .NET Framework.")]
        public static IEnumerable<IReflectionElement> GetPropertiesAndFields(
            this Type type, BindingFlags bindingAttr)
        {
            if (type == null) throw new ArgumentNullException("type");
            return type
                .GetProperties(bindingAttr)
                .Select(pi => new PropertyInfoElement(pi))
                .Cast<IReflectionElement>()
                .Concat(type
                    .GetFields(bindingAttr)
                    .Select(fi => new FieldInfoElement(fi))
                    .Cast<IReflectionElement>());
        }

        /// <summary>
        /// Gets the public instance properties and fields from the <paramref name="type"/>,
        /// and returns them as a sequence of <see cref="IReflectionElement"/> instances.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> which properties and fields are
        /// obtained from.</param>
        /// <returns>The sequence of <see cref="IReflectionElement"/> instances representing
        /// the public instance properties and fields from the 
        /// <paramref name="type"/>.</returns>
        public static IEnumerable<IReflectionElement> GetPublicPropertiesAndFields(this Type type)
        {
            return type.GetPropertiesAndFields(
                BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        }
    }
}
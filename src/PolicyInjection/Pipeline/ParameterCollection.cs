

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Interception.Utilities;

namespace Unity.Interception.PolicyInjection.Pipeline
{
    /// <summary>
    /// An implementation of <see cref="IParameterCollection"/> that wraps a provided array
    /// containing the argument values.
    /// </summary>
    public class ParameterCollection : IParameterCollection
    {
        /// <summary>
        /// An internal struct that maps the index in the arguments collection to the
        /// corresponding <see cref="ParameterInfo"/> about that argument.
        /// </summary>
        private struct ArgumentInfo
        {
            public readonly int Index;
            public readonly string Name;
            public readonly ParameterInfo ParameterInfo;

            /// <summary>
            /// Construct a new <see cref="ArgumentInfo"/> object linking the
            /// given index and <see cref="ParameterInfo"/> object.
            /// </summary>
            /// <param name="index">Index into arguments array (zero-based).</param>
            /// <param name="parameterInfo"><see cref="ParameterInfo"/> for the argument at <paramref name="index"/>.</param>
            public ArgumentInfo(int index, ParameterInfo parameterInfo)
            {
                Index = index;
                Name = parameterInfo.Name;
                ParameterInfo = parameterInfo;
            }
        }

        private readonly List<ArgumentInfo> _argumentInfo;
        private readonly object[] _arguments;

        /// <summary>
        /// Construct a new <see cref="ParameterCollection"/> that wraps the
        /// given array of arguments.
        /// </summary>
        /// <param name="arguments">Complete collection of arguments.</param>
        /// <param name="argumentInfo">Type information about each parameter.</param>
        /// <param name="isArgumentPartOfCollection">A <see cref="Predicate{T}"/> that indicates
        /// whether a particular parameter is part of the collection. Used to filter out only input
        /// parameters, for example.</param>
        public ParameterCollection(object[] arguments, ParameterInfo[] argumentInfo, Predicate<ParameterInfo> isArgumentPartOfCollection)
        {
            Guard.ArgumentNotNull(arguments, "arguments");
            Guard.ArgumentNotNull(isArgumentPartOfCollection, "isArgumentPartOfCollection");

            _arguments = arguments;
            _argumentInfo = new List<ArgumentInfo>();
            for (int argumentNumber = 0; argumentNumber < argumentInfo.Length; ++argumentNumber)
            {
                if (isArgumentPartOfCollection(argumentInfo[argumentNumber]))
                {
                    _argumentInfo.Add(new ArgumentInfo(argumentNumber, argumentInfo[argumentNumber]));
                }
            }
        }

        /// <summary>
        /// Fetches a parameter's value by name.
        /// </summary>
        /// <param name="parameterName">parameter name.</param>
        /// <value>value of the named parameter.</value>
        public object this[string parameterName]
        {
            get { return _arguments[_argumentInfo[IndexForInputParameterName(parameterName)].Index]; }

            set { _arguments[_argumentInfo[IndexForInputParameterName(parameterName)].Index] = value; }
        }

        private int IndexForInputParameterName(string paramName)
        {
            for (int i = 0; i < _argumentInfo.Count; ++i)
            {
                if (_argumentInfo[i].Name == paramName)
                {
                    return i;
                }
            }
            throw new ArgumentException("Invalid parameter Name", "paramName");
        }

        /// <summary>
        /// Gets the value of a parameter based on index.
        /// </summary>
        /// <param name="index">Index of parameter to get the value for.</param>
        /// <value>Value of the requested parameter.</value>
        public object this[int index]
        {
            get => _arguments[_argumentInfo[index].Index];
            set => _arguments[_argumentInfo[index].Index] = value;
        }

        /// <summary>
        /// Gets the ParameterInfo for a particular parameter by index.
        /// </summary>
        /// <param name="index">Index for this parameter.</param>
        /// <returns>ParameterInfo object describing the parameter.</returns>
        public ParameterInfo GetParameterInfo(int index)
        {
            return _argumentInfo[index].ParameterInfo;
        }

        /// <summary>
        /// Gets the <see cref="ParameterInfo"/> for the given named parameter.
        /// </summary>
        /// <param name="parameterName">Name of parameter.</param>
        /// <returns><see cref="ParameterInfo"/> for the requested parameter.</returns>
        public ParameterInfo GetParameterInfo(string parameterName)
        {
            return _argumentInfo[IndexForInputParameterName(parameterName)].ParameterInfo;
        }

        /// <summary>
        /// Gets the name of a parameter based on index.
        /// </summary>
        /// <param name="index">Index of parameter to get the name for.</param>
        /// <returns>Name of the requested parameter.</returns>
        public string ParameterName(int index)
        {
            return _argumentInfo[index].Name;
        }

        /// <summary>
        /// Does this collection contain a parameter value with the given name?
        /// </summary>
        /// <param name="parameterName">Name of parameter to find.</param>
        /// <returns>True if the parameter name is in the collection, false if not.</returns>
        public bool ContainsParameter(string parameterName)
        {
            return _argumentInfo.Any(info => info.Name == parameterName);
        }

        /// <summary>
        /// Adds to the collection. This is a read only collection, so this method
        /// always throws <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="value">Object to add.</param>
        /// <returns>Nothing, always throws.</returns>
        /// <exception cref="NotSupportedException">Always throws this.</exception>
        public int Add(object value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Checks to see if the collection contains the given object.
        /// </summary>
        /// <remarks>Tests for the object using object.Equals.</remarks>
        /// <param name="value">Object to find.</param>
        /// <returns>true if object is in collection, false if it is not.</returns>
        public bool Contains(object value)
        {
            return
                _argumentInfo.Exists(
                    delegate(ArgumentInfo info)
                    {
                        var argument = _arguments[info.Index];

                        if (argument == null)
                        {
                            return value == null;
                        }

                        return argument.Equals(value);
                    });
        }

        /// <summary>
        /// Remove all items in the collection. This collection is fixed-size, so this
        /// method always throws <see cref="NotSupportedException"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">This is always thrown.</exception>
        public void Clear()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns the index of the given object, or -1 if not found.
        /// </summary>
        /// <param name="value">Object to find.</param>
        /// <returns>zero-based index of found object, or -1 if not found.</returns>
        public int IndexOf(object value)
        {
            return _argumentInfo.FindIndex(
                delegate(ArgumentInfo info)
                {
                    return _arguments[info.Index].Equals(value);
                });
        }

        /// <summary>
        /// Inserts a new item. This is a fixed-size collection, so this method throws <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="index">Index to insert at.</param>
        /// <param name="value">Always throws.</param>
        /// <exception cref="NotSupportedException">Always throws this.</exception>
        public void Insert(int index, object value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Removes the given item. This is a fixed-size collection, so this method throws <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="value">Always throws.</param>
        /// <exception cref="NotSupportedException">Always throws this.</exception>
        public void Remove(object value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Removes the given item. This is a fixed-size collection, so this method throws <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="index">Always throws.</param>
        /// <exception cref="NotSupportedException">Always throws this.</exception>
        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Is this collection read only?
        /// </summary>
        /// <value>No, it is not read only, the contents can change.</value>
        public bool IsReadOnly { get; } = false;

        /// <summary>
        /// Is this collection fixed size?
        /// </summary>
        /// <value>Yes, it is.</value>
        public bool IsFixedSize { get; } = true;

        /// <summary>
        /// Copies the contents of this collection to the given array.
        /// </summary>
        /// <param name="array">Destination array.</param>
        /// <param name="index">index to start copying from.</param>
        public void CopyTo(Array array, int index)
        {
            int destIndex = 0;
            _argumentInfo.GetRange(index, _argumentInfo.Count - index).ForEach(
                delegate(ArgumentInfo info)
                {
                    array.SetValue(_arguments[info.Index], destIndex);
                    ++destIndex;
                });
        }

        /// <summary>
        /// Total number of items in the collection.
        /// </summary>
        /// <value>The count.</value>
        public int Count => _argumentInfo.Count;

        /// <summary>
        /// Gets a synchronized version of this collection. WARNING: Not implemented completely,
        /// DO NOT USE THIS METHOD.
        /// </summary>
        public object SyncRoot => this;

        /// <summary>
        /// Is the object synchronized for thread safety?
        /// </summary>
        /// <value>No, it isn't.</value>
        public bool IsSynchronized => false;

        /// <summary>
        /// Gets an enumerator object to support the foreach construct.
        /// </summary>
        /// <returns>Enumerator object.</returns>
        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < _argumentInfo.Count; ++i)
            {
                yield return _arguments[_argumentInfo[i].Index];
            }
        }
    }
}

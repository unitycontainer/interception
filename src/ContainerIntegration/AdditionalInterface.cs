using System;
using System.Globalization;
using Unity.Injection;
using Unity.Interception.ContainerIntegration.ObjectBuilder;
using Unity.Interception.Properties;
using Unity.Policy;

namespace Unity.Interception.ContainerIntegration
{
    /// <summary>
    /// Stores information about a single <see cref="Type"/> to be an additional interface for an intercepted object and
    /// configures a container accordingly.
    /// </summary>
    public class AdditionalInterface : InterceptionMember// TODO: IAddPolicies
    {
        private readonly Type _additionalInterface;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdditionalInterface"/> with a 
        /// <see cref="Type"/>.
        /// </summary>
        /// <param name="additionalInterface">A descriptor representing the interception behavior to use.</param>
        /// <exception cref="ArgumentNullException">when <paramref name="additionalInterface"/> is
        /// <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">when <paramref name="additionalInterface"/> is not an interface.
        /// </exception>
        public AdditionalInterface(Type additionalInterface)
        {
            if (!(additionalInterface ?? throw new ArgumentNullException(nameof(additionalInterface))).IsInterface)
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.ExceptionTypeIsNotInterface,
                        additionalInterface.Name),
                    nameof(additionalInterface));
            }

            _additionalInterface = additionalInterface;
        }

        public override bool BuildRequired => false;

        public void AddPolicies<TPolicySet>(Type type, string name, ref TPolicySet policies) 
            where TPolicySet : IPolicySet
        {
            AdditionalInterfacesPolicy policy = AdditionalInterfacesPolicy.GetOrCreate(ref policies);
            policy.AddAdditionalInterface(_additionalInterface);
        }
    }

    /// <summary>
    /// Stores information about a single <see cref="Type"/> to be an additional interface for an intercepted object and
    /// configures a container accordingly.
    /// </summary>
    /// <typeparam name="T">The interface.</typeparam>
    public class AdditionalInterface<T> : AdditionalInterface
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdditionalInterface{T}"/>.
        /// </summary>
        public AdditionalInterface()
            : base(typeof(T))
        {
        }
    }
}

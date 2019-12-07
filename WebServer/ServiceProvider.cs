using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpkServices
{
    /// <summary>
    /// Глобальная точка доступа к зарегистрированным в системе сервисам
    /// </summary>
    public static class ServiceProvider
    {
        static Dictionary<Type, Tuple<Type, object[]>> registeredServices;
        /// <summary>
        /// Зарегистрированные типы-обработчики
        /// </summary>
        static Dictionary<Type, Tuple<Type, object[]>> RegisteredServices
        {
            get => registeredServices ?? (registeredServices = new Dictionary<Type, Tuple<Type, object[]>>());
        }
        static Dictionary<Type, object> registeredServiceInstances;
        /// <summary>
        /// Зарегистрированные объекты-обработчики
        /// </summary>
        static Dictionary<Type, object> RegisteredInstances
        {
            get => registeredServiceInstances ?? (registeredServiceInstances = new Dictionary<Type, object>());
        }

        public static void RegisterService( Type serviceType, Type handlerType, params object[] parameters)
        {
            if (serviceType.Equals(handlerType) == false)
            {
                if (serviceType == null || !serviceType.IsInterface)
                {
                    throw new ArgumentException("service param must be an interface!");
                }
                if (handlerType == null || !handlerType.GetInterfaces().Contains(serviceType))
                {
                    throw new ArgumentException("handler must be derived class from service interface!");
                }
            }
            RegisteredServices[serviceType] = Tuple.Create(handlerType, parameters);
        }

        public static void RegisterService<T>(object handler)
        {
            var handlerType = typeof(T);
            RegisteredInstances[handlerType] = handler;
        }

        public static T GetService<T>() where T :class
        {
            var serviceInterface = typeof(T);
            if( RegisteredServices.ContainsKey(serviceInterface)) {
                var handler = RegisteredServices[serviceInterface];
                return Activator.CreateInstance(handler.Item1, handler.Item2) as T;
            }else if( RegisteredInstances.ContainsKey(serviceInterface) ) {
                return RegisteredInstances[serviceInterface] as T;
            }
            throw new NotImplementedException();
        }
        
    }
}

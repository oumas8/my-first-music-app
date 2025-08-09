using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace infrastructure.Helpers
{
    public static class IoCContainer
    {
        public static IUnityContainer Current { get; set; } = new UnityContainer();
        public static T Resolve<T>()
        {
            return Current.Resolve<T>();
        }
    }
}
